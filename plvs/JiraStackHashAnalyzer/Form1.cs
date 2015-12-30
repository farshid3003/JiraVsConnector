using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Security.Cryptography;
using Npgsql;

namespace JiraStackHashAnalyzer {
    public partial class Form1 : Form {

        private Thread workerThread;
        private readonly NpgsqlConnection connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=postgres;Password=dupa;Database=jddet;");
        private readonly NpgsqlConnection connJiraDb = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=postgres;Password=dupa;Database=jira;");

        public Form1() {
            InitializeComponent();

            textServerName.Text = "stac";
            textUrl.Text = "https://studio.atlassian.com";
            textProject.Text = "PL";
            textUser.Text = "jgorycki";
            textAnalyzeServerName.Text = "stac";
            textAnalyzeProject.Text = "PL";
            numericProjectId.Value = 10240;
        }

        protected override void OnLoad(EventArgs e) {
            initializeDb();
        }

        protected override void OnClosing(CancelEventArgs e) {
            abortQuery();
        }

        private void initializeDb() {
            try {
                appendToCollectLog("Testing connection to database...");
                connection.Open();
                connection.Close();
                appendToCollectLog("... ok");
            } catch (Exception e) {
                appendToCollectLog(e);
            }
        }

        private void appendToCollectLog(object txt) {
            appendToLogControl(txt, textLog);
        }

        private void appendToAnalyzeLog(object txt) {
            appendToLogControl(txt, textAnalyzeLog);
        }

        private void appendToAnalyzeDbLog(object txt) {
            appendToLogControl(txt, textAnalyzeDbLog);
        }

        private void appendToLogControl(object txt, TextBoxBase box) {
            BeginInvoke(new MethodInvoker(delegate {
                                              box.Text = box.Text + txt + "\r\n";
                                              box.SelectionStart = box.TextLength;
                                              box.ScrollToCaret();
                                          }));
        }

        private void buttonGo_Click(object sender, EventArgs e) {
            textAnalyzeLog.Text = "";
            enableControls(false);
            workerThread = new Thread(() => grabIssuesWorker(textServerName.Text, textUrl.Text, textProject.Text, textUser.Text, textPassword.Text));
            workerThread.Start();
        }

        private void buttonStop_Click(object sender, EventArgs e) {
            enableControls(true);
            abortQuery();
        }

        private void abortQuery() {
            if (workerThread == null) return;

            try {
                workerThread.Abort();
                workerThread.Join();
            } catch (Exception ex) {
                appendToCollectLog(ex);
            }
            workerThread = null;
        }

        private void enableControls(bool enable) {
            buttonGo.Enabled = enable;
            buttonStop.Enabled = !enable;
            textUser.Enabled = enable;
            textPassword.Enabled = enable;
            textUrl.Enabled = enable;
            textServerName.Enabled = enable;
            textProject.Enabled = enable;
        }

        private const int BATCH = 100;

        private void grabIssuesWorker(string srvName, string url, string projectKey, string user, string password) {
            try {
                connection.Open();
                JiraServer server = new JiraServer(srvName, url, user, password, true);

                int serverId;

                bool serverExists = getServer(server.Name, out serverId);
                NpgsqlCommand command;

                if (!serverExists) {
                    appendToCollectLog("Adding server " + srvName + " to database...");
                    command = new NpgsqlCommand(
                        string.Format("insert into servers (name, url) values ('{0}', '{1}')", server.Name, server.Url), connection);
                    command.ExecuteNonQuery();
                }

                getServer(server.Name, out serverId);

                int projectId;

                bool projectExists = getProject(projectKey, serverId, out projectId);

                if (!projectExists) {
                    appendToCollectLog("Adding project " + projectKey + " to database...");
                    command = new NpgsqlCommand(
                        string.Format("insert into projects (server, key) values ('{0}', '{1}')", serverId, projectKey), connection);
                    command.ExecuteNonQuery();
                }

                getProject(projectKey, serverId, out projectId);

                appendToCollectLog("querying JIRA for project " + projectKey + "...");

                int retrieved;
                int last = 0;
                List<JiraProject> projects = JiraServerFacade.Instance.getProjects(server);
                JiraProject project = projects.FirstOrDefault(p => p.Key.Equals(projectKey));
                if (project == null) {
                    throw new Exception("No such project in JIRA");
                }
                int total = 0;
                do {
                    appendToCollectLog(string.Format("retrieving issues {0} to {1}...", last, last + BATCH));
                    List<JiraIssue> issues = JiraServerFacade.Instance.getCustomFilterIssues(server, new Filter(project.Id), last, BATCH);
                    foreach (JiraIssue issue in issues) {
                        putIssueInDbIfNotPresent(connection, issue, projectId);
                    }
                    retrieved = issues.Count;
                    last += retrieved;
                    total += retrieved;
                } while (retrieved > 0);
                appendToCollectLog("done retrieving issues - got " + total + " issues");
            } catch (ThreadAbortException) {
                appendToCollectLog("Aborting thread...");
            } catch (Exception e) {
                appendToCollectLog("Exception while retrieving issues: " + e);
            } finally {
                appendToCollectLog("Closing connection to database...");
                connection.Close();
            }
        }

        private bool getProject(string projectKey, int serverId, out int projectId) {
            NpgsqlCommand command = new NpgsqlCommand(
                string.Format("select id from projects where key like '{0}' and server={1}", projectKey, serverId), connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            projectId = -1;
            bool projectExists = false;
            while (reader.Read()) {
                projectExists = true;
                projectId = reader.GetInt32(0);
            }
            reader.Close();
            return projectExists;
        }

        private bool getServer(string server, out int serverId) {
            NpgsqlCommand command = new NpgsqlCommand(
                string.Format("select id from servers where name like '{0}'", server), connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            serverId = -1;
            bool serverExists = false;
            while (reader.Read()) {
                serverExists = true;
                serverId = reader.GetInt32(0);
            }
            reader.Close();
            return serverExists;
        }

        private static void putIssueInDbIfNotPresent(NpgsqlConnection connection, JiraIssue issue, int projectId) {
            NpgsqlCommand command = new NpgsqlCommand(
                string.Format("select id from issues where key like '{0}' and project={1}", issue.Key, projectId), connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            bool issueExists = false;
            while (reader.Read()) {
                issueExists = true;
            }
            reader.Close();
            if (issueExists) return;

            string summary = issue.Summary ?? "";
            string description = issue.Description ?? "";
            command = new NpgsqlCommand(
                string.Format("insert into issues (key, project, summary, description) values ('{0}', {1}, '{2}', '{3}')",
                issue.Key, projectId, to64(summary), to64(description)), connection);
            command.ExecuteNonQuery();
        }

        static public string to64(string toEncode) {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        static public string from64(string toDecode) {
            byte[] fromBase64String = Convert.FromBase64String(toDecode);
            string returnValue = Encoding.UTF8.GetString(fromBase64String);
            return returnValue;
        }

        private class Filter : JiraFilter {
            private readonly int project;

            public Filter(int project) {
                this.project = project;
            }

            public string getFilterQueryString() {
                return "pid=" + project;
            }

            public string getSortBy() {
                return "updated";
            }
        }

        private const string STACK_TRACE_REGEXP = @"([a-zA-Z0-9_\.]*)\.([<>a-zA-Z0-9_\.]*)\([a-zA-Z0-9_\.]*(:([\d]*))*\)";

        private readonly Dictionary<string, int> hashes = new Dictionary<string, int>();

        private void buttonAnalyze_Click(object sender, EventArgs e) {
            try {
                textAnalyzeLog.Text = "";

                connection.Open();

                int serverId;
                if (!getServer(textAnalyzeServerName.Text, out serverId)) {
                    appendToAnalyzeLog("No such server in the database");
                    return;
                }
                int projectId;
                if (!getProject(textAnalyzeProject.Text, serverId, out projectId)) {
                    appendToAnalyzeLog("No such project");
                }

                hashes.Clear();

                NpgsqlCommand command = new NpgsqlCommand(string.Format("select description from issues where project={0}", projectId), connection);
                NpgsqlDataReader reader = command.ExecuteReader();
                int issuesCount = 0;
                int issuesWithDescription = 0;
                int issuesWithStackTrace = 0;
                while (reader.Read()) {
                    ++issuesCount;
                    string b64descr = reader.GetString(0);
                    string description = from64(b64descr);
                    if (string.IsNullOrEmpty(description)) continue;

                    ++issuesWithDescription;

                    Regex regex = new Regex(STACK_TRACE_REGEXP);
                    MatchCollection matches = regex.Matches(description);
                    if (matches.Count == 0) continue;
                    handleMatch(matches);

                    ++issuesWithStackTrace;
                }

                appendToAnalyzeLog("total issues:\t\t\t " + issuesCount);
                appendToAnalyzeLog("issues with description:\t\t " + issuesWithDescription);
                appendToAnalyzeLog(string.Format("issues with stack trace in description:\t {0} ({1:0.00}% {2:0.00}%)", 
                    issuesWithStackTrace, 
                    ((double) (100 * issuesWithStackTrace)) / issuesWithDescription, 
                    ((double) (100 * issuesWithStackTrace)) / issuesCount));

                mapReduce(issuesWithStackTrace, issuesWithDescription, issuesCount, textAnalyzeLog);

            } catch (Exception ex) {
                appendToAnalyzeLog(ex);
            } finally {
                appendToAnalyzeLog("Closing database connection");
                connection.Close();
            }
        }

        private void mapReduce(int issuesWithStackTrace, int issuesWithDescriptionOrComments, int issuesCount, TextBox log) {
            SortedDictionary<int, int> dupeCounts = new SortedDictionary<int, int>();
            foreach (int dupeCount in hashes.Values) {
                if (dupeCounts.ContainsKey(dupeCount)) {
                    ++dupeCounts[dupeCount];
                } else {
                    dupeCounts[dupeCount] = 1;
                }
            }

            appendToLogControl("", log);
            int totalDuplicates = 0;
            foreach (KeyValuePair<int, int> pair in dupeCounts) {
                if (pair.Key == 1) continue;
                appendToLogControl(string.Format("duplicates: {0} - count: {1}", pair.Key, pair.Value), log);
                totalDuplicates += pair.Value;
            }
            appendToLogControl("", log);
            appendToLogControl("total duplicates:\t\t\t\t\t " + totalDuplicates, log);
            appendToLogControl(
                string.Format("total duplicate percentage of stack issues with traces:\t {0:0.00}", ((double) 100 * totalDuplicates) / issuesWithStackTrace), log);
            appendToLogControl(
                string.Format("total duplicate percentage of issues with description:\t {0:0.00}", ((double)100 * totalDuplicates) / issuesWithDescriptionOrComments), log);
            appendToLogControl(
                string.Format("total duplicate percentage of all issues:\t\t\t {0:0.00}", ((double)100 * totalDuplicates) / issuesCount), log);
        }

        private void handleMatch(MatchCollection matches) {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Math.Min(matches.Count, 10); ++i) {
                Group @group = matches[i].Groups[0];
                sb.Append(group.Value).Append("\r\n");
            }
            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            string hashString = Convert.ToBase64String(hash);

            if (hashes.ContainsKey(hashString)) {
                ++hashes[hashString];
            } else {
                hashes[hashString] = 1;
            }
        }

        private void buttonAnalyzeFromDbGoClick(object sender, EventArgs e) {
            try {
                textAnalyzeLog.Text = "";

                int projectId = (int) numericProjectId.Value;

                bool justDescription = checkJustDescription.Checked;

                connJiraDb.Open();

                hashes.Clear();

                List<decimal> issues = new List<decimal>();

                NpgsqlCommand command = new NpgsqlCommand(string.Format("select id, pkey, description from jiraissue where project={0}", projectId), connJiraDb);
                NpgsqlDataReader reader = command.ExecuteReader();
                int issuesCount = 0;
                int issuesWithDescription = 0;
                int issuesWithStackTraceInDescription = 0;
                while (reader.Read()) {
                    ++issuesCount;
                    issues.Add(reader.GetDecimal(0));

                    object val = reader.GetValue(2);
                    string description = val != null && val is string ? reader.GetString(2) : "";
                    if (string.IsNullOrEmpty(description)) continue;

                    ++issuesWithDescription;

                    Regex regex = new Regex(STACK_TRACE_REGEXP);
                    MatchCollection matches = regex.Matches(description);
                    if (matches.Count == 0) continue;
                    handleMatch(matches);

                    ++issuesWithStackTraceInDescription;
                }

                reader.Close();
                command.Dispose();

                int issuesWithComments = 0;
                int issuesWihStackTraceInComments = 0;
                if (!justDescription) {
                    foreach (var issue in issues) {
                        bool haveStackTrace;
                        bool haveComment = handleComments(issue, out haveStackTrace);

                        if (haveComment) {
                            ++issuesWithComments;
                        }
                        if (haveStackTrace) {
                            ++issuesWihStackTraceInComments;
                        }
                    }
                }

                appendToAnalyzeDbLog("total issues:\t\t\t " + issuesCount);
                appendToAnalyzeDbLog("issues with description:\t\t " + issuesWithDescription);
                appendToAnalyzeDbLog("issues with comments:\t\t " + issuesWithComments);
                appendToAnalyzeDbLog(string.Format("issues with stack trace in description:\t {0} ({1:0.00}% {2:0.00}%)",
                    issuesWithStackTraceInDescription,
                    ((double)(100 * issuesWithStackTraceInDescription)) / issuesWithDescription,
                    ((double)(100 * issuesWithStackTraceInDescription)) / issuesCount));
                appendToAnalyzeDbLog(string.Format("issues with stack trace in comments:\t {0} ({1:0.00}% {2:0.00}%)",
                    issuesWihStackTraceInComments,
                    ((double)(100 * issuesWihStackTraceInComments)) / issuesWithComments,
                    ((double)(100 * issuesWihStackTraceInComments)) / issuesCount));

                mapReduce(issuesWithStackTraceInDescription + issuesWihStackTraceInComments, issuesWithDescription + issuesWithComments, issuesCount, textAnalyzeDbLog);

            } catch (Exception ex) {
                appendToAnalyzeDbLog(ex);
            } finally {
                appendToAnalyzeDbLog("Closing database connection");
                connJiraDb.Close();
            }

        }

        private bool handleComments(decimal issueId, out bool haveStackTrace) {
            NpgsqlCommand command = new NpgsqlCommand(string.Format("select actionbody from jiraaction where issueid={0}", issueId), connJiraDb);
            NpgsqlDataReader reader = command.ExecuteReader();
            bool haveComments = false;
            haveStackTrace = false;
            while (reader.Read()) {
                haveComments = true;
                string comment = reader.GetString(0);
                Regex regex = new Regex(STACK_TRACE_REGEXP);
                MatchCollection matches = regex.Matches(comment);
                if (matches.Count == 0) continue;
                haveStackTrace = true;
                handleMatch(matches);
            }
            reader.Close();
            return haveComments;
        }

        private void buttonClear_Click(object sender, EventArgs e) {
            textAnalyzeDbLog.Text = "";
        }
    }
}

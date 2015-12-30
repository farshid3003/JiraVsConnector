using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.store;
using Atlassian.plvs.ui.bamboo.treemodels;
using Atlassian.plvs.util;
using Atlassian.plvs.util.bamboo;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Shell.Interop;
using Process = System.Diagnostics.Process;
using EnvDTE;
using Thread = System.Threading.Thread;

namespace Atlassian.plvs.ui.bamboo {
    public partial class BuildDetailsPanel : UserControl {
        private readonly Solution solution;
        private readonly BambooBuild build;

        private BambooBuild buildWithDetails;
        private ICollection<BuildArtifact> buildArtifacts;

        private bool unableToRetrieveDetails;

//        private readonly TabPage myTab;
//        private readonly Action<TabPage> buttonCloseClicked;

        private BambooTestResultTree testResultTree;

        private const int MAX_PARSEABLE_LENGTH = 200;
        private const int A_LOT = 10000000;
        private const string OPENFILE_ON_LINE_URL = "openfileonline:";
        private const string SHOW_FAILED_TESTS_ONLY = "BambooBuildShowFailedTestsOnly";

        private const string RETRIEVING_DETAILS = "Retrieving...";
        private const string OPENISSUE_URL_TYPE = "openissue:";

        private readonly StatusLabel status;

        public BuildDetailsPanel(Solution solution, BambooBuild build, TabPage myTab, 
            BuildDetailsWindow parent) { //}, Action<TabPage> buttonCloseClicked) {
            
            this.solution = solution;
            this.build = build;
//            this.myTab = myTab;
//            this.buttonCloseClicked = buttonCloseClicked;
            
            InitializeComponent();

            parent.setMyTabIconFromBuildResult(build.Result, myTab);

            status = new StatusLabel(statusStrip, labelStatus);

            tabLogPanels.Visible = false;
            tabLogPanels.TabPages.Clear();

            webLog.Dock = DockStyle.Fill;
        }

        protected override void OnLoad(EventArgs e) {
            init();
        }

        private void init() {
            displaySummary();
            runGetBuildDetailsThread();
        }

        private void runGetBuildDetailsThread() {
            status.setInfo("Retrieving build details...");
            Thread t = PlvsUtils.createThread(getBuildDetailsRunner);
            t.Start();
        }

        private void getBuildDetailsRunner() {
            try {
                buildWithDetails = BambooServerFacade.Instance.getBuildByKey(build.Server, build.Key);
                buildArtifacts = BambooServerFacade.Instance.getArtifacts(build);
            } catch(Exception e) {
                status.setError("Failed to retrieve build details", e);
                unableToRetrieveDetails = true;
            }
            this.safeInvoke(new MethodInvoker(displaySummary));
            runGetTestsThread();
        }

        private void runGetLogThread() {
            status.setInfo("Retrieving build logs...");
            Thread t = PlvsUtils.createThread(getLogRunner);
            t.Start();
        }

        private void getLogRunner() {
            SortedDictionary<string, string> logs = new SortedDictionary<string, string>();
            List<Exception> exceptions = new List<Exception>();

            int unnamed = 1;
            foreach (BuildArtifact artifact in buildArtifacts) {
                if (!artifact.Name.Equals("Build log")) { continue; }
                try {
                    logs[artifact.ResultKey ?? ("Log #" + unnamed)] =
                        BambooServerFacade.Instance.getBuildLog(build.Server, artifact.Url);
                    if (artifact.ResultKey == null) {
                        ++unnamed;
                    }
                } catch (Exception e) {
                    exceptions.Add(e);
                }
            }

            this.safeInvoke(new MethodInvoker(delegate {
                                                  webLog.Visible = false;
                                                  tabLogPanels.Visible = true;
                                                  foreach (string key in logs.Keys) {
                                                      addLogTab(key, logs[key]);
                                                  }
                                                  tabLogPanels.Dock = DockStyle.Fill;
                                              }));
            if (exceptions.Count > 0) {
                status.setError("Failed to retrieve some of the build logs", exceptions);
                if (logs.Count == 0) {
                    this.safeInvoke(new MethodInvoker(delegate { webLog.DocumentText = ""; }));
                }
            } else {
                status.setInfo("Build log retrieved");
            } 
        }

        private void addLogTab(string key, string log) {
            TabPage page = new TabPage(key);
            WebBrowser txt = new WebBrowser
                             {
                                 WebBrowserShortcutsEnabled = false,
                                 IsWebBrowserContextMenuEnabled = false,
                                 ScriptErrorsSuppressed = true,
                                 Dock = DockStyle.Fill
                             };

            page.Controls.Add(txt);
            tabLogPanels.TabPages.Add(page);
            txt.DocumentText = getLogHtml(log, false, null);

            Thread t = PlvsUtils.createThread(() => fillLogTab(txt, key, log));
            t.Start();
        }

        private void fillLogTab(WebBrowser txt, string key, string log) {
            string augmentedLog = getLogHtml(log, true,
                percent => status.setInfo(string.Format("Processing build log: {0} - {1}%", key, percent)));

            this.safeInvoke(new MethodInvoker(delegate {
                                                  status.setInfo(string.Format("Build log {0} processed", key));
                                                  txt.DocumentCompleted += (s, e) => { txt.Navigating += logNavigating; };
                                                  txt.DocumentText = augmentedLog;
                                              }));
        }

        private string getLogHtml(string log, bool augmented, Action<int> progress) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>\n<head>\n").Append(Resources.summary_and_description_css);
            sb.Append("\n</head>\n<body class=\"description\">\n");
            sb.Append(augmented
                          ? createAugmentedLog(HttpUtility.HtmlEncode(log), progress)
                          : HttpUtility.HtmlEncode(log).Replace("\n", "<br>\n"));
            sb.Append("</body></html>");
            return sb.ToString();
        }

        private void runGetTestsThread() {
            status.setInfo("Retrieving build test results...");
            Thread t = PlvsUtils.createThread(getTestsRunner);
            t.Start();
        }

        private void getTestsRunner() {
            try {
                ICollection<BambooTest> tests = BambooServerFacade.Instance.getTestResults(build);
                if (tests == null || tests.Count == 0) {
                    this.safeInvoke(new MethodInvoker(delegate { toolStripContainerTests.Visible = false; }));
                } else {
                    this.safeInvoke(new MethodInvoker(() => createAndFillTestTree(tests)));
                }
                status.setInfo("Test results retrieved");
            } catch (Exception e) {
                status.setError("Failed to retrieve test results", e);
            }
            runGetLogThread();
        }

        private void createAndFillTestTree(ICollection<BambooTest> tests) {
            labelNoTestsFound.Visible = false;
            toolStripContainerTests.Dock = DockStyle.Fill;

            toolStripContainerTests.TopToolStripPanel.Controls.Add(testResultsToolStrip);
            testResultsToolStrip.Dock = DockStyle.None;
            testResultsToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            testResultsToolStrip.Items.AddRange(new ToolStripItem[] { buttonFailedOnly, buttonOpenTest, buttonRunTestInVs, buttonDebugTestInVs});
            testResultsToolStrip.Location = new System.Drawing.Point(3, 0);
            testResultsToolStrip.Size = new System.Drawing.Size(126, 25);
            testResultsToolStrip.TabIndex = 0;
            testResultsToolStrip.Visible = true;

            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            bool failedOnly = store.loadParameter(SHOW_FAILED_TESTS_ONLY, 0) > 0;

            testResultTree = new BambooTestResultTree {Model = new TestResultTreeModel(tests, failedOnly)};
            toolStripContainerTests.ContentPanel.Controls.Add(testResultTree);
            testResultTree.SelectionChanged += testResultTree_SelectionChanged;
            testResultTree.MouseDoubleClick += testResultTree_MouseDoubleClick;
            testResultTree.KeyPress += testResultTree_KeyPress;

            buttonFailedOnly.Checked = failedOnly;
            buttonFailedOnly.CheckedChanged += buttonFailedOnly_CheckedChanged;

            buttonOpenTest.Click += buttonOpenTest_Click;
            buttonRunTestInVs.Click += buttonRunTestInVs_Click;
            buttonDebugTestInVs.Click += buttonDebugTestInVs_Click;
            updateTestButtons();
            testResultTree.ExpandAll();
        }

        private void buttonDebugTestInVs_Click(object sender, EventArgs e) {
            TestMethodNode method = getSelectedTestMethod();
            if (method == null) return;
            if (navigateToTestClassAndMethod(method.Test)) {
                solution.DTE.ExecuteCommand("Test.DebugTestsInCurrentContext", "");
            }
        }

        private void buttonRunTestInVs_Click(object sender, EventArgs e) {
            TestMethodNode method = getSelectedTestMethod();
            if (method == null) return;
            if (navigateToTestClassAndMethod(method.Test)) {
                solution.DTE.ExecuteCommand("Test.RunTestsInCurrentContext", "");
            }
        }

        private void testResultTree_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (int) Keys.Enter) {
                openTestMethod(true);
            }
        }

        private void testResultTree_MouseDoubleClick(object sender, MouseEventArgs e) {
            openTestMethod(false);
        }

        private void buttonOpenTest_Click(object sender, EventArgs e) {
            openTestMethod(false);
        }

        private void openTestMethod(bool refocusOnTestList) {
            TestMethodNode method = getSelectedTestMethod();
            if (method == null) return;
            navigateToTestClassAndMethod(method.Test);
            if (!refocusOnTestList) return;

            // this seems to be the only way to refocus the bamboo toolwindow
            IVsWindowFrame windowFrame = (IVsWindowFrame) ToolWindowManager.Instance.BuildDetailsWindow.Frame;
            windowFrame.Show();
        }

        private TestMethodNode getSelectedTestMethod() {
            if (testResultTree == null || testResultTree.SelectedNode == null) return null;
            return testResultTree.SelectedNode.Tag as TestMethodNode;
        }

        private void buttonFailedOnly_CheckedChanged(object sender, EventArgs e) {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            store.storeParameter(SHOW_FAILED_TESTS_ONLY, buttonFailedOnly.Checked ? 1 : 0);
            TestResultTreeModel model = testResultTree.Model as TestResultTreeModel;
            if (model == null) return;
            model.FailedOnly = buttonFailedOnly.Checked;
            testResultTree.ExpandAll();
        }

        private void testResultTree_SelectionChanged(object sender, EventArgs e) {
            updateTestButtons();
        }

        private void updateTestButtons() {
            bool enabled = 
                testResultTree != null 
                && testResultTree.SelectedNode != null 
                && testResultTree.SelectedNode.Tag is TestMethodNode;
            buttonOpenTest.Enabled = enabled;
            buttonRunTestInVs.Enabled = enabled;
            buttonDebugTestInVs.Enabled = enabled;
        }

        private static readonly Regex FILE_AND_LINE = new Regex("(.*?)\\s(&quot;)?((([a-zA-Z]:)|(\\\\))?\\S+?)(&quot;)?\\s*\\(((\\d+)(,\\d+)?)\\)(.*?)"); 

        private string createAugmentedLog(string log, Action<int> progress) {

            SolutionUtils.refillAllSolutionProjectItems(solution);

            string[] strings = log.Split(new[] {'\n'});
            StringBuilder logSb = new StringBuilder();
            int prevPercent = 0;
            double i = 0;
            foreach (var s in strings) {
                if (progress != null) {
                    ++i;
                    int percent = (int)(i * 100 / strings.Length);
                    if (percent > prevPercent) {
                        prevPercent = percent;
                        progress(percent);
                    }
                }

                StringBuilder lineSb = new StringBuilder();
                bool tooLong = s.Length > MAX_PARSEABLE_LENGTH;
                string parseablePart = tooLong ? s.Substring(0, MAX_PARSEABLE_LENGTH) : s;
                string restPart = tooLong ? s.Substring(MAX_PARSEABLE_LENGTH) : "";

                MatchCollection matches = FILE_AND_LINE.Matches(parseablePart);
                if (matches.Count > 0) {
                    foreach (Match match in matches) {
                        string fileName = match.Groups[3].Value;
                        if (!fileName.EndsWith(".sln") && !fileName.EndsWith("proj") && SolutionUtils.solutionContainsFile(fileName, solution)) {
                            string lineNumber = match.Groups[8].Value.Trim();
                            lineSb.Append(match.Groups[1].Value)
                                .Append(" <a href=\"").Append(OPENFILE_ON_LINE_URL)
                                .Append(fileName).Append('@').Append(lineNumber).Append("\">")
                                .Append(fileName).Append('(').Append(lineNumber).Append(")</a>")
                                .Append(match.Groups[11]);
                        } else {
                            lineSb.Append(match.Groups[0]);
                        }
                    }
                    Match lastMatch = matches[matches.Count - 1];
                    lineSb.Append(parseablePart.Substring(lastMatch.Index + lastMatch.Length));
                } else {
                    lineSb.Append(parseablePart);
                }
                lineSb.Append(restPart);
                logSb.Append(colorLine(lineSb.ToString())).Append("<br>\r\n");
            }
            return logSb.ToString();
        }

        private static readonly Regex SUCCESS_REGEX = new Regex(@"(BUILD SUCCESSFUL)|(BUILD SUCCEEDED)|(\[INFO\])|(Build succeeded)");
        private static readonly Regex FAILURE_REGEX = new Regex(@"(Build FAILED)|(BUILD FAILED)|(\[ERROR\])|([Ee]rror \w+\d+)");
        private static readonly Regex WARNING_REGEX = new Regex(@"(\[WARNING\])|([Ww]arning \w+\d+)");

        private static string colorLine(string line) {
            string color = null;
            if (FAILURE_REGEX.IsMatch(line)) {
                color = BambooBuild.BuildResult.FAILED.GetColorValue();
            } else if (WARNING_REGEX.IsMatch(line)) {
                color = "#87721e";
            } else if (SUCCESS_REGEX.IsMatch(line)) {
                color = BambooBuild.BuildResult.SUCCESSFUL.GetColorValue();
            }
            return color != null ? "<span style=\"color:" + color + ";\">" + line + "</span>" : line;
        }

        private void displaySummary() {
            summaryLoaded = false;

            StringBuilder sb = new StringBuilder();

            BuildNode bn = new BuildNode(build);
            sb.Append("<html>\n<head>\n").Append(Resources.summary_and_description_css)
                .Append("\n</head>\n<body class=\"description\">\n")
                .Append(
                    string.Format(Resources.build_summary_html, 
                        build.Number, 
                        build.Key, 
                        bn.Reason, 
                        bn.Tests, 
                        build.RelativeTime, 
                        build.Duration, 
                        getServerHtml(), 
                        build.Result, 
                        build.Result.GetColorValue(),
                        getArtifactsHtml(),
                        getRelatedIssuesHtml()))
                .Append("\n</body>\n</html>\n");

            webSummary.DocumentText = sb.ToString();
        }

        private string getRelatedIssuesHtml() {
            if (buildWithDetails == null) return RETRIEVING_DETAILS;
            if (unableToRetrieveDetails) return getUnableToRetrieve();
            if (buildWithDetails.RelatedIssues == null || buildWithDetails.RelatedIssues.Count == 0) return "No related JIRA issues";
            StringBuilder sb = new StringBuilder();
            foreach (BambooBuild.RelatedIssue issue in buildWithDetails.RelatedIssues) {
                ICollection<JiraServer> jiraServers = JiraServerModel.Instance.getAllEnabledServers();
                string url = issue.Url;
                foreach (JiraServer jiraServer in jiraServers) {
                    if (issue.Url.StartsWith(jiraServer.Url)) {                        
                        url = OPENISSUE_URL_TYPE + issue.Key + "@" + jiraServer.GUID;
                        break;
                    }
                }
                sb.Append("<a href=\"").Append(url).Append("\">").Append(issue.Key).Append("</a>, ");
            }
            return sb.ToString(0, sb.Length - 2);
        }

        private string getArtifactsHtml() {
            if (buildWithDetails == null) return RETRIEVING_DETAILS;
            if (buildArtifacts == null || buildArtifacts.Count == 0) return "No build artifacts";
            StringBuilder sb = new StringBuilder();
            foreach (BuildArtifact artifact in buildArtifacts) {
                sb.Append("<a href=\"").Append(artifact.Url).Append("\">");
                if (artifact.ResultKey != null) {
                    sb.Append(artifact.ResultKey).Append(" - ");    
                }
                sb.Append(artifact.Name).Append("</a>, ");
            }
            return sb.ToString(0, sb.Length - 2);
        }

        private static string getUnableToRetrieve() {
            return "<span style=\"color:" + BambooBuild.BuildResult.FAILED.GetColorValue() + ";\">Unable to retrieve</span>";
        }

        private object getServerHtml() {
            return build.Server.Name + " (<a href=" + build.Server.Url + ">" + build.Server.Url + "</a>)"; 
        }

//        private void buttonClose_Click(object sender, EventArgs e) {
//            if (buttonCloseClicked != null) {
//                buttonCloseClicked(myTab);
//            }
//        }

        private void buttonRun_Click(object sender, EventArgs e) {
            string key = BambooBuildUtils.getPlanKey(build);
            status.setInfo("Adding build " + key + " to the build queue...");
            Thread t = PlvsUtils.createThread(() => runBuildWorker(key));
            t.Start();
        }

        private void runBuildWorker(string key) {
            try {
                BambooServerFacade.Instance.runBuild(build.Server, key);
                status.setInfo("Added build " + key + " to the build queue");
                UsageCollector.Instance.bumpBambooBuildsOpen();
            } catch (Exception ex) {
                status.setError("Failed to add build " + key + " to the build queue", ex);
            }
        }

        private void buttonViewInBrowser_Click(object sender, EventArgs e) {
            try {
                PlvsUtils.runBrowser(build.Server.Url + "/browse/" + build.Key);
            } catch (Exception ex) {
                Debug.WriteLine("buttonViewInBrowser_Click - exception: " + ex.Message);
            }
        }

        private bool summaryLoaded;

        private void webSummary_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (!summaryLoaded) return;

            e.Cancel = true;

            if (e.Url.Equals("about:blank")) return;

            if (e.Url.ToString().StartsWith(OPENISSUE_URL_TYPE)) {
                openIssue(e.Url.ToString().Substring(OPENISSUE_URL_TYPE.Length));
            } else {
                viewUrlInTheBrowser(e.Url.ToString());
            }
        }

        private static void viewUrlInTheBrowser(string url) {
            try {
                PlvsUtils.runBrowser(url);
            } catch (Exception ex) {
                Debug.WriteLine("viewUrlInTheBrowser - exception: " + ex.Message);
            }
        }

        private static void openIssue(string url) {
            string[] strings = url.Split(new[] { '@' });
            JiraServer jiraServer = JiraServerModel.Instance.getServer(new Guid(strings[1]));
            if (jiraServer == null) return;
            AtlassianPanel.Instance.Jira.findAndOpenIssue(strings[0], jiraServer, (s, m, e) => {
                                                                                      if (!s) viewUrlInTheBrowser(jiraServer.Url + "/browse/" + strings[0]);
                                                                                  });
        }

        private void webSummary_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            summaryLoaded = true;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (!tabControl.SelectedTab.Equals(tabLog)) return;
            webLog.DocumentText = PlvsUtils.getThrobberHtml(PlvsUtils.getThroberPath(), "Fetching build log...");
        }

        private void logNavigating(object sender, WebBrowserNavigatingEventArgs e) {

            e.Cancel = true;
            
            if (e.Url.Equals("about:blank")) return;

            string url = e.Url.ToString();
            if (url.StartsWith(OPENFILE_ON_LINE_URL)) {
                string file = url.Substring(OPENFILE_ON_LINE_URL.Length, url.LastIndexOf('@') - OPENFILE_ON_LINE_URL.Length);

                string lineNoStr = url.Substring(url.LastIndexOf('@') + 1);

                SolutionUtils.openSolutionFile(file, lineNoStr, solution);
            }
        }

        private bool navigateToTestClassAndMethod(BambooTest test) {
            string fileName = null, lineNo = null;
            foreach (Project project in solution.Projects) {
                if (examineProjectItems(project.ProjectItems, test.ClassName, test.MethodName, ref fileName, ref lineNo)) {
                    return SolutionUtils.openSolutionFile(fileName, lineNo, solution);
                }
            }
            return false;
        }

        private static bool examineProjectItems(ProjectItems projectItems, string classFqdn, string methodName, ref string fileName, ref string lineNo) {
            if (projectItems == null || projectItems.Count == 0) return false;
            foreach (ProjectItem item in projectItems) {
                if (examineOneItem(item, classFqdn, methodName, ref fileName, ref lineNo)) return true;
                if (examineProjectItems(item.ProjectItems, classFqdn, methodName, ref fileName, ref lineNo)) return true;
            }
            return false;
        }

        private static bool examineOneItem(ProjectItem item, string classFqdn, string methodName, ref string fileName, ref string lineNo) {
            FileCodeModel codeModel = item.FileCodeModel;
            if (codeModel == null || codeModel.CodeElements == null) return false;

            foreach (CodeElement element in codeModel.CodeElements) {
                if (examineClass(item, element, classFqdn, methodName, ref fileName, ref lineNo)) return true;
                if (examineNamespace(item, element, classFqdn, methodName, ref fileName, ref lineNo)) return true;
            }
            return false;
        }

        private static bool examineNamespace(ProjectItem item, CodeElement element, string classFqdn, string methodName, ref string fileName, ref string lineNo) {
            if (element.Kind != vsCMElement.vsCMElementNamespace) return false;

            foreach (CodeElement childElement in element.Children) {
                if (examineClass(item, childElement, classFqdn, methodName, ref fileName, ref lineNo)) return true;
                if (examineNamespace(item, childElement, classFqdn, methodName, ref fileName, ref lineNo)) return true;
            }
            return false;
        }

        private static bool examineClass(ProjectItem item, CodeElement element, string classFqdn, string methodName, ref string fileName, ref string lineNo) {
            if (element.Kind != vsCMElement.vsCMElementClass || !element.FullName.Equals(classFqdn)) return false;
            fileName = item.Name;
            foreach (CodeElement grandChildElement in element.Children) {
                if (grandChildElement.Kind != vsCMElement.vsCMElementFunction || !grandChildElement.Name.Equals(methodName)) continue;

                lineNo = "" + grandChildElement.StartPoint.Line + "," + grandChildElement.StartPoint.LineCharOffset;
                return true;
            }
            return false;
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.util;
using Atlassian.plvs.util.jira;
using Atlassian.plvs.windows;

namespace Atlassian.plvs.dialogs.jira {
    public sealed partial class SearchIssue : Form {
        public JiraServer Server { get; set; }
        public JiraIssueListModel Model { get; set; }
        public StatusLabel Status { get; set; }

        public SearchIssue(JiraServer server, JiraIssueListModel model, StatusLabel status) {
            Server = server;
            Model = model;
            Status = status;
            InitializeComponent();

            Text = "Search issue on server \"" + server.Name + "\"";

            buttonOk.Enabled = false;
            StartPosition = FormStartPosition.CenterParent;
        }

        private void textQueryString_TextChanged(object sender, EventArgs e) {
            buttonOk.Enabled = textQueryString.Text.Length > 0;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            executeSearchAndClose();
        }

        private void textQueryString_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Enter) return;
            executeSearchAndClose();
        }

        private void fetchAndOpenIssue(string key) {
            textQueryString.Enabled = false;
            buttonOk.Enabled = false;
            buttonCancel.Enabled = false;
            AtlassianPanel.Instance.Jira.findAndOpenIssue(key, findFinished);
        }

        private void findFinished(bool success, string message, Exception e) {
            if (!success) {
                PlvsUtils.showError(message, e);
            }
            Close();
        }

        private void executeSearchAndClose() {
            string query = textQueryString.Text.Trim();
            if (query.Length == 0) return;

            if (JiraIssueUtils.ISSUE_REGEX.IsMatch(query.ToUpper())) {
                JiraIssue foundIssue = Model.Issues.FirstOrDefault(issue => issue.Key.Equals(query) && issue.Server.Url.Equals(Server.Url));

                if (foundIssue == null) {
                    string key = query.ToUpper();
                    fetchAndOpenIssue(key);
                    return;
                }
                IssueDetailsWindow.Instance.openIssue(foundIssue, AtlassianPanel.Instance.Jira.ActiveIssueManager);
            }
            else {
                string url = Server.Url + "/secure/QuickSearch.jspa?searchString=" + HttpUtility.UrlEncode(query);
                PlvsUtils.runBrowser(url);
            }
            Close();
        }

        private void searchIssueKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape && buttonCancel.Enabled) {
                Close();
            }
        }
    }
}
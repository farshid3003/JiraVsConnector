using System.Windows.Forms;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.windows;

namespace Atlassian.plvs.scm {
    public partial class AnkhSvnJiraActiveIssueControl : UserControl {
        private readonly bool enabled;

        private const string NO_ISSUE_SELECTED = JiraActiveIssueManager.NO_ISSUE_ACTIVE;
        private const string NO_INTEGRATION = "Atlassian Connector Integration Disabled";

        public AnkhSvnJiraActiveIssueControl(bool enabled) {
            this.enabled = enabled;
            InitializeComponent();

//            AtlassianPanel.Instance.Jira.SelectedIssueChanged += selectedIssueChanged;
            AtlassianPanel.Instance.Jira.ActiveIssueManager.ActiveIssueChanged += activeIssueManagerActiveIssueChanged;
            labelJira.Text = enabled ? getCommentText() : NO_INTEGRATION;
        }

        void activeIssueManagerActiveIssueChanged(object sender, System.EventArgs e) {
            labelJira.Text = enabled ? getCommentText() : NO_INTEGRATION;
        }

        private void selectedIssueChanged(object sender, TabJira.SelectedIssueEventArgs e) {
            labelJira.Text = enabled ? getCommentText() : NO_INTEGRATION;
        }

        private static string getCommentText() {
            JiraActiveIssueManager.ActiveIssue issue = AtlassianPanel.Instance.Jira.ActiveIssueManager.CurrentActiveIssue;
            if (issue == null || !issue.Enabled) {
                return NO_ISSUE_SELECTED;
            }
            return "Commit message is set to: \"" + issue.Key + (issue.Summary != null ? " - " + issue.Summary : "") + "\"";
        }
    }
}

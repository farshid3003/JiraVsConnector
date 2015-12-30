using System;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;

namespace Atlassian.plvs.dialogs.jira {
    public partial class NewIssueComment : Form {

        public NewIssueComment(JiraIssue issue, AbstractJiraServerFacade facade) {
            InitializeComponent();
            buttonOk.Enabled = false;

            textComment.Facade = facade;
            textComment.Issue = issue;

            StartPosition = FormStartPosition.CenterParent;
        }

        public string CommentBody {
            get { return textComment.Text; }
        }

        private void newIssueCommentKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Escape) return;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void textComment_MarkupTextChanged(object sender, EventArgs e) {
            buttonOk.Enabled = textComment.Text.Trim().Length > 0;
        }
    }
}
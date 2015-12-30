using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.ui.jira {
    public class JiraAttachmentListViewItem : ListViewItem {

        private readonly JiraIssue issue;
        public JiraAttachment Attachment { get; private set; }

        public string Url { get { return issue.Server.Url + "/" + Attachment.RelativeUrl; } }

        public JiraAttachmentListViewItem(JiraIssue issue, JiraAttachment att) 
            : base(new [] { att.Name, att.Author, att.Size.ToString(), JiraIssueUtils.getShortDateStringFromDateTime(issue.ServerLanguage, att.Created) }) {

            this.issue = issue;
            Attachment = att;
        }
    }
}

using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public abstract class TreeNodeWithJiraServer : TreeNode {
        protected TreeNodeWithJiraServer(string name, int imageIdx) : base(name, imageIdx, imageIdx) {}
        public abstract JiraServer Server { get; set; }
    }
}
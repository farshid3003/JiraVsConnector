using System.Windows.Forms;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public class RecentlyOpenIssuesTreeNode : TreeNode {
        public RecentlyOpenIssuesTreeNode(int imageIdx) : base("Recently Open Issues", imageIdx, imageIdx) {}
    }
}
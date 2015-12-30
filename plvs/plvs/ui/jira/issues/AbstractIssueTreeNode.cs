using System.Drawing;

namespace Atlassian.plvs.ui.jira.issues {
    public abstract class AbstractIssueTreeNode {
        public abstract Image Icon { get; }
        public abstract string Name { get; }
    }
}
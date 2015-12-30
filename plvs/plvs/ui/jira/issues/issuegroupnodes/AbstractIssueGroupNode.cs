using System.Collections.Generic;

namespace Atlassian.plvs.ui.jira.issues.issuegroupnodes {
    public abstract class AbstractIssueGroupNode
        : AbstractIssueTreeNode, TreeNodeCollapseExpandStatusManager.TreeNodeRememberingCollapseState {
        protected AbstractIssueGroupNode() {
            IssueNodes = new List<IssueNode>();
        }

        public override string Name {
            get { return getGroupName() + " (" + IssueNodes.Count + ")"; }
        }

        public abstract string getGroupName();

        public List<IssueNode> IssueNodes { get; private set; }

        public string NodeKey {
            get { return getGroupName().Replace(" ", "_"); }
        }

        public bool NodeExpanded { get; set; }
    }
}
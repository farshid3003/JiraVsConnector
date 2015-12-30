using System.Drawing;

namespace Atlassian.plvs.ui.jira.issues.issuegroupnodes {
    class ByLastUpdatedIssueGroupNode : AbstractIssueGroupNode {
        private readonly string name;

        public ByLastUpdatedIssueGroupNode(string name) {
            this.name = name;
        }

        #region Overrides of AbstractIssueGroupNode

        public override Image Icon {
            get { return null; }
        }

        public override string getGroupName() {
            return name;
        }

        #endregion
    }
}
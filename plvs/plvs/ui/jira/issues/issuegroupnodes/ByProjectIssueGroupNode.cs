using System.Drawing;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issues.issuegroupnodes {

    class ByProjectIssueGroupNode : AbstractIssueGroupNode {
        private readonly JiraProject project;

        public ByProjectIssueGroupNode(JiraProject project) {
            this.project = project;
        }

        #region Overrides of AbstractIssueGroupNode

        public override Image Icon {
            get { return null; }
        }

        public override string getGroupName() {
            return project.Key + ": " + project.Name;
        }

        #endregion
    }
}
using System.Drawing;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.jira.issues.issuegroupnodes {
    abstract class AbstractByNamedEntityIssueGroupNode: AbstractIssueGroupNode {
        private readonly JiraServer server;
        private readonly JiraNamedEntity entity;

        protected AbstractByNamedEntityIssueGroupNode(JiraServer server, JiraNamedEntity entity) {
            this.server = server;
            this.entity = entity;
        }

        #region Overrides of AbstractIssueGroupNode

        public override Image Icon {
            get { return ImageCache.Instance.getImage(server, entity.IconUrl).Img; }
        }

        public override string getGroupName() {
            return entity.Name.Replace("&", "&&");
        }

        #endregion

    }
}
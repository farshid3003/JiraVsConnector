using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public abstract class JiraFilterGroupTreeNode : TreeNodeWithJiraServer, TreeNodeCollapseExpandStatusManager.TreeNodeRememberingCollapseState {
        private readonly JiraServer server;

        protected JiraFilterGroupTreeNode(JiraServer server, string name, int imageIdx) : base(name, imageIdx) {
            this.server = server;
        }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }

        public abstract string NodeKey { get; }

        public bool NodeExpanded {
            get { return IsExpanded; }
            set { if (value) Expand(); else Collapse(); }
        }
    }
}
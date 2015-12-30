using System;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public class JiraServerTreeNode : TreeNodeWithJiraServer, TreeNodeCollapseExpandStatusManager.TreeNodeRememberingCollapseState, IDisposable {
        private readonly JiraServerModel model;
        private JiraServer server;

        public JiraServerTreeNode(JiraServerModel model, JiraServer server, int imageIdx)
            : base(PlvsUtils.getServerNodeName(model, server), imageIdx) {

            this.model = model;
            this.server = server;
            model.DefaultServerChanged += model_DefaultServerChanged;
        }

        void model_DefaultServerChanged(object sender, EventArgs e) {
            Text = PlvsUtils.getServerNodeName(model, server);
        }

        public override JiraServer Server {
            get { return server; }
            set {
                server = value;
                Text = PlvsUtils.getServerNodeName(model, server);
            }
        }

        public string NodeKey { get { return "JIRA_Server_Node_" + Server.GUID; } }

        public bool NodeExpanded { 
            get { return IsExpanded; }
            set { if (value) Expand(); else Collapse(true); }
        }

        public void Dispose() {
            model.DefaultServerChanged -= model_DefaultServerChanged;
        }
    }
}
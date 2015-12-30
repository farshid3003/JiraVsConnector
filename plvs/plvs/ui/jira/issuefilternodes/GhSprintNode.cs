using System;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.gh;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public sealed class GhSprintTreeNode : TreeNodeWithJiraServer {
        private readonly JiraServer server;

        public GhSprintTreeNode(JiraServer server, Sprint sprint, int imageIdx)
            : base(sprint.Name, imageIdx) {

            this.server = server;
            this.Sprint = sprint;
        }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }

        public Sprint Sprint { get; private set; }
    }
}
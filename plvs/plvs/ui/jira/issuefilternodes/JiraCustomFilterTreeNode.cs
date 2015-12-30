using System;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public class JiraCustomFilterTreeNode : TreeNodeWithJiraServer {
        private readonly JiraServer server;

        public JiraCustomFilterTreeNode(JiraServer server, JiraCustomFilter filter, int imageIdx) : base(filter.Name, imageIdx) {
            this.server = server;
            Filter = filter;
            Tag = filter;
        }

        public JiraCustomFilter Filter { get; private set; }

        public void setFilterName(string newName) {
            Text = newName;
        }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }
    }
}
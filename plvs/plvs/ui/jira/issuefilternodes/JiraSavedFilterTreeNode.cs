using System;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public sealed class JiraSavedFilterTreeNode : TreeNodeWithJiraServer {
        private readonly JiraServer server;

        public JiraSavedFilterTreeNode(JiraServer server, JiraSavedFilter filter, int imageIdx, ContextMenuStrip menu)
            : base(filter.Name, imageIdx) {

            this.server = server;
            Filter = filter;
            Tag = filter.Name;
            ContextMenuStrip = menu;
        }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }

        public JiraSavedFilter Filter { get; private set; }
    }
}
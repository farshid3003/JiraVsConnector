using System;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs.jira {
    public sealed class TestJiraConnection : AbstractTestConnection {
        private readonly AbstractJiraServerFacade facade;
        private readonly JiraServer server;

        public TestJiraConnection(AbstractJiraServerFacade facade, JiraServer server) : base(server) {
            this.facade = facade;
            this.server = server;
        }

        public override void testConnection() {
            string result = "Connection to server successful";
            Exception ex = null;
            try {
                facade.login(server);
            } catch (Exception e) {
                ex = e; 
                result = "Failed to connect to to server";
            }
            this.safeInvoke(new MethodInvoker(() => stopTest(result, ex)));
        }
    }
}
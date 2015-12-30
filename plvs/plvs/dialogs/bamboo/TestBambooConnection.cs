using System;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs.bamboo {
    class TestBambooConnection : AbstractTestConnection {
        private readonly BambooServerFacade facade;
        private readonly BambooServer server;

        public TestBambooConnection(BambooServerFacade facade, BambooServer server) : base(server) {
            this.facade = facade;
            this.server = server;
        }

        public override void testConnection() {
            var result = "Connection to server successful";
            Exception ex = null;
            try {
                facade.login(server);
            } catch (Exception e) {
                result = "Failed to connec to to server";
                ex = e;
            }
            this.safeInvoke(new MethodInvoker(() => stopTest(result, ex)));
        }
    }
}

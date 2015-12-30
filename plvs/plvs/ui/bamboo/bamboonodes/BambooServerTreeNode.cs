using System;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.bamboo.bamboonodes {
    public class BambooServerTreeNode : TreeNodeWithBambooServer, IDisposable {
        private readonly BambooServerModel model;
        private BambooServer server;

        public BambooServerTreeNode(BambooServerModel model, BambooServer server, int imageIdx)
            : base(PlvsUtils.getServerNodeName(model, server), imageIdx) {

            this.model = model;
            this.server = server;
            model.DefaultServerChanged += model_DefaultServerChanged;
        }

        void model_DefaultServerChanged(object sender, EventArgs e) {
            Text = PlvsUtils.getServerNodeName(model, server);
        }

        public override BambooServer Server {
            get { return server; }
            set {
                server = value;
                Text = PlvsUtils.getServerNodeName(model, server);
            }
        }

        public void Dispose() {
            model.DefaultServerChanged -= model_DefaultServerChanged;
        }
    }
}
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.ui.bamboo.bamboonodes {
    public abstract class TreeNodeWithBambooServer : TreeNode {
        protected TreeNodeWithBambooServer(string name, int imageIdx) : base(name, imageIdx, imageIdx) { }
        public abstract BambooServer Server { get; set; }
    }
}
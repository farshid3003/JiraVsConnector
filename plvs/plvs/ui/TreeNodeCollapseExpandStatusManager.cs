using Atlassian.plvs.store;

namespace Atlassian.plvs.ui {
    public class TreeNodeCollapseExpandStatusManager {
        private readonly ParameterStore store;
        private const string COLLAPSE_EXPAND_STATE = "TreeNodeExpandCollapseState_";

        public interface TreeNodeRememberingCollapseState {
            string NodeKey { get; }
            bool NodeExpanded { get; set; }
        }

        public TreeNodeCollapseExpandStatusManager(ParameterStore store) {
            this.store = store;
        }

        public bool restoreNodeState(object n) {
            TreeNodeRememberingCollapseState node = n as TreeNodeRememberingCollapseState;
            if (node == null) return true;

            int value = store.loadParameter(COLLAPSE_EXPAND_STATE + node.NodeKey, 1);
            node.NodeExpanded = value > 0;
            return node.NodeExpanded;
        }

        public void rememberNodeState(object n) {
            TreeNodeRememberingCollapseState node = n as TreeNodeRememberingCollapseState;
            if (node == null) return;

            store.storeParameter(COLLAPSE_EXPAND_STATE + node.NodeKey, node.NodeExpanded ? 1 : 0);
        }
    }
}

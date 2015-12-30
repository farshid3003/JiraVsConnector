namespace Atlassian.plvs.ui.bamboo {
    public class ProjectNode : TreeNodeCollapseExpandStatusManager.TreeNodeRememberingCollapseState {
        private readonly string guid;
        public string Key { get; private set; }
        public string ProjectKey { get; private set; }

        public ProjectNode(string guid, string key, string projectKey) {
            this.guid = guid;
            Key = key;
            ProjectKey = projectKey;
        }

        public string NodeKey { get { return guid + ProjectKey; } }
        public bool NodeExpanded { get; set; }
    }
}

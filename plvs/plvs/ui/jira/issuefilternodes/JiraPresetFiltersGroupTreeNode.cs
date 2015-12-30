using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public class JiraPresetFiltersGroupTreeNode : JiraFilterGroupTreeNode {
        public JiraPresetFiltersGroupTreeNode(JiraServer server, int imageIdx) : base(server, "Preset Filters", imageIdx) {
            Tag = "Right-click to set or clear project";
        }

        public JiraProject Project { get; set; }

        public override string NodeKey {
            get { return "JIRA_PresetFilters_Node_" + Server.GUID; }
        }
    }
}
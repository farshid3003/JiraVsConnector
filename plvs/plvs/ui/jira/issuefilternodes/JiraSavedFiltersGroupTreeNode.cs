using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public class JiraSavedFiltersGroupTreeNode : JiraFilterGroupTreeNode {
        public JiraSavedFiltersGroupTreeNode(JiraServer server, int imageIdx) : base(server, "Favourite Filters", imageIdx) {}

        public override string NodeKey {
            get { return "JIRA_SavedFilters_Node_" + Server.GUID; }
        }
    }
}
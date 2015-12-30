using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public class JiraCustomFiltersGroupTreeNode : JiraFilterGroupTreeNode {
        public JiraCustomFiltersGroupTreeNode(JiraServer server, int imageIdx)
            : base(server, "Local Filters", imageIdx) {
            Tag = "Right-click to add filter";
        }

        public override string NodeKey {
            get { return "JIRA_CustomFilters_Node_" + Server.GUID; }
        }
    }
}
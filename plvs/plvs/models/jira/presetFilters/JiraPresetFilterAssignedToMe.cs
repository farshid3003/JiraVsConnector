using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.presetFilters {
    public class JiraPresetFilterAssignedToMe : JiraPresetFilter {
        public JiraPresetFilterAssignedToMe(JiraServer server) : base(server, "Assigned To Me") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "assigneeSelect=issue_current_user&resolution=-1";
        }

        public override string getJqlNoProject() {
            return "assignee = currentUser() and resolution = Unresolved";
        }

        public override string getSortBy() {
            return "priority";
        }

        #endregion
    }
}
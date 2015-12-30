using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.presetFilters {
    public class JiraPresetFilterReportedByMe : JiraPresetFilter {
        public JiraPresetFilterReportedByMe(JiraServer server) : base(server, "Reported By Me") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "reporterSelect=issue_current_user";
        }

        public override string getJqlNoProject() {
            return "reporter = currentUser()";
        }

        public override string getSortBy() {
            return "updated";
        }

        #endregion
    }
}
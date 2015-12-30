using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.presetFilters {
    public class JiraPresetFilterRecentlyResolved : JiraPresetFilter {
        public JiraPresetFilterRecentlyResolved(JiraServer server) : base(server, "Resolved Recently") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "status=5&status=6&updated:previous=-1w";
        }

        public override string getJqlNoProject() {
            return "status = Resolved or status = Closed and updated < -1w";
        }

        public override string getSortBy() {
            return "updated";
        }

        #endregion
    }
}
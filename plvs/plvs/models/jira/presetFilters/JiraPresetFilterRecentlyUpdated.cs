using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.presetFilters {
    public class JiraPresetFilterRecentlyUpdated : JiraPresetFilter {
        public JiraPresetFilterRecentlyUpdated(JiraServer server) : base(server, "Updated Recently") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "updated:previous=-1w";
        }

        public override string getJqlNoProject() {
            return "updated < -1w";
        }

        public override string getSortBy() {
            return "updated";
        }

        #endregion
    }
}
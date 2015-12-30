using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.presetFilters {
    public class JiraPresetFilterRecentlyAdded: JiraPresetFilter {
        public JiraPresetFilterRecentlyAdded(JiraServer server) : base(server, "Added Recently") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "created:previous=-1w";
        }

        public override string getJqlNoProject() {
            return "created < -1w";
        }

        public override string getSortBy() {
            return "created";
        }

        #endregion
    }
}
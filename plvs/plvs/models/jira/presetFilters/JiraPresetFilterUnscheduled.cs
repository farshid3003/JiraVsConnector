using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.presetFilters {
    public class JiraPresetFilterUnscheduled : JiraPresetFilter {
        public JiraPresetFilterUnscheduled(JiraServer server) : base(server, "Unscheduled") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "resolution=-1&fixfor=-1";
        }

        public override string getJqlNoProject() {
            return "resolution = Unresolved and fixVersion is EMPTY";
        }

        public override string getSortBy() {
            return "priority";
        }

        #endregion
    }
}
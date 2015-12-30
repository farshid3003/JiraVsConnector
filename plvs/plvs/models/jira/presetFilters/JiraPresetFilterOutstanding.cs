using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.presetFilters {
    public class JiraPresetFilterOutstanding : JiraPresetFilter {
        public JiraPresetFilterOutstanding(JiraServer server) : base(server, "Outstanding") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "resolution=-1";
        }

        public override string getJqlNoProject() {
            return "resolution = Unresolved";
        }

        public override string getSortBy() {
            return "updated";
        }

        #endregion
    }
}
using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.presetFilters {
    public class JiraPresetFilterMostImportant : JiraPresetFilter {
        public JiraPresetFilterMostImportant(JiraServer server) : base(server, "Most Important") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "resolution=-1";
        }

        public override string getJqlNoProject() {
            return "resolution = Unresolved";
        }

        public override string getSortBy() {
            return "priority";
        }

        #endregion
    }
}
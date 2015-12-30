using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Atlassian.plvs.markers.vs2010.menu {
    internal class JiraIssueActionsSmartTag : SmartTag {
        public JiraIssueActionsSmartTag(ReadOnlyCollection<SmartTagActionSet> actionSets) : base(SmartTagType.Factoid, actionSets) { }
    }
}



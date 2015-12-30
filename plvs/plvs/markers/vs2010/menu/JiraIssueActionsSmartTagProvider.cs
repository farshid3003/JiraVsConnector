using System.ComponentModel.Composition;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.menu {
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [Order(Before = "default")]
    [TagType(typeof(SmartTag))]
    internal class JiraIssueActionsSmartTaggerProvider : IViewTaggerProvider {
        [Import(typeof(ITextStructureNavigatorSelectorService))]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        internal IClassifierAggregatorService AggregatorService;

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
            if (GlobalSettings.AllIssueLinksDisabled) {
                return null;
            }

#if NO_TAGGING
            return null;
#else

            if (AtlassianPanel.Instance == null || AtlassianPanel.Instance.Jira == null) {
                return null;
            }

            if (buffer == null || textView == null) {
                return null;
            }

            //make sure we are tagging only the top buffer
            if (buffer == textView.TextBuffer) {
                return new JiraIssueActionsSmartTagger(textView, AggregatorService.GetClassifier(buffer)) as ITagger<T>;
            }
            return null;
#endif
        }
    }
}

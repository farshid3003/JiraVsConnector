using System.ComponentModel.Composition;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.marginglyph {
    internal class JiraIssueLineGlyphTagProvider {
        [Export(typeof(ITaggerProvider))]
        [ContentType("code")]
        [TagType(typeof(JiraIssueLineGlyphTag))]
        internal class JiraIssueLineGlyphTaggerProvider : ITaggerProvider {
            [Import]
            internal IClassifierAggregatorService AggregatorService;

            public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
                if (GlobalSettings.AllIssueLinksDisabled) {
                    return null;
                }

#if NO_TAGGING
                return null;
#else
                if (AtlassianPanel.Instance == null || AtlassianPanel.Instance.Jira == null) {
                    return null;
                }

                if (buffer == null) {
                    return null;
                }

                return new JiraIssueLineGlyphTagger(buffer, AggregatorService.GetClassifier(buffer)) as ITagger<T>;
#endif
            }
        }
    }
}
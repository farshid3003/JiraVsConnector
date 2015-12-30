using System.ComponentModel.Composition;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.texttag {
    internal class JiraIssueTagProvider {
        [Export(typeof(IViewTaggerProvider))]
        [ContentType("code")]
        [TagType(typeof(JiraIssueTextTag))]
        internal class JiraIssueTaggerProvider : IViewTaggerProvider {

            [Import]
            internal IClassificationTypeRegistryService ClassificationRegistry = null;

            [Export(typeof(ClassificationTypeDefinition))]
            [Name("JiraIssueUnderlineClassification")]
            internal static ClassificationTypeDefinition underlineClassificationType = null;

            [Import]
            internal IClassifierAggregatorService AggregatorService;

            public static IClassificationType UnderlineClassification { get; private set; }

            public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
                if (GlobalSettings.AllIssueLinksDisabled) {
                    return null;
                }

#if NO_TAGGING
                return null;
#else

                if (UnderlineClassification == null)
                    UnderlineClassification = ClassificationRegistry.GetClassificationType("JiraIssueUnderlineClassification");

                if (AtlassianPanel.Instance == null || AtlassianPanel.Instance.Jira == null) {
                    return null;
                }

                if (buffer == null) {
                    return null;
                }

                return new JiraIssueTextTagger(buffer, AggregatorService.GetClassifier(buffer)) as ITagger<T>;
#endif
            }
        }
    }
}
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Atlassian.plvs.markers.vs2010.texttag {
    internal class JiraIssueTextTagger : LineTagger<JiraIssueTextTag> {
        public const string TEXT_TAGGER_CACHE_PROPERTY_NAME = "PlvsJiraIssueTextTaggerTagCache";

        public JiraIssueTextTagger(ITextBuffer buffer, IClassifier classifier) : base(buffer, classifier, TEXT_TAGGER_CACHE_PROPERTY_NAME) { }

        protected override TagSpan<JiraIssueTextTag> getTagForKey(SnapshotSpan span, string issueKey, int lastLine) {
            return new TagSpan<JiraIssueTextTag>(span, new JiraIssueTextTag(span, issueKey));
        }
    }
}



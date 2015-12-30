using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Atlassian.plvs.markers.vs2010.marginglyph {
    internal class JiraIssueLineGlyphTagger : LineTagger<JiraIssueLineGlyphTag> {

        private JiraIssueLineGlyphTag lastTag;
        public const string GLYPH_TAGGER_CACHE = "PlvsJiraIssueLineGlyphTaggerTagCache";

        public JiraIssueLineGlyphTagger(ITextBuffer buffer, IClassifier classifier) : base(buffer, classifier, GLYPH_TAGGER_CACHE) { }

        protected override TagSpan<JiraIssueLineGlyphTag> getTagForKey(SnapshotSpan span, string issueKey, int lastLine) {
            int currentLine = span.Start.GetContainingLine().LineNumber;
            if (lastLine == currentLine) {
                if (lastTag != null) {
                    lastTag.IssueKeys.Add(issueKey);
                }
                return null;
            }
            lastTag = new JiraIssueLineGlyphTag(span, new List<string> { issueKey });
            return new TagSpan<JiraIssueLineGlyphTag>(span, lastTag);
        }
    }
}



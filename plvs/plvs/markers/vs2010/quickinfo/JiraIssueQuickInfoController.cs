using System.Collections.Generic;
using System.Linq;
using Atlassian.plvs.markers.vs2010.texttag;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Atlassian.plvs.markers.vs2010.quickinfo {
    internal class JiraIssueQuickInfoController : IIntellisenseController {
        private ITextView textView;
        private readonly IList<ITextBuffer> subjectBuffers;
        private readonly JiraIssueQuickInfoControllerProvider provider;

        internal JiraIssueQuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, JiraIssueQuickInfoControllerProvider provider) {
            this.textView = textView;
            this.subjectBuffers = subjectBuffers;
            this.provider = provider;

            this.textView.MouseHover += OnTextViewMouseHover;
        }

        private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e) {
            SnapshotPoint? point = textView.BufferGraph.MapDownToFirstMatch(
                new SnapshotPoint(textView.TextSnapshot, e.Position),
                PointTrackingMode.Positive,
                snapshot => subjectBuffers.Contains(snapshot.TextBuffer),
                PositionAffinity.Predecessor);

            if (point == null) return;

            if (!textView.TextBuffer.Properties.ContainsProperty(JiraIssueTextTagger.TEXT_TAGGER_CACHE_PROPERTY_NAME)) {
                return;
            }
            IEnumerable<TagCache.TagEntry> tags = textView.TextBuffer.Properties.GetProperty(JiraIssueTextTagger.TEXT_TAGGER_CACHE_PROPERTY_NAME) as IEnumerable<TagCache.TagEntry>;
            if (tags == null) return;
            TagCache.TagEntry tagUnderCursor = tags.FirstOrDefault(tag => tag.Start <= point.Value.Position && tag.End >= point.Value.Position);

            if (tagUnderCursor == null) {
                provider.InfoSourceProvider.CurrentIssueKey = null;
                return;
            }

            provider.InfoSourceProvider.CurrentIssueKey = tagUnderCursor.IssueKey;

            ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive);

            if (!provider.QuickInfoBroker.IsQuickInfoActive(textView)) {
                provider.QuickInfoBroker.TriggerQuickInfo(textView, triggerPoint, true);
            }
        }

        public void Detach(ITextView textview) {
            if (textView != textview) return;
            textView.MouseHover -= OnTextViewMouseHover;
            textView = null;
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer) {
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer) {
        }
    }
}

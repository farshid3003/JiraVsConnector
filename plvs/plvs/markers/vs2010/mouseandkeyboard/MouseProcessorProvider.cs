using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Atlassian.plvs.markers.vs2010.texttag;
using Atlassian.plvs.util;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.mouseandkeyboard {
    [Export(typeof(IMouseProcessorProvider))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [Name("JiraIssueMouseProcessorProvider")]
    internal sealed class MouseProcessorProvider : IMouseProcessorProvider {
        [Import]
        public IViewTagAggregatorFactoryService TagAggregatorFactoryService;

        public IMouseProcessor GetAssociatedProcessor(IWpfTextView view) {
            return new MouseProcessor(this, view);
        }
    }

    internal class MouseProcessor : MouseProcessorBase {
        private readonly MouseProcessorProvider provider;
        private readonly IWpfTextView view;

        public MouseProcessor(MouseProcessorProvider provider, IWpfTextView view) {
            this.provider = provider;
            this.view = view;
        }

        public override void PreprocessMouseMove(MouseEventArgs e) {
            e.Handled = false;
        }

        public override void PreprocessMouseLeftButtonUp(MouseButtonEventArgs e) {

            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) return;

            JiraIssueTextTag textTag = getIssueTagUnderCursor(view, provider.TagAggregatorFactoryService);

            if (textTag == null) return;

            e.Handled = true;

            AtlassianPanel.Instance.Jira.findAndOpenIssue(textTag.IssueKey, (success, message, ex) => {
                if (!success) {
                    PlvsUtils.showError(message, ex);
                }
            });
        }

        private static JiraIssueTextTag getIssueTagUnderCursor(IWpfTextView view, IViewTagAggregatorFactoryService tagAggregatorFactory) {
            Point position = Mouse.GetPosition(view.VisualElement);
            position = relativeToView(view, position);

            var line = view.TextViewLines.GetTextViewLineContainingYCoordinate(position.Y);
            if (line == null) return null;
            var bufferPos = line.GetBufferPositionFromXCoordinate(position.X);
            if (!bufferPos.HasValue) return null;

            SnapshotPoint? point = view.BufferGraph.MapDownToFirstMatch(
                new SnapshotPoint(view.TextSnapshot, bufferPos.Value), PointTrackingMode.Positive, snapshot => true, PositionAffinity.Predecessor);

            if (point == null) return null;

            ITagAggregator<JiraIssueTextTag> aggregator = tagAggregatorFactory.CreateTagAggregator<JiraIssueTextTag>(view);
            IEnumerable<IMappingTagSpan<JiraIssueTextTag>> spans = aggregator.GetTags(new SnapshotSpan(new SnapshotPoint(view.TextSnapshot, 0),
                                                                               view.TextSnapshot.Length - 1));

            JiraIssueTextTag textTag = (from span in spans
                                        let t = span.Tag
                                        where t.Where.Start.Position <= point.Value.Position && t.Where.End.Position >= point.Value.Position
                                        select span.Tag).FirstOrDefault();

            return textTag;
        }

        private static Point relativeToView(ITextView view, Point position) {
            return new Point(position.X + view.ViewportLeft, position.Y + view.ViewportTop);
        }
    }
}

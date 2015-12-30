using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.util;
using Atlassian.plvs.util.jira;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Image = System.Windows.Controls.Image;
using MenuItem = System.Windows.Controls.MenuItem;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;

namespace Atlassian.plvs.markers.vs2010.marginglyph {
    [Export(typeof(IGlyphMouseProcessorProvider))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [Name("JiraIssueGlyphMouseProcessorProvider")]
    internal class JiraIssueGlyphMouseProcessorProvider : IGlyphMouseProcessorProvider {

        [Import]
        public IViewTagAggregatorFactoryService TagAggregatorFactoryService;

        public IMouseProcessor GetAssociatedMouseProcessor(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin margin) {
            return new MouseProcessor(this, wpfTextViewHost.TextView, margin);
        }

        private class MouseProcessor : MouseProcessorBase {
            private readonly JiraIssueGlyphMouseProcessorProvider provider;
            private readonly IWpfTextView textView;
            private readonly IWpfTextViewMargin margin;
            private const string RIGHT_CLICK_FOR_CONTEXT_MENU = "\r\n\r\nRight-click to open context menu";

            public MouseProcessor(JiraIssueGlyphMouseProcessorProvider provider, IWpfTextView textView, IWpfTextViewMargin margin) {
                this.provider = provider;
                this.textView = textView;
                this.margin = margin;
            }

            public override void PreprocessMouseEnter(MouseEventArgs e) {
                showToolTipIfOverIssueGlyph();
            }

            public override void PreprocessMouseMove(MouseEventArgs e) {
                showToolTipIfOverIssueGlyph();
            }

            private void showToolTipIfOverIssueGlyph() {
                JiraIssueLineGlyphTag tag = getIssueGlyphTagUnderCursor(textView, provider.TagAggregatorFactoryService);
                if (tag == null) {
                    margin.VisualElement.ToolTip = null;
                    margin.VisualElement.ContextMenu = null;
                    return;
                }
                ContextMenu contextMenu = new ContextMenu();
                string txt;
                IList<string> keys = tag.IssueKeys;
                if (keys.Count == 1) {
                    txt = "This line contains issue " + keys[0] + RIGHT_CLICK_FOR_CONTEXT_MENU;
                    addMenuItems(contextMenu, keys[0], true);
                } else {
                    StringBuilder sb = new StringBuilder();
                    foreach (var key in keys) {
                        sb.Append(key).Append(", ");
                        MenuItem menuItem = new MenuItem {Header = key};
                        addMenuItems(menuItem, key, false);
                        contextMenu.Items.Add(menuItem);
                    }
                    txt = sb.Length > 0 ? "This line contains issues: " + sb.ToString(0, sb.Length - 2) + RIGHT_CLICK_FOR_CONTEXT_MENU : null;
                }
                margin.VisualElement.ToolTip = txt;
                margin.VisualElement.ContextMenu = contextMenu;
            }

            private static void addMenuItems(ItemsControl menu, string key, bool displayKey) {
                MenuItem item1 = new MenuItem { Header = "Open" + (displayKey ? " " + key : "") + " in IDE", Icon = getImagePath(Resources.open_in_ide) };
                item1.Click += (s, e) => JiraIssueUtils.openInIde(key);
                menu.Items.Add(item1);
                MenuItem item2 = new MenuItem { Header = "View" + (displayKey ? " " + key : "") + " in the Browser", Icon = getImagePath(Resources.view_in_browser) };
                item2.Click += (s, e) => JiraIssueUtils.launchBrowser(key);
                menu.Items.Add(item2);
            }

            private static Image getImagePath(System.Drawing.Image img) {
                Image image = new Image {Source = PlvsUtils.bitmapSourceFromPngImage(img), Width = 16, Height = 16};
                return image;
            }
 
            private static JiraIssueLineGlyphTag getIssueGlyphTagUnderCursor(IWpfTextView view, IViewTagAggregatorFactoryService tagAggregatorFactory) {
                Point position = Mouse.GetPosition(view.VisualElement);
                position = relativeToView(view, position);

                ITextViewLine line = view.TextViewLines.GetTextViewLineContainingYCoordinate(position.Y);
                if (line == null) return null;
                SnapshotPoint? bufferPos = line.GetBufferPositionFromXCoordinate(line.Left);
                if (!bufferPos.HasValue) return null;

                ITagAggregator<JiraIssueLineGlyphTag> aggregator = tagAggregatorFactory.CreateTagAggregator<JiraIssueLineGlyphTag>(view);
                IEnumerable<IMappingTagSpan<JiraIssueLineGlyphTag>> spans = aggregator.GetTags(new SnapshotSpan(new SnapshotPoint(view.TextSnapshot, 0),
                                                                                   view.TextSnapshot.Length - 1));

                JiraIssueLineGlyphTag textTag = (from span in spans
                                            let t = span.Tag
                                            where t.Where.Start.GetContainingLine().LineNumber == bufferPos.Value.GetContainingLine().LineNumber
                                            select span.Tag).FirstOrDefault();

                return textTag;
            }

            private static Point relativeToView(ITextView view, Point position) {
                return new Point(position.X + view.ViewportLeft, position.Y + view.ViewportTop);
            }
        }
    }
}

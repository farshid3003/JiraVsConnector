using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Atlassian.plvs.util.jira;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.eventsinks {
    public sealed class MarginMarkerClientEventSink : AbstractMarkerClientEventSink {
        private readonly List<string> issueKeys;

        private const string RIGHT_CLICK_FOR_MENU = "\r\n\r\nRight-click to open context menu";

        public MarginMarkerClientEventSink(List<string> issueKeys) {
            this.issueKeys = issueKeys;
        }

        public override int GetTipText(IVsTextMarker pMarker, string[] pbstrText) {
            string txt;
            if (issueKeys.Count == 1) {
                txt = issueKeys[0];
            } else {
                StringBuilder sb = new StringBuilder();
                foreach (string issueKey in issueKeys) {
                    sb.Append(issueKey).Append(", ");
                }
                txt = sb.Length > 2 ? sb.ToString(0, sb.Length - 2) : "";
            }

            pbstrText[0] = issueKeys.Count > 1 
                ? "This line contains links to issues: " + txt + RIGHT_CLICK_FOR_MENU
                : "This line contains a link to issue " + txt + RIGHT_CLICK_FOR_MENU;

            return VSConstants.S_OK;
        }

        public override int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf) {
            const uint menuItemFlags = (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);

            if (pcmdf == null) {
                return VSConstants.S_OK;
            }
            
            pcmdf[0] = menuItemFlags;
            if (iItem == (int) MarkerCommandValues2.mcvRightClickCommand) {
                return VSConstants.S_OK;
            }
            return VSConstants.S_FALSE;
        }

        public override int ExecMarkerCommand(IVsTextMarker pMarker, int iItem) {
            if (iItem == (int)MarkerCommandValues2.mcvRightClickCommand) {
                ContextMenuStrip menu = new ContextMenuStrip();
                if (issueKeys.Count == 1) {
                    addMenuItems(menu, issueKeys[0], true);
                } else {
                    foreach (string issueKey in issueKeys) {
                        ToolStripMenuItem item = new ToolStripMenuItem(issueKey);
                        addMenuItems(item.DropDown, issueKey, false);
                        menu.Items.Add(item);
                    }
                }

                menu.Show(Cursor.Position);
            }
            return VSConstants.S_OK;
        }

        private static void addMenuItems(ToolStrip menu, string key, bool showKey) {
            ToolStripMenuItem item1 = new ToolStripMenuItem("Open " + (showKey ? key + " " : "") + "in IDE", Resources.open_in_ide);
            item1.Click += (s, e) => JiraIssueUtils.openInIde(key);
            menu.Items.Add(item1);
            ToolStripMenuItem item2 = new ToolStripMenuItem("View " + (showKey ? key + " " : "") + "in the Browser", Resources.view_in_browser);
            item2.Click += (s, e) => JiraIssueUtils.launchBrowser(key);
            menu.Items.Add(item2);
        }
    }
}
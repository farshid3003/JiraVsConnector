using System;
using System.Diagnostics;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.ui.jira.issuefilternodes;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.jira.issues.menus {
    public sealed class CustomFilterContextMenu : ContextMenuStrip {
        private readonly JiraServer server;
        private readonly JiraCustomFilterTreeNode filterNode;
        private readonly MenuSelectionAction editAction;
        private readonly MenuSelectionAction removeAction;

        private readonly ToolStripMenuItem[] items;

        public delegate void MenuSelectionAction(JiraCustomFilterTreeNode filterNode);

        public CustomFilterContextMenu(JiraServer server, JiraCustomFilterTreeNode filterNode, MenuSelectionAction editAction, MenuSelectionAction removeAction) {
            this.server = server;
            this.filterNode = filterNode;
            this.editAction = editAction;
            this.removeAction = removeAction;

            items = new[]
                    {
                        new ToolStripMenuItem("Edit Filter", Resources.edit, new EventHandler(editFilter)),
                        new ToolStripMenuItem("Remove Filter", Resources.minus, new EventHandler(removeFilter)), 
                        new ToolStripMenuItem("View Filter in Browser", Resources.view_in_browser,
                                              new EventHandler(browseFilter)),
                    };

            Items.Add("dummy");

            Opened += filterContextMenuOpened;
        }

        private void filterContextMenuOpened(object sender, EventArgs e) {
            Items.Clear();

            Items.Add(items[0]);
            Items.Add(items[1]);
            if (!filterNode.Filter.Empty) {
                Items.Add(items[2]);
            }
        }

        private void browseFilter(object sender, EventArgs e) {
            string url = server.Url;
            try {
                if (server.BuildNumber > 0) {
                    // we have REST
                    PlvsUtils.runBrowser(url + filterNode.Filter.getBrowserJqlQueryString());
                } else {
                    PlvsUtils.runBrowser(url + filterNode.Filter.getOldstyleBrowserQueryString());
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        private void editFilter(object sender, EventArgs e) {
            editAction(filterNode);
        }

        private void removeFilter(object sender, EventArgs e) {
            removeAction(filterNode);
        }
    }
}
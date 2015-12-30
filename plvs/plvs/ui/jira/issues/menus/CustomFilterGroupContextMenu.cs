using System;
using System.Windows.Forms;
using Atlassian.plvs.ui.jira.issuefilternodes;

namespace Atlassian.plvs.ui.jira.issues.menus {
    public sealed class CustomFilterGroupContextMenu : ContextMenuStrip {

        private readonly ToolStripMenuItem[] items;
        private readonly JiraCustomFiltersGroupTreeNode groupTreeNode;
        private readonly AddFilterAction addFilterAction;

        public delegate void AddFilterAction(JiraCustomFiltersGroupTreeNode groupTreeNode);

        public CustomFilterGroupContextMenu(JiraCustomFiltersGroupTreeNode groupTreeNode, AddFilterAction addFilterAction) {
            this.groupTreeNode = groupTreeNode;
            this.addFilterAction  = addFilterAction;

            items = new[]
                    {
                        new ToolStripMenuItem("Add Filter", Resources.plus, new EventHandler(addFilter)),
                    };

            Items.Add("dummy");

            Opened += filterContextMenuOpened;
        }

        private void filterContextMenuOpened(object sender, EventArgs e) {
            Items.Clear();

            Items.Add(items[0]);
        }

        private void addFilter(object sender, EventArgs e) {
            addFilterAction(groupTreeNode);
        }
    }
}
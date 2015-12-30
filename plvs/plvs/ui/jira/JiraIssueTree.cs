using System;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.store;
using Atlassian.plvs.ui.jira.issues;
using Atlassian.plvs.ui.jira.issues.menus;

namespace Atlassian.plvs.ui.jira {
    public sealed class JiraIssueTree : TreeViewAdv {

        private const string JIRA_UPDATED_COLUMN_WIDTH = "JiraUpdatedColumnWidth";
        private const string JIRA_STATUS_COLUMN_WIDTH = "JiraStatusColumnWidth";

        private int updatedWidth;
        private int statusWidth;

        private readonly Control parent;
        private readonly StatusLabel status;
        private readonly JiraIssueListModel model;

        public TreeNodeCollapseExpandStatusManager CollapseExpandManager { get; set; }

        private const int MARGIN = 16;
        private const int STATUS_WIDTH_DEFAULT = 150;
        private const int UPDATED_WIDTH_DEFAULT = 150;
        private const int PRIORITY_WIDTH = 24;
        private const int STATUS_MIN = 100;
        private const int UPDATED_MIN = 100;

        private readonly TreeColumn colName = new TreeColumn();
        private readonly TreeColumn colStatus = new TreeColumn();
        private readonly TreeColumn colPriority = new TreeColumn();
        private readonly TreeColumn colUpdated = new TreeColumn();
        private readonly NodeIcon controlIcon = new NodeIcon();
//        private readonly NodeTextBox controlName = new BoldableNodeTextBox();
        private readonly NodeTextBox controlName = new NodeTextBox();
        private readonly NodeTextBox controlStatusText = new NodeTextBox();
        private readonly NodeIcon controlStatusIcon = new NodeIcon();
        private readonly NodeIcon controlPriorityIcon = new NodeIcon();
        private readonly NodeTextBox controlUpdated = new NodeTextBox();

        private class ToolTipProvider : IToolTipProvider {
            public string GetToolTip(TreeNodeAdv node, NodeControl nodeControl) {
                IssueNode issueNode = node.Tag as IssueNode;
                if (issueNode != null) {
                    return issueNode.Issue.ToolTipText;
                }
                return null;
            }
        }

        private static readonly ToolTipProvider toolTipProvider = new ToolTipProvider();

        public JiraIssueTree(SplitContainer splitter, StatusLabel status, JiraIssueListModel model, int itemHeight, Font font) {
            parent = splitter.Panel2;
            this.status = status;
            this.model = model;

            Dock = DockStyle.Fill;
            SelectionMode = TreeSelectionMode.Single;
            FullRowSelect = true;
            GridLineStyle = GridLineStyle.None;
            UseColumns = true;
            RowHeight = itemHeight;
            Font = font;
            ShowNodeToolTips = true;

            colName.Header = "Summary";
            colStatus.Header = "Status";
            colPriority.Header = "P";
            colUpdated.Header = "Updated";

            int i = 0;
            controlIcon.ParentColumn = colName;
            controlIcon.DataPropertyName = "Icon";
            controlIcon.LeftMargin = i++;
            controlIcon.ToolTipProvider = toolTipProvider;

            controlName.ParentColumn = colName;
            controlName.DataPropertyName = "Name";
            controlName.Trimming = StringTrimming.EllipsisCharacter;
            controlName.UseCompatibleTextRendering = true;
            controlName.ToolTipProvider = toolTipProvider;
            controlName.LeftMargin = i++;

            controlPriorityIcon.ParentColumn = colPriority;
            controlPriorityIcon.DataPropertyName = "PriorityIcon";
            controlPriorityIcon.ToolTipProvider = toolTipProvider;
            controlPriorityIcon.LeftMargin = i++;

            controlStatusIcon.ParentColumn = colStatus;
            controlStatusIcon.DataPropertyName = "StatusIcon";
            controlStatusIcon.ToolTipProvider = toolTipProvider;
            controlStatusIcon.LeftMargin = i++;

            controlStatusText.ParentColumn = colStatus;
            controlStatusText.DataPropertyName = "StatusText";
            controlStatusText.Trimming = StringTrimming.EllipsisCharacter;
            controlStatusText.UseCompatibleTextRendering = true;
            controlStatusText.ToolTipProvider = toolTipProvider;
            controlStatusText.LeftMargin = i++;

            controlUpdated.ParentColumn = colUpdated;
            controlUpdated.DataPropertyName = "Updated";
            controlUpdated.Trimming = StringTrimming.EllipsisCharacter;
            controlUpdated.UseCompatibleTextRendering = true;
            controlUpdated.TextAlign = HorizontalAlignment.Right;
            controlUpdated.ToolTipProvider = toolTipProvider;
            controlUpdated.LeftMargin = i;

            Columns.Add(colName);
            Columns.Add(colPriority);
            Columns.Add(colStatus);
            Columns.Add(colUpdated);

            NodeControls.Add(controlIcon);
            NodeControls.Add(controlName);
            NodeControls.Add(controlPriorityIcon);
            NodeControls.Add(controlStatusIcon);
            NodeControls.Add(controlStatusText);
            NodeControls.Add(controlUpdated);

            loadColumnWidths();

            parent.SizeChanged += parentSizeChanged;
            splitter.SizeChanged += parentSizeChanged;
            splitter.SplitterMoved += parentSizeChanged;

            colPriority.TextAlign = HorizontalAlignment.Left;
            colPriority.Width = PRIORITY_WIDTH;
            colPriority.MinColumnWidth = PRIORITY_WIDTH;
            colPriority.MaxColumnWidth = PRIORITY_WIDTH;

            colStatus.Width = statusWidth;
            colStatus.MinColumnWidth = STATUS_MIN;

            colUpdated.Width = updatedWidth;
            colUpdated.MinColumnWidth = UPDATED_MIN;

            colName.TextAlign = HorizontalAlignment.Left;
            colPriority.TooltipText = "Priority";
            colStatus.TextAlign = HorizontalAlignment.Left;
            colPriority.TextAlign = HorizontalAlignment.Left;
            colUpdated.TextAlign = HorizontalAlignment.Right;

            setSummaryColumnWidth();

            ItemDrag += jiraIssueTreeItemDrag;

            Expanded += jiraIssueTreeExpanded;
            Collapsed += jiraIssueTreeCollapsed;

            colUpdated.WidthChanged += columnWidthChanged;
            colStatus.WidthChanged += columnWidthChanged;
        }

        private void columnWidthChanged(object sender, EventArgs e) {
            saveColumnWidths();
            setSummaryColumnWidth();
        }

        private void saveColumnWidths() {
            statusWidth = colStatus.Width;
            updatedWidth = colUpdated.Width;

            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            store.storeParameter(JIRA_STATUS_COLUMN_WIDTH, statusWidth);
            store.storeParameter(JIRA_UPDATED_COLUMN_WIDTH, updatedWidth);
        }

        private void loadColumnWidths() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            statusWidth = store.loadParameter(JIRA_STATUS_COLUMN_WIDTH, STATUS_WIDTH_DEFAULT);
            updatedWidth = store.loadParameter(JIRA_UPDATED_COLUMN_WIDTH, UPDATED_WIDTH_DEFAULT);
        }

        public void addContextMenu(ToolStripItem[] items) {
            IssueContextMenu strip = new IssueContextMenu(model, status, this, items);
            ContextMenuStrip = strip;
        }

        private void setSummaryColumnWidth() {
            int summaryWidth = parent.Width - PRIORITY_WIDTH - updatedWidth - statusWidth - SystemInformation.VerticalScrollBarWidth - MARGIN;
            if (summaryWidth < 0) {
                summaryWidth = 4 * PRIORITY_WIDTH;
            }
            colName.Width = summaryWidth;
            colName.MinColumnWidth = summaryWidth;
            colName.MaxColumnWidth = summaryWidth;
        }

        private void parentSizeChanged(object sender, EventArgs e) {
            setSummaryColumnWidth();
        }

        private class BoldableNodeTextBox : NodeTextBox {
            protected override void OnDrawText(DrawEventArgs args) {
                args.Font = new Font(args.Font, FontStyle.Bold);
                base.OnDrawText(args);
            }

            protected override bool DrawTextMustBeFired(TreeNodeAdv node) {
                return !(node.Tag is IssueNode);
            }
        }

        private void jiraIssueTreeItemDrag(object sender, ItemDragEventArgs e) {
            
            if (SelectedNode == null || !(SelectedNode.Tag is IssueNode)) return;

            IssueNode n = (IssueNode) SelectedNode.Tag;
            DataObject d = new DataObject();

            d.SetText("ISSUE:" + n.Issue.Key + ":SERVER:{" + n.Issue.Server.GUID + "}");

            DoDragDrop(d, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private bool isRestoring;
        
        public void restoreExpandCollapseStates() {
            if (CollapseExpandManager == null) {
                ExpandAll();
                return;
            }
            isRestoring = true;
            foreach (var node in AllNodes) {
                restoreExpandCollapseState(node);
            }
            isRestoring = false;
        }

        private void restoreExpandCollapseState(TreeNodeAdv node) {
            if (CollapseExpandManager == null) return;
            bool expanded = CollapseExpandManager.restoreNodeState(node.Tag);
            if (expanded) node.Expand(true); else node.Collapse(true);
        }

        private void rememberExpandCollapseState(TreeNodeAdv node) {
            if (CollapseExpandManager == null) return;

            TreeNodeCollapseExpandStatusManager.TreeNodeRememberingCollapseState n = 
                node.Tag as TreeNodeCollapseExpandStatusManager.TreeNodeRememberingCollapseState;
            if (n == null) return;
            n.NodeExpanded = node.IsExpanded;
            CollapseExpandManager.rememberNodeState(node.Tag);
        }

        private void jiraIssueTreeCollapsed(object sender, TreeViewAdvEventArgs e) {
            if (isRestoring) return;
            rememberExpandCollapseState(e.Node);
        }

        private void jiraIssueTreeExpanded(object sender, TreeViewAdvEventArgs e) {
            if (isRestoring) return;
            rememberExpandCollapseState(e.Node);
        }
    }
}
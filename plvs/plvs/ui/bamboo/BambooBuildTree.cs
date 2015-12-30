using System;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Atlassian.plvs.store;

namespace Atlassian.plvs.ui.bamboo {
    public sealed class BambooBuildTree : TreeViewAdv {
        private readonly Control parent;

        public TreeNodeCollapseExpandStatusManager CollapseExpandManager { get; set; }

        private readonly TreeColumn colStatusAndKey = new TreeColumn();
        private readonly TreeColumn colTests = new TreeColumn();
        private readonly TreeColumn colReason = new TreeColumn();
        private readonly TreeColumn colCompleted = new TreeColumn();
        private readonly TreeColumn colDuration = new TreeColumn();
        private readonly TreeColumn colServer = new TreeColumn();
        private readonly NodeIcon controlIcon = new NodeIcon();
        private readonly NodeBuildInProgressIcon inProgressIcon;
        private readonly NodeTextBox controlBuildKey = new NodeTextBox();
        private readonly NodeTextBox controlTests = new NodeTextBox();
        private readonly NodeTextBox controlReason = new NodeTextBox();
        private readonly NodeTextBox controlCompleted = new NodeTextBox();
        private readonly NodeTextBox controlDuration = new NodeTextBox();
        private readonly NodeTextBox controlServer = new NodeTextBox();

        private const string BAMBOO_STATUS_COLUMN_WIDTH = "BambooStatusColumnWidth";
        private const string BAMBOO_TESTS_COLUMN_WIDTH = "BambooTestsColumnWidth";
        private const string BAMBOO_COMPLETED_COLUMN_WIDTH = "BambooCompletedColumnWidth";
        private const string BAMBOO_DURATION_COLUMN_WIDTH = "BambooDurationColumnWidth";
        private const string BAMBOO_SERVER_COLUMN_WIDTH = "BambooServerColumnWidth";

        private int statusWidth;
        private int testsWidth;
        private int completedWidth;
        private int durationWidth;
        private int serverWidth;

        private const int STATUS_AND_KEY_WIDTH_DEFAULT = 200;
        private const int TESTS_WIDTH_DEFAULT = 200;
        private const int COMPLETED_WIDTH_DEFAULT = 150;
        private const int DURATION_WIDTH_DEFAULT = 150;
        private const int SERVER_WIDTH_DEFAULT = 200;

        private const int MIN_COLUMN_WIDTH = 50;

        private const int MARGIN = 24;

        public BambooBuildTree(SplitContainer splitter) {
            parent = splitter.Panel1;

            Dock = DockStyle.Fill;
            SelectionMode = TreeSelectionMode.Single;
            FullRowSelect = true;
            GridLineStyle = GridLineStyle.None;
            UseColumns = true;

            colStatusAndKey.Header = "Key and Status";
            colTests.Header = "Tests";
            colReason.Header = "Reason";
            colCompleted.Header = "Completed";
            colDuration.Header = "Duration";
            colServer.Header = "Server";

            int i = 0;

            controlIcon.ParentColumn = colStatusAndKey;
            controlIcon.DataPropertyName = "Icon";
            controlIcon.LeftMargin = i++;

            inProgressIcon = new NodeBuildInProgressIcon(this) {
                                 ParentColumn = colStatusAndKey,
                                 DataPropertyName = "IsInProgress",
                                 LeftMargin = i++
                             };

            controlBuildKey.ParentColumn = colStatusAndKey;
            controlBuildKey.DataPropertyName = "Key";
            controlBuildKey.Trimming = StringTrimming.EllipsisCharacter;
            controlBuildKey.UseCompatibleTextRendering = true;
            controlBuildKey.LeftMargin = i++;

            controlTests.ParentColumn = colTests;
            controlTests.DataPropertyName = "Tests";
            controlTests.Trimming = StringTrimming.EllipsisCharacter;
            controlTests.UseCompatibleTextRendering = true;
            controlTests.LeftMargin = i++;

            controlReason.ParentColumn = colReason;
            controlReason.DataPropertyName = "Reason";
            controlReason.Trimming = StringTrimming.EllipsisCharacter;
            controlReason.UseCompatibleTextRendering = true;
            controlReason.LeftMargin = i++;

            controlCompleted.ParentColumn = colCompleted;
            controlCompleted.DataPropertyName = "Completed";
            controlCompleted.Trimming = StringTrimming.EllipsisCharacter;
            controlCompleted.UseCompatibleTextRendering = true;
            controlCompleted.LeftMargin = i++;

            controlDuration.ParentColumn = colDuration;
            controlDuration.DataPropertyName = "Duration";
            controlDuration.Trimming = StringTrimming.EllipsisCharacter;
            controlDuration.UseCompatibleTextRendering = true;
            controlDuration.LeftMargin = i++;

            controlServer.ParentColumn = colServer;
            controlServer.DataPropertyName = "Server";
            controlServer.Trimming = StringTrimming.EllipsisCharacter;
            controlServer.UseCompatibleTextRendering = true;
            controlServer.TextAlign = HorizontalAlignment.Right;
            controlServer.LeftMargin = i;

            Columns.Add(colStatusAndKey);
            Columns.Add(colTests);
            Columns.Add(colReason);
            Columns.Add(colCompleted);
            Columns.Add(colDuration);
            Columns.Add(colServer);

            NodeControls.Add(controlIcon);
            NodeControls.Add(inProgressIcon);
            NodeControls.Add(controlBuildKey);
            NodeControls.Add(controlTests);
            NodeControls.Add(controlReason);
            NodeControls.Add(controlCompleted);
            NodeControls.Add(controlDuration);
            NodeControls.Add(controlServer);

            colServer.TextAlign = HorizontalAlignment.Right;

            parent.Resize += bambooBuildTreeResize;
            splitter.Resize += bambooBuildTreeResize;
            splitter.SplitterMoved += bambooBuildTreeResize;


            colStatusAndKey.MinColumnWidth = MIN_COLUMN_WIDTH;
            colTests.MinColumnWidth = MIN_COLUMN_WIDTH;
            colCompleted.MinColumnWidth = MIN_COLUMN_WIDTH;
            colDuration.MinColumnWidth = MIN_COLUMN_WIDTH;
            colServer.MinColumnWidth = MIN_COLUMN_WIDTH;

            loadColumnWidths();
            resizeColumns();

            colStatusAndKey.WidthChanged += columnWidthChanged;
            colTests.WidthChanged += columnWidthChanged;
            colCompleted.WidthChanged += columnWidthChanged;
            colDuration.WidthChanged += columnWidthChanged;
            colServer.WidthChanged += columnWidthChanged;

            Expanded += treeExpanded;
            Collapsed += treeCollapsed;
        }

        private void loadColumnWidths() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            statusWidth = store.loadParameter(BAMBOO_STATUS_COLUMN_WIDTH, STATUS_AND_KEY_WIDTH_DEFAULT);
            testsWidth = store.loadParameter(BAMBOO_TESTS_COLUMN_WIDTH, TESTS_WIDTH_DEFAULT);
            completedWidth = store.loadParameter(BAMBOO_COMPLETED_COLUMN_WIDTH, COMPLETED_WIDTH_DEFAULT);
            durationWidth = store.loadParameter(BAMBOO_DURATION_COLUMN_WIDTH, DURATION_WIDTH_DEFAULT);
            serverWidth = store.loadParameter(BAMBOO_SERVER_COLUMN_WIDTH, SERVER_WIDTH_DEFAULT);
        }

        private void saveColumnWidths() {
            statusWidth = colStatusAndKey.Width;
            testsWidth = colTests.Width;
            completedWidth = colCompleted.Width;
            durationWidth = colDuration.Width;
            serverWidth = colServer.Width;

            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            store.storeParameter(BAMBOO_STATUS_COLUMN_WIDTH, statusWidth);
            store.storeParameter(BAMBOO_TESTS_COLUMN_WIDTH, testsWidth);
            store.storeParameter(BAMBOO_COMPLETED_COLUMN_WIDTH, completedWidth);
            store.storeParameter(BAMBOO_DURATION_COLUMN_WIDTH, durationWidth);
            store.storeParameter(BAMBOO_SERVER_COLUMN_WIDTH, serverWidth);
        }

        private void columnWidthChanged(object sender, EventArgs e) {
            saveColumnWidths();
            resizeReasonColumn();
        }

        private void bambooBuildTreeResize(object sender, EventArgs e) {
            resizeColumns();
        }

        private void resizeColumns() {
            colStatusAndKey.Width = statusWidth;
            colTests.Width = testsWidth;
            colCompleted.Width = completedWidth;
            colDuration.Width = durationWidth;
            colServer.Width = serverWidth;

            resizeReasonColumn();
        }

        private void resizeReasonColumn() {
            int total = statusWidth + testsWidth + completedWidth + durationWidth + serverWidth + MARGIN;
            int reasonWidth = total < parent.Width ? parent.Width - total : MIN_COLUMN_WIDTH;
            colReason.Width = reasonWidth;
            colReason.MinColumnWidth = reasonWidth;
            colReason.MaxColumnWidth = reasonWidth;
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

            var n = node.Tag as TreeNodeCollapseExpandStatusManager.TreeNodeRememberingCollapseState;
            if (n == null) return;
            n.NodeExpanded = node.IsExpanded;
            CollapseExpandManager.rememberNodeState(node.Tag);
        }

        private void treeCollapsed(object sender, TreeViewAdvEventArgs e) {
            if (isRestoring) return;
            rememberExpandCollapseState(e.Node);
        }
        private void treeExpanded(object sender, TreeViewAdvEventArgs e) {
            if (isRestoring) return;
            rememberExpandCollapseState(e.Node);
        }
    }
}

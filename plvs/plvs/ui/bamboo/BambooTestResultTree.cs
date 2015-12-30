using System;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.bamboo {
    public sealed class BambooTestResultTree : TreeViewAdv {

        private readonly TreeColumn colIconAndName = new TreeColumn();
        private readonly TreeColumn colResult = new TreeColumn();
        private readonly NodeIcon controlIcon = new NodeIcon();
        private readonly NodeTextBox controlName = new NodeTextBox();
        private readonly ColoredNodeTextBox controlResult;

        private const int NAME_WIDTH = 300;
        private const int RESULT_WIDTH = 200;
        private const int MARGIN = 24;

        private class ColoredNodeTextBox : NodeTextBox {

            private readonly TreeViewAdv parent;

            public ColoredNodeTextBox(TreeViewAdv parent) {
                this.parent = parent;
            }

            protected override void OnDrawText(DrawEventArgs args) {
                TestMethodNode node = args.Node.Tag as TestMethodNode;
                if (node != null) {
                    args.TextColor = args.Node.IsSelected && parent.Focused
                        ? SystemColors.HighlightText 
                        : ColorTranslator.FromHtml(node.Test.Result.GetColorValue());
                }
                base.OnDrawText(args);
            }

            protected override bool DrawTextMustBeFired(TreeNodeAdv node) {
                return node.Tag is TestMethodNode;
            }
        }

        public BambooTestResultTree() {
            Dock = DockStyle.Fill;
            SelectionMode = TreeSelectionMode.Single;
            FullRowSelect = true;
            GridLineStyle = GridLineStyle.None;
            UseColumns = true;

            colIconAndName.Header = "Test Method";
            colResult.Header = "Result";

            int i = 0;

            controlIcon.ParentColumn = colIconAndName;
            controlIcon.DataPropertyName = "Icon";
            controlIcon.LeftMargin = i++;

            controlName.ParentColumn = colIconAndName;
            controlName.DataPropertyName = "Name";
            controlName.Trimming = StringTrimming.EllipsisCharacter;
            controlName.UseCompatibleTextRendering = true;
            controlName.LeftMargin = i++;

            controlResult = new ColoredNodeTextBox(this) {
                                ParentColumn = colResult,
                                DataPropertyName = "Result",
                                Trimming = StringTrimming.EllipsisCharacter,
                                UseCompatibleTextRendering = true,
                                LeftMargin = i++,
                                TextAlign = HorizontalAlignment.Right
                            };

            Columns.Add(colIconAndName);
            Columns.Add(colResult);

            NodeControls.Add(controlIcon);
            NodeControls.Add(controlName);
            NodeControls.Add(controlResult);

            colResult.TextAlign = HorizontalAlignment.Right;

            Resize += bambooBuildTreeResize;

            resizeColumns();
        }

        private void resizeColumns() {
            const int total = RESULT_WIDTH + MARGIN;
            colIconAndName.Width = total < Width ? Width - total : NAME_WIDTH;
            colResult.Width = RESULT_WIDTH;
        }

        private void bambooBuildTreeResize(object sender, EventArgs e) {
            resizeColumns();
        }
    }
}

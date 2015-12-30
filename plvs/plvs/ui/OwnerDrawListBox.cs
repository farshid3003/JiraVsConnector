using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

///
/// For everybody interested - this shit does not work properly - at least not on Windows 7. 
/// No matter what I do, the listbox looks like crap.
/// 
/// I am leaving it here as a warning to others.
/// 
namespace Atlassian.plvs.ui {
    public sealed class OwnerDrawListBox : ListBox {
        public OwnerDrawListBox() {
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawItem += ownerDrawListBoxDrawItem;
            ItemHeight = 17;
            DoubleBuffered = true;
        }

        private void ownerDrawListBoxDrawItem(object sender, DrawItemEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
//            e.Graphics.TextContrast = 8;
//            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
//            bool selected = e.State.Equals(DrawItemState.Selected);
//            SolidBrush backBrush = new SolidBrush(selected ? SystemColors.Highlight : e.BackColor);
//            e.Graphics.FillRectangle(backBrush, e.Bounds);
//            backBrush.Dispose();
            if (Items.Count > e.Index && e.Bounds.Y % 17 == 0) {
                e.DrawBackground();
//                SolidBrush foreBrush = new SolidBrush(selected ? SystemColors.HighlightText : e.ForeColor);
                SolidBrush foreBrush = new SolidBrush(e.ForeColor);
                e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, foreBrush, e.Bounds.X, e.Bounds.Y + 1);
                foreBrush.Dispose();
            }
            e.Graphics.SmoothingMode = SmoothingMode.Default;
            e.DrawFocusRectangle();
        }
    }
}

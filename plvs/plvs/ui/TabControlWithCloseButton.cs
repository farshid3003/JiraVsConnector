using System.Drawing;
using System.Windows.Forms;

namespace Atlassian.plvs.ui {
    public class TabControlWithCloseButton : TabControl {
        public delegate bool PreRemoveTab(int indx);
        public delegate void PostRemoveTab(int indx);

        public PreRemoveTab PreRemoveTabPage { get; set; }
        public PostRemoveTab PostRemoveTabPage { get; set; }

        public TabControlWithCloseButton() {
            PreRemoveTabPage = null;
            DrawMode = TabDrawMode.OwnerDrawFixed;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            TabStop = false;
            Padding = new Point(Padding.X + 8, Padding.Y);
//            MouseMove += tabControlWithCloseButtonMouseMove;
        }

//        void tabControlWithCloseButtonMouseMove(object sender, MouseEventArgs e) {
//            Invalidate();
//        }

        protected override void OnDrawItem(DrawItemEventArgs e) {
            Rectangle r = getCloseButtonRect(e.Index);
            Brush b = new SolidBrush(Color.Black);
            Pen p = new Pen(b, 2);
            e.Graphics.DrawLine(p, r.X, r.Y, r.X + r.Width, r.Y + r.Height);
            e.Graphics.DrawLine(p, r.X + r.Width, r.Y, r.X, r.Y + r.Height);

            string title = TabPages[e.Index].Text;
            Font f = Font;
            Rectangle tabRect = GetTabRect(e.Index);
            bool haveImage = ImageList != null && ImageList.Images.Count > 0;
            if (haveImage) {
                e.Graphics.DrawImage(ImageList.Images[TabPages[e.Index].ImageIndex], tabRect.X + 6, tabRect.Y + 2);
            }
            e.Graphics.DrawString(title, f, b, new PointF(tabRect.X + (haveImage ? 26 : 3), tabRect.Y + 2));

            p.Dispose();
            b.Dispose();
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            Point p = e.Location;
            
            for (int i = 0; i < TabCount; i++) {
                Rectangle r = getCloseButtonRect(i);
                if ((e.Button == MouseButtons.Middle && GetTabRect(i).Contains(e.Location)) || r.Contains(p)) {
                    CloseTab(i);
                }
            }
        }

        private Rectangle getCloseButtonRect(int index) {
            Rectangle r = GetTabRect(index);
            r.Offset(r.Width - 16, r.Height / 2 - 4);
            r.Width = 8;
            r.Height = 8;
            return r;
        }

        private void CloseTab(int i) {
            if (PreRemoveTabPage != null) {
                bool closeIt = PreRemoveTabPage(i);
                if (!closeIt)
                    return;
            }
            TabPages.Remove(TabPages[i]);

            if (PostRemoveTabPage != null) PostRemoveTabPage(i);
        }
    }
}

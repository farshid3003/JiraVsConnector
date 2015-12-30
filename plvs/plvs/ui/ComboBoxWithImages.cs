using System.Drawing;
using System.Windows.Forms;

namespace Atlassian.plvs.ui {
    public class ComboBoxWithImages<T> : ComboBox {
        private ImageList imageList;

        public ImageList ImageList {
            get { return imageList; }
            set { imageList = value; }
        }

        public ComboBoxWithImages() {
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(DrawItemEventArgs ea) {
            ea.DrawBackground();
            ea.DrawFocusRectangle();

            ComboBoxWithImagesItem<T> item;
            Size imageSize = imageList != null ? imageList.ImageSize : new Size(0, 0);
            Rectangle bounds = ea.Bounds;

            if (ea.Index >= 0 && Items.Count > ea.Index) {
                item = (ComboBoxWithImagesItem<T>)Items[ea.Index];

                if (imageList != null && item.ImageIndex != -1) {
                    imageList.Draw(ea.Graphics, bounds.Left, bounds.Top, item.ImageIndex);
                    ea.Graphics.DrawString(item.Value.ToString(), ea.Font, new SolidBrush(ea.ForeColor),
                                           bounds.Left + imageSize.Width, bounds.Top);
                } else {
                    ea.Graphics.DrawString(item.Value.ToString(), ea.Font, new SolidBrush(ea.ForeColor), bounds.Left, bounds.Top);
                }
            } else if (ea.Index != -1) {
                ea.Graphics.DrawString(Items[ea.Index].ToString(), ea.Font, new SolidBrush(ea.ForeColor),
                                       bounds.Left, bounds.Top);
            } else {
                ea.Graphics.DrawString(Text, ea.Font, new SolidBrush(ea.ForeColor), bounds.Left, bounds.Top);
            }

            base.OnDrawItem(ea);
        }
    }

    public class ComboBoxWithImagesItem<T> {
        public T Value { get; set; }

        public int ImageIndex { get; set; }

        public ComboBoxWithImagesItem(T value)
            : this(value, -1) {}

        public ComboBoxWithImagesItem(T value, int imageIndex) {
            Value = value;
            ImageIndex = imageIndex;
        }

        public override string ToString() {
            return Value.ToString();
        }
    }
}

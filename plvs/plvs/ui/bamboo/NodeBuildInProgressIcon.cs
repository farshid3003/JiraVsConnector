using System;
using System.Drawing;
using System.IO;
using System.Threading;
using Aga.Controls;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace Atlassian.plvs.ui.bamboo {

    internal class NodeBuildInProgressIcon : BindableControl {
        private readonly TreeViewAdv parent;
        private static int instances;

        public NodeBuildInProgressIcon(TreeViewAdv parent) {
            this.parent = parent;
            lock (lockObject) {
                if (instances++ == 0) Start();
            }
            IconChanged += inProgressIconIconChanged;
        }

        protected override void Dispose(bool disposing) {
            lock (lockObject) {
                if (disposing && --instances == 0) {
                    Stop();
                }
            }
            IconChanged -= inProgressIconIconChanged;
            base.Dispose(disposing);
        }

        private void inProgressIconIconChanged(object sender, EventArgs e) {
            parent.Invalidate();
        }

        private static GifDecoder GetGifDecoder(byte[] data) {
            using (MemoryStream ms = new MemoryStream(data))
                return new GifDecoder(ms, true);
        }

        private static readonly GifDecoder gif = GetGifDecoder(Resources.small_throbber);
        private static int index;
        private static volatile Thread animatingThread;
        private static readonly object lockObject = new object();

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context) {
            return isInProgress(node) ? gif.FrameSize : new Size(0, 0);
        }

        private bool isInProgress(TreeNodeAdv node) {
            bool? isInProgress = GetValue(node) as bool?;
            return isInProgress.HasValue && isInProgress.Value;
        }

        protected override void OnIsVisibleValueNeeded(NodeControlValueEventArgs args) {
            args.Value = isInProgress(args.Node);
            base.OnIsVisibleValueNeeded(args);
        }

        public override void Draw(TreeNodeAdv node, DrawContext context) {
            if (!isInProgress(node)) return;

            Rectangle rect = GetBounds(node, context);
            Image img = gif.GetFrame(index).Image;
            context.Graphics.DrawImage(img, rect.Location);
        }

        private static void Start() {
            if (animatingThread != null) return;

            index = 0;
            animatingThread = new Thread(IterateIcons) {
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            };
            animatingThread.Start();
        }

        private static void Stop() {
            index = 0;
            animatingThread = null;
        }

        private static void IterateIcons() {
            while (animatingThread != null) {
                if (index < gif.FrameCount - 1) index++;
                else index = 0;

                if (IconChanged != null) IconChanged(null, EventArgs.Empty);

                int delay = gif.GetFrame(index).Delay;
                Thread.Sleep(delay);
            }
        }

        public static event EventHandler IconChanged;
    }
}

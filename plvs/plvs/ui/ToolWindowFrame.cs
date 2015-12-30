using System;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Atlassian.plvs.ui {
    public class ToolWindowFrame : UserControl, IVsWindowFrameNotify {
        public ToolWindowPane WindowFrame { get; set; }

        public class ShowHideEventArgs : EventArgs {
            public ShowHideEventArgs(bool visible) {
                Visible = visible;
            }

            public bool Visible { get; private set; }
        }

        public EventHandler<ShowHideEventArgs> ShownOrHidden;

        public bool FrameVisible {
            get { return WindowFrame != null && ((IVsWindowFrame)WindowFrame.Frame).IsVisible() == VSConstants.S_OK; }
            set {
                if (WindowFrame == null) {
                    return;
                }
                if (value) {
                    ((IVsWindowFrame)WindowFrame.Frame).Show();
                } else {
                    ((IVsWindowFrame)WindowFrame.Frame).Hide();
                }
            }
        }

        public int OnShow(int fShow) {
            switch (fShow) {
                case (int)__FRAMESHOW.FRAMESHOW_WinShown:
                    if (ShownOrHidden != null) ShownOrHidden(this, new ShowHideEventArgs(true));
                    break;
                case (int)__FRAMESHOW.FRAMESHOW_WinHidden:
                    if (ShownOrHidden != null) ShownOrHidden(this, new ShowHideEventArgs(false));
                    break;
            }

            return VSConstants.S_OK;
        }

        public int OnMove() {
            return VSConstants.S_OK;
        }

        public int OnSize() {
            return VSConstants.S_OK;
        }

        public int OnDockableChange(int fDockable) {
            return VSConstants.S_OK;
        }

    }
}

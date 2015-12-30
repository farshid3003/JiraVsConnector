using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Atlassian.plvs.windows {
    public class ToolWindowManager {
        public ToolWindowPane AtlassianWindow { get; set; }
        public ToolWindowPane IssueDetailsWindow { get; set; }
        public ToolWindowPane BuildDetailsWindow { get; set; }

        private static readonly ToolWindowManager INSTANCE = new ToolWindowManager();

        public static ToolWindowManager Instance { get { return INSTANCE; } }
        
        private ToolWindowManager() {}

        public bool AtlassianWindowVisible {
            get {
                if (AtlassianWindow == null) {
                    return false;
                }
                IVsWindowFrame windowFrame = (IVsWindowFrame) AtlassianWindow.Frame;
                int visible = windowFrame.IsVisible();
                return visible != VSConstants.S_FALSE;
            }

            set {
                if (AtlassianWindow == null) return;
                IVsWindowFrame windowFrame = (IVsWindowFrame)AtlassianWindow.Frame;
                if (value) {
                    windowFrame.Show();
                } else {
                    windowFrame.Hide();
                }
            }
        }
    }
}
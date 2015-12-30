using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Atlassian.plvs.ui.jira {
    [Guid("34218DB5-88B7-4773-B356-C07E94987CD2")]
    public class IssueDetailsToolWindow : ToolWindowPane {
        private readonly IssueDetailsWindow control;

        public IssueDetailsToolWindow() :
            base(null) {
            Caption = Resources.IssueDetailsToolWindowTitle;
            BitmapResourceID = 301;
            BitmapIndex = 1;

            control = new IssueDetailsWindow(); 
        }

        public override IWin32Window Window {
            get { return control; }
        }
    }
}
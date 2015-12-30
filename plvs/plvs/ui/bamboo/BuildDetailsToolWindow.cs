using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Atlassian.plvs.ui.bamboo {
    [Guid("F9624C15-E757-4582-BF55-F2DB8146681C")]
    public class BuildDetailsToolWindow : ToolWindowPane {
       
       private readonly BuildDetailsWindow control;

       public BuildDetailsToolWindow() : base(null) {
            Caption = Resources.BuildDetailsToolWindowTitle;
            BitmapResourceID = 301;
            BitmapIndex = 4;

            control = new BuildDetailsWindow(); 
        }

        public override IWin32Window Window {
            get { return control; }
        }
    }
}

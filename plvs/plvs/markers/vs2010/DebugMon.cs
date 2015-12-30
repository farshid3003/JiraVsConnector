using System;
using System.Windows.Forms;

namespace Atlassian.plvs.markers.vs2010 {
    public partial class DebugMon : Form {

        private static DebugMon inst = new DebugMon();

        public static DebugMon Instance() {
            return inst ?? (inst = new DebugMon());
        }

        private DebugMon() {
#if SHOW_DEBUG_WINDOW
            InitializeComponent();
            TopMost = true;
            Show();
#endif
        }

        private void buttonClear_Click(object sender, EventArgs e) {
#if SHOW_DEBUG_WINDOW
            textDebug.Text = "";
#endif
        }

        public void addText(string text) {
#if SHOW_DEBUG_WINDOW
            textDebug.Text = "[" + DateTime.Now + "]: " + text + "\r\n" + textDebug.Text;
#endif
        }
    }
}

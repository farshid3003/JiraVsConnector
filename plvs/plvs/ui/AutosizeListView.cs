using System.Windows.Forms;

namespace Atlassian.plvs.ui {
    public class AutosizeListView : ListView {

        // from http://www.codeproject.com/KB/list/listviewautosize.aspx

        protected override void WndProc(ref Message m) {
            const int WM_PAINT = 0xf;

            // if the control is in details view mode and columns
            // have been added, then intercept the WM_PAINT message
            // and reset the last column width to fill the list view

            switch (m.Msg) {
                case WM_PAINT:
                    if (View == View.Details && Columns.Count > 0) {
                        Columns[Columns.Count - 1].Width = -2;
                    }
                    break;
            }
            base.WndProc(ref m);

        }
    }
}

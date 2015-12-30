using System;
using System.Windows.Forms;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs {
    public partial class ExceptionViewer : Form {

        public ExceptionViewer(string message, Exception exception) {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;

            textException.Text = PlvsUtils.getFullExceptionTextDetails(message, exception);
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void exceptionViewerKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Escape) {
                Close();
            }
        }
    }
}

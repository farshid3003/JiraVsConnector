using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Atlassian.plvs.api;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs {
    public abstract partial class AbstractTestConnection : Form {
        private readonly Server server;
        private bool testInProgress = true;
        private Thread worker;

        public abstract void testConnection();

        private Exception exception;

        private const string CONNECTION_ERROR = "Connection error. Click here for details";

        protected AbstractTestConnection(Server server) {
            this.server = server;

            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;

            Text = "Test Connection to Server \"" + server.Name + "\"";
            labelStatus.Text = "Testing connection to server " + server.Name + ", please wait...";
            buttonClose.Text = "Cancel";

            linkErrorDetails.Visible = false;
            linkErrorDetails.Enabled = false;
        }

        private void abstractTestConnectionLoad(object sender, EventArgs e) {
            worker = PlvsUtils.createThread(testConnection);
            worker.Start();
        }

        public override sealed string Text {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            stopOrClose();
        }

        private void stopOrClose() {
            if (!testInProgress) {
                Close();
            } else {
                // too brutal?
                worker.Abort();
                stopTest("Test aborted", null);
            }
        }

        protected void stopTest(string text, Exception e) {
            testInProgress = false;
            if (e != null) {
                exception = e;
                labelStatus.Text = text;
                linkErrorDetails.Text = CONNECTION_ERROR;
                linkErrorDetails.Visible = true;
                linkErrorDetails.Enabled = true;

                Graphics graphics = CreateGraphics();
                SizeF size = graphics.MeasureString(CONNECTION_ERROR, linkErrorDetails.Font);
                Bitmap errorIcon = new Bitmap((int) size.Width + 2 * 16, 16);
                using (Graphics g = Graphics.FromImage(errorIcon)) {
                    g.DrawImage(SystemIcons.Error.ToBitmap(), 0, 0, 16, 16);
                    linkErrorDetails.Image = errorIcon;
                }

                labelStatus.Visible = false;
            } else {
                labelStatus.Location = linkErrorDetails.Location;
                labelStatus.Size = linkErrorDetails.Size;
                labelStatus.Visible = true;
                labelStatus.Text = text;
            }
            progress.Visible = false;
            buttonClose.Text = "Close";
        }

        private void testJiraConnectionKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape) {
                stopOrClose();
            }
        }

        private void linkErrorDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (exception == null) return;
            PlvsUtils.showError("Failed to connect to server \"" + server.Name + "\"", exception);
        }
    }
}
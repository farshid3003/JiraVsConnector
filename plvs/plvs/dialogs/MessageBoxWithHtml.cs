using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs {
    public partial class MessageBoxWithHtml : Form {

        public const string EXCEPTION_LINK_TAG = "exceptionlink:";

        public delegate void ExceptionLinkCallback(string tag);

        private ExceptionLinkCallback callback;
        private Action copyToClipboardClicked;

        public static void showError(string title, string html, Action copyToClipboardClicked, ExceptionLinkCallback callback) {
            try {
                MessageBoxWithHtml box = new MessageBoxWithHtml {
                    Text = title,
                    labelIcon = { Image = SystemIcons.Error.ToBitmap() },
                    copyToClipboardClicked = copyToClipboardClicked,
                    callback = callback
                };
                box.webContent.DocumentText = getHtml(box.labelIcon.Font, html);
                //            box.webContent.IsWebBrowserContextMenuEnabled = true;
                box.ShowDialog();
            } catch (Exception e) {
                Debug.WriteLine("MessageBoxWithHtml.showError() - exception: " + e);
                MessageBox.Show(html, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string getHtml(Font font, string html) {
            string fontFamily = font.FontFamily.Name;
            return 
                "<html>\r\n<body style=\"margin:0;padding:0;font-family:" + fontFamily + ";font-size:12px;\">\r\n" 
                + html + "\r\n</body>\r\n</html>\r\n";
        }

        private MessageBoxWithHtml() {
            InitializeComponent();
            TopMost = true;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            Close();
        }

        private void webContent_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (e.Url.Equals("about:blank")) {
                return;
            }
            e.Cancel = true;
            string url = e.Url.ToString();
            if (url.StartsWith(EXCEPTION_LINK_TAG)) {
                if (callback != null) {
                    callback(url.Substring(EXCEPTION_LINK_TAG.Length));
                }
            } else {
                try {
                    PlvsUtils.runBrowser(url);
                    // ReSharper disable EmptyGeneralCatchClause
                } catch {
                    // ReSharper restore EmptyGeneralCatchClause
                }
            }
        }

        private void linkCopyToClipboard_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            copyToClipboardClicked();
        }

        private void messageBoxWithHtmlKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Escape) {
                Close();
            }
        }

        private void webContent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (e.KeyValue == (char)Keys.Escape) {
                Close();
            }
        }

        private void messageBoxWithHtmlLoad(object sender, EventArgs e) {
            // cancel being topmost. This should make the dialog box not hide behind other windows
            Timer t = new Timer {Interval = 500};
            t.Tick += (s, ev) => {
                          t.Stop();
                          t.Dispose();
                          TopMost = false;
                      };
            t.Start();
        }
    }
}

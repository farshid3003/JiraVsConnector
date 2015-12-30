using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs {
    public sealed partial class AutoUpdateDialog : Form {
        private readonly string updateUrl;
        private readonly string releaseNotesUrl;
        private bool pageLoaded;

        public AutoUpdateDialog(string stamp, string updateUrl, string blurbText, string releaseNotesUrl) {
            this.updateUrl = updateUrl;
            this.releaseNotesUrl = releaseNotesUrl;

            InitializeComponent();

            browser.IsWebBrowserContextMenuEnabled = false;
            browser.DocumentText = string.Format(Resources.autoupdate_html, Font.FontFamily.Name, 
                ColorTranslator.ToHtml(SystemColors.Control), stamp, blurbText);
            browser.ScrollBarsEnabled = true;

            StartPosition = FormStartPosition.CenterParent;

            buttonUpdate.Click += buttonUpdate_Click;
        }

        void buttonUpdate_Click(object sender, EventArgs e) {
            try {
                PlvsUtils.runBrowser(updateUrl);
                // ReSharper disable EmptyGeneralCatchClause
            } catch {
                // ReSharper restore EmptyGeneralCatchClause
            }
            Close();
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void browser_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (!pageLoaded) return;
            string url = e.Url.ToString();
            try {
                PlvsUtils.runBrowser(url);
// ReSharper disable EmptyGeneralCatchClause
            } catch {
// ReSharper restore EmptyGeneralCatchClause
            }
            e.Cancel = true;
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            pageLoaded = true;
        }

        private void aboutKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape) {
                Close();
            }
        }

        private void browser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (e.KeyValue == (char)Keys.Escape) {
                Close();
            }
        }

        private void buttonReleaseNotes_Click(object sender, EventArgs e) {
            try {
                PlvsUtils.runBrowser(releaseNotesUrl);
// ReSharper disable EmptyGeneralCatchClause
            } catch {
// ReSharper restore EmptyGeneralCatchClause
            }
        }
    }
}
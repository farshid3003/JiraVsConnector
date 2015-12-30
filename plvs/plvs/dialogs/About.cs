using System;
using System.Diagnostics;
using System.Windows.Forms;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs {
    public partial class About : Form {
        private bool pageLoaded;

        public About() {
            InitializeComponent();

            picture.Image = Resources.atlassian_538x235;
            browser.DocumentText = string.Format(Resources.about_html, PlvsVersionInfo.Version, PlvsVersionInfo.BuildType, PlvsVersionInfo.Stamp);
            browser.ScrollBarsEnabled = false;

            StartPosition = FormStartPosition.CenterParent;
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void browser_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (!pageLoaded) return;
            string url = e.Url.ToString();
            PlvsUtils.runBrowser(url);
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
    }
}
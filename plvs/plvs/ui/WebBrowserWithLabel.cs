using System;
using System.Windows.Forms;

namespace Atlassian.plvs.ui {
    public partial class WebBrowserWithLabel : UserControl {
        public WebBrowser Browser { get { return webContent; } }

        public string Title { get { return labelTitle.Text; } set { labelTitle.Text = value; }}

        /// <summary>
        /// Error string is a string format with two parameters. 
        /// First is font family name, second is the cause of navigation error
        /// </summary>
        public string ErrorString { get; set; }

        public WebBrowserWithLabel() {
            InitializeComponent();

            Browser.Url = new Uri("", UriKind.Relative);
        }

        private void webContent_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (e.Url.Equals("javascript:closePage()")) {
                e.Cancel = true;
                Browser.DocumentText = ErrorString != null 
                    ? string.Format(ErrorString, Font.FontFamily.Name, "") 
                    : "about:blank";
            }
        }

        private void webContent_Navigated(object sender, WebBrowserNavigatedEventArgs e) {
            if (webContent.Document != null && webContent.Document.Url != null)
                if (webContent.Document.Url.ToString().StartsWith("res://ieframe.dll/invalidcert.htm?SSLError=#")) {
                    Browser.DocumentText = ErrorString != null 
                        ? string.Format(ErrorString, Font.FontFamily.Name, "due to SSL certificate error") 
                        : "SSL certificate error";
                }
        }
    }
}

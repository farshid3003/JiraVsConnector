using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.jira {
    public partial class JiraTextAreaWithWikiPreview : UserControl {
        
        private string throbberPath;

        public JiraTextAreaWithWikiPreview() {
            IssueType = -1;
            InitializeComponent();

            throbberPath = PlvsUtils.getThroberPath();
        }

        public override string Text {
            get { return textMarkup.Text; }
            set { textMarkup.Text = value; }
        }

        public AbstractJiraServerFacade Facade { get; set; }

        public JiraServer Server { get; set; }

        public JiraIssue Issue { get; set; }

        public JiraProject Project { get; set; }

        public int IssueType { get; set; }

        private void tabContents_Selected(object sender, TabControlEventArgs e) {
            if (e.TabPage != tabPreview) return;
            if (textMarkup.Text.Length == 0) {
                webPreview.DocumentText = "";
                return;
            }
            webPreview.DocumentText = PlvsUtils.getThrobberHtml(throbberPath, "Fetching preview...");
            Thread t = PlvsUtils.createThread(() => getMarkup(textMarkup.Text));
            t.Start();
        }

        private void getMarkup(string text) {
            if (Facade == null || Issue == null && !(Server != null && Project != null && IssueType > -1)) {
                Invoke(new MethodInvoker(delegate {
                                             webPreview.DocumentText =
                                                 "<html><head>" + Resources.summary_and_description_css
                                                 + "</head><body class=\"summary\">Unable to render preview</body></html>";
                                         }));
                return;
            }
            try {
                string renderedContent = Issue != null 
                    ? Facade.getRenderedContent(Issue, text) 
                    : Facade.getRenderedContent(Server, IssueType, Project, text);
                Invoke(new MethodInvoker(delegate {
                                             webPreview.DocumentText = 
                                                 "<html><head>" + Resources.summary_and_description_css
                                                 + "</head><body class=\"summary\">" + renderedContent + "</body></html>";
                                         }));
            } catch (Exception e) {
                // just log the problem. This is an informational functionality only, 
                // let's not make a big deal out of errors here
                Debug.WriteLine("JiraTextAreaWithWikiPreview.getMarkup() - exception: " + e.Message);
            }
        }

        private void textMarkup_TextChanged(object sender, EventArgs e) {
            if (MarkupTextChanged != null) {
                MarkupTextChanged(this, new EventArgs());
            }
        }

        public event EventHandler<EventArgs> MarkupTextChanged;

        private void webPreview_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (e.Url.Equals("about:blank")) {
                return;
            }
            try {
                e.Cancel = true;
                PlvsUtils.runBrowser(e.Url.ToString());
            } catch (Exception ex) {
                Debug.WriteLine("JiraTextAreaWithWikiPreview.webPreview_Navigating() - exception: " + ex.Message);
            }
        }
    }
}

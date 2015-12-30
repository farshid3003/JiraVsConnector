using System;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Atlassian.plvs.util;
using EnvDTE;

namespace Atlassian.plvs.dialogs {
    public partial class UnhandledExceptionDialog : Form {
        public UnhandledExceptionDialog(Exception e) {
            InitializeComponent();

            textException.Text =
                "Exception type: " + e.GetType()
                + "\r\nException Message: " + e.Message
                + "\r\nStack Trace:\r\n" + e.StackTrace;
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void buttonReportBug_Click(object sender, EventArgs e) {
            try {
                StringBuilder url = new StringBuilder("https://ecosystem.atlassian.net/secure/CreateIssueDetails!init.jspa?pid=13773&issuetype=1");
                url.Append("&environment=");
                StringBuilder env = new StringBuilder();
                env.Append("connector version: ").Append(PlvsVersionInfo.VersionAndStamp);
                DTE dte = PlvsUtils.Dte;
                if (dte != null) {
                    env.Append("\nVisual Studio version: ").Append(dte.Version).Append(", ").Append(dte.Edition);
                }
                env.Append("\nOperating System: ").Append(Environment.OSVersion);
                env.Append("\nCPU count: ").Append(Environment.ProcessorCount);
                url.Append(HttpUtility.UrlEncode(env.ToString()));
                url.Append("&description=").Append(HttpUtility.UrlEncode(textException.Text));
                PlvsUtils.runBrowser(url.ToString());
                // ReSharper disable EmptyGeneralCatchClause
            } catch (Exception) {
                // ReSharper restore EmptyGeneralCatchClause
            }

        }
    }
}

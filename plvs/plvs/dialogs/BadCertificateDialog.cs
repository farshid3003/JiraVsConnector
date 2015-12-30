using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace Atlassian.plvs.dialogs {
    public partial class BadCertificateDialog : Form {
        private readonly X509Certificate2 cert;

        public BadCertificateDialog(object sender, X509Certificate2 cert) {
            this.cert = cert;
            InitializeComponent();

            labelWarning1.Text = "Warning! You have attempted to connect to";
            labelWarning3.Text = " but the server certificate is not trusted. Do you want to continue?";
            HttpWebRequest req = sender as HttpWebRequest;
            labelWarning2.Text = req != null ? req.Address.ToString() : sender.ToString();

            labelSubject.Text = cert.SubjectName.Name;
            labelIssuer.Text = cert.IssuerName.Name;
            labelValidFrom.Text = cert.NotBefore.ToShortDateString() + " " + cert.NotBefore.ToShortTimeString();
            labelValidUntil.Text = cert.NotAfter.ToShortDateString() + " " + cert.NotAfter.ToShortTimeString();

            StartPosition = FormStartPosition.CenterParent;
        }

        private void linkCertificateDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            X509Certificate2UI.DisplayCertificate(cert, Handle);
        }
    }
}

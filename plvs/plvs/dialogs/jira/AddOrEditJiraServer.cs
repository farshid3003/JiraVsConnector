using System;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;

namespace Atlassian.plvs.dialogs.jira {
    public partial class AddOrEditJiraServer : Form {
        private readonly AbstractJiraServerFacade facade;
        private static int invocations;

        private readonly bool editing;

        private readonly JiraServer server;

        public AddOrEditJiraServer(JiraServer server, AbstractJiraServerFacade facade) {
            this.facade = facade;
            InitializeComponent();

            editing = server != null;

            this.server = new JiraServer(server);

            Text = editing ? "Edit JIRA Server" : "Add JIRA Server";
            buttonAddOrEdit.Text = editing ? "Apply Changes" : "Add Server";

            if (editing) {
                if (server != null) {
                    name.Text = server.Name;
                    url.Text = server.Url;
                    user.Text = server.UserName;
                    password.Text = server.Password;
                    checkEnabled.Checked = server.Enabled;
                    checkShared.Checked = server.IsShared;
                    checkDontUseProxy.Checked = server.NoProxy;
                    checkUseOldskoolAuth.Checked = server.OldSkoolAuth;
                }
            }
            else {
                ++invocations;
                name.Text = "JIRA Server #" + invocations;
                buttonAddOrEdit.Enabled = false;
                checkEnabled.Checked = true;
                checkShared.Checked = false;
                checkDontUseProxy.Checked = false;
                checkUseOldskoolAuth.Checked = false;
            }

            StartPosition = FormStartPosition.CenterParent;
        }

        public override sealed string Text {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void buttonAddOrEdit_Click(object sender, EventArgs e) {
            fillServerData();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void fillServerData() {
            server.Name = name.Text.Trim();
            string fixedUrl = url.Text.Trim();
            if (!(fixedUrl.StartsWith("http://") || fixedUrl.StartsWith("https://"))) {
                fixedUrl = "http://" + fixedUrl;
            }
            if (fixedUrl.EndsWith("/")) {
                fixedUrl = fixedUrl.Substring(0, fixedUrl.Length - 1);
            }
            server.Url = fixedUrl;
            server.UserName = user.Text.Trim();
            server.Password = password.Text;
            server.Enabled = checkEnabled.Checked;
            server.IsShared = checkShared.Checked;
            server.NoProxy = checkDontUseProxy.Checked;
            server.OldSkoolAuth = checkUseOldskoolAuth.Checked;
        }

        private void name_TextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void url_TextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void user_TextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void checkIfValid() {
            buttonAddOrEdit.Enabled = name.Text.Trim().Length > 0 && url.Text.Trim().Length > 0 &&
                                      user.Text.Trim().Length > 0;
            buttonTestConnection.Enabled = buttonAddOrEdit.Enabled;
        }

        public JiraServer Server {
            get { return server; }
        }

        private void addOrEditJiraServerKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Escape) return;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void checkEnabled_CheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void buttonTestConnection_Click(object sender, EventArgs e) {
            fillServerData();
            new TestJiraConnection(facade, server).ShowDialog();
        }

        private void checkDontUseProxy_CheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void checkShared_CheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void checkUseOldskoolAuth_CheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }
    }
}
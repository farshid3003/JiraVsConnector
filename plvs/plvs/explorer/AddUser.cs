using System;
using System.Drawing;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.explorer {
    public partial class AddUser : Form {
        private readonly JiraServer server;

        public AddUser(JiraServer server) {
            this.server = server;
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;

            buttonOk.Enabled = false;
            labelUserExists.Visible = false;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void textUserId_TextChanged(object sender, EventArgs e) {
            updateOkButton();
        }

        private void updateOkButton() {
            if (textUserId.Text.Length == 0) {
                buttonOk.Enabled = false;
                labelUserExists.Visible = false;
                return;
            }
            bool haveUser = JiraServerCache.Instance.getUsers(server).haveUser(textUserId.Text);
            textUserId.ForeColor = haveUser ? Color.Red : SystemColors.ControlText;
            buttonOk.Enabled = !haveUser;
            labelUserExists.Visible = haveUser;
        }

        public JiraUser Value {
            get { return new JiraUser(textUserId.Text, null); }
        }

        public bool OpenDropZone { get { return checkOpenDropZone.Checked; } }

        private void AddUser_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Escape) return;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

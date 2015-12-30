using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira {
    public partial class JiraUserPicker : UserControl {

        private JiraServer jiraServer;

        public JiraUserPicker() {
            InitializeComponent();
        }

        public void init(JiraServer server, string currentUserId, bool showAssignToMe) {
            
            jiraServer = server;

            ICollection<JiraUser> users = JiraServerCache.Instance.getUsers(server).getAllUsers();

            JiraUser selected = null;

            foreach (JiraUser user in users) {
                if (currentUserId != null && currentUserId.Equals(user.Id)) {
                    selected = user;
                }
                comboUsers.Items.Add(user);
            }

            if (selected != null) {
                comboUsers.SelectedItem = selected;
            }

            checkAssignToMe.Visible = showAssignToMe;
            checkAssignToMe.Enabled = showAssignToMe;
            if (!showAssignToMe) {
                Height -= checkAssignToMe.Height + 10;
            }
        }

        public string Value { 
            get {
                if (checkAssignToMe.Enabled && checkAssignToMe.Checked) {
                    return CredentialUtils.getUserNameWithoutDomain(jiraServer.UserName);
                }
                if (!(comboUsers.SelectedItem is JiraUser)) {
                    return comboUsers.Text;
                }
                return ((JiraUser) comboUsers.SelectedItem).Id;        
            }
        }

        private void checkAssignToMe_CheckedChanged(object sender, System.EventArgs e) {
            comboUsers.Enabled = !checkAssignToMe.Checked;
        }
    }
}

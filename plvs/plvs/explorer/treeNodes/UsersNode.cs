using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    sealed class UsersNode : AbstractNavigableTreeNodeWithServer {
        private bool usersLoaded;

        private readonly List<ToolStripItem> menuItems = new List<ToolStripItem>();

        public UsersNode(JiraIssueListModel model, AbstractJiraServerFacade facade, JiraServer server)
            : base(model, facade, server, "Users", 0) {

            ContextMenuStrip = new ContextMenuStrip();

            menuItems.Add(new ToolStripMenuItem("Add new user...", null, addUser));

            ContextMenuStrip.Items.Add("dummy");

            ContextMenuStrip.Items.AddRange(MenuItems.ToArray());

            ContextMenuStrip.Opening += contextMenuStripOpening;
            ContextMenuStrip.Opened += contextMenuStripOpened;
        }

        public override List<ToolStripItem> MenuItems { get { return menuItems; } }

        private void contextMenuStripOpened(object sender, EventArgs e) {
            ContextMenuStrip.Items.Clear();
            foreach (ToolStripItem item in MenuItems) {
                ContextMenuStrip.Items.Add(item);
            }
        }

        private static void contextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = false;
        }

        public override string getUrl(string authString) {
            return Server.Url + "?" + authString;
        }

        public override void onClick(StatusLabel status) {
            if (usersLoaded) return;
            usersLoaded = true;
            loadUsers();
        }

        private void loadUsers() {
            JiraUserCache userCache = JiraServerCache.Instance.getUsers(Server);
            foreach (JiraUser user in userCache.getAllUsers()) {
                Nodes.Add(new UserNode(Model, Facade, Server, user));
            }
            ExpandAll();
        }

        private void addUser(object sender, EventArgs e) {
            AddUser dlg = new AddUser(Server);
            DialogResult result = dlg.ShowDialog();
            
            if (!result.Equals(DialogResult.OK)) return;

            JiraUser user = dlg.Value;
            JiraServerCache.Instance.getUsers(Server).putUser(user);
            Nodes.Add(new UserNode(Model, Facade, Server, user));

            if (dlg.OpenDropZone) {
                DropZone.showDropZoneFor(Model, Server, Facade, new AssigneeDropZoneWorker(Facade, Server, user));
            }
        }
    }
}

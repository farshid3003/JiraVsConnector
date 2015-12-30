using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    sealed class UserNode : AbstractNavigableTreeNodeWithServer {
        private readonly JiraUser user;

        private readonly AssigneeDropZoneWorker worker;

        private readonly List<ToolStripItem> menuItems = new List<ToolStripItem>();

        public UserNode(JiraIssueListModel model, AbstractJiraServerFacade facade, JiraServer server, JiraUser user)
            : base(model, facade, server, user.ToString(), 0) {

            this.user = user;

            ContextMenuStrip = new ContextMenuStrip();

            worker = new AssigneeDropZoneWorker(Facade, Server, user);

            menuItems.Add(new ToolStripMenuItem("Open \"Assign to " + user + "\" Drop Zone", null, createDropZone));

            ContextMenuStrip.Items.Add("dummy");

            ContextMenuStrip.Items.AddRange(MenuItems.ToArray());

            ContextMenuStrip.Opening += contextMenuStripOpening;
            ContextMenuStrip.Opened += contextMenuStripOpened;
        }

        public override List<ToolStripItem> MenuItems { get { return menuItems; } }

        public override string getUrl(string authString) {
            return Server.Url + "/secure/ViewProfile.jspa?name=" + user.Id + "&" + authString; 
        }

        public override void onClick(StatusLabel status) { }

        private void contextMenuStripOpened(object sender, EventArgs e) {
            ContextMenuStrip.Items.Clear();
            foreach (ToolStripItem item in MenuItems) {
                ContextMenuStrip.Items.Add(item);
            }
        }

        private static void contextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = false;
        }

        private void createDropZone(object sender, EventArgs e) {
            DropZone.showDropZoneFor(Model, Server, Facade, worker);
        }
    }
}

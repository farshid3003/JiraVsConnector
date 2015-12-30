using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    sealed class PriorityNode : AbstractNavigableTreeNodeWithServer, DropZone.DropZoneWorker {
        private readonly JiraNamedEntity priority;

        private readonly List<ToolStripItem> menuItems = new List<ToolStripItem>();

        public PriorityNode(JiraIssueListModel model, AbstractJiraServerFacade facade, JiraServer server, JiraNamedEntity priority)
            : base(model, facade, server, priority.Name, 0) {
            this.priority = priority;

            ContextMenuStrip = new ContextMenuStrip();

            menuItems.Add(new ToolStripMenuItem(
                "Open \"Set Priority to " + priority.Name + "\" Drop Zone", 
                ImageCache.Instance.getImage(server, priority.IconUrl).Img, createDropZone));

            ContextMenuStrip.Items.Add("dummy");

            ContextMenuStrip.Items.AddRange(MenuItems.ToArray());

            ContextMenuStrip.Opening += contextMenuStripOpening;
            ContextMenuStrip.Opened += contextMenuStripOpened;

        }

        public override List<ToolStripItem> MenuItems { get { return menuItems; } }

        public override string getUrl(string authString) {
            return Server.Url + "?" + authString; 
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
            DropZone.showDropZoneFor(Model, Server, Facade, this);
        }

        public DropZone.PerformAction Action { get { return dropAction; } }

        private void dropAction(JiraIssue issue, bool add) {
            JiraField field = new JiraField("priority", null) { Values = new List<string>() };
            if (add) {
                throw new ArgumentException("Unable to have multiple priorities in one issue");
            }

            // skip if issue already has this priority
            if (field.Values.Contains(priority.Id.ToString())) return;

            field.Values.Add(priority.Id.ToString());
            Facade.updateIssue(issue, new List<JiraField> { field });
        }

        public string ZoneName { get { return "Priority: " + priority.Name; } }

        public string ZoneKey { get { return Server.GUID + "_priority_" + priority.Id; } }

        public bool CanAdd { get { return false; } }

        public string IssueWillBeAddedText { get { return "Unavailable"; } }

        public string issueWillBeMovedText { get { return "Priority " + priority.Name + " will be set as priority for issue"; } }

        public string InitialText { get { return "Drag issues here to set their priority to \"" + priority.Name + "\""; } }
    }
}

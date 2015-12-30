using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    sealed class ComponentNode : AbstractNavigableTreeNodeWithServer, DropZone.DropZoneWorker {
        private readonly JiraProject project;
        private readonly JiraNamedEntity comp;

        private readonly List<ToolStripItem> menuItems = new List<ToolStripItem>();

        public ComponentNode(JiraIssueListModel model, AbstractJiraServerFacade facade, JiraServer server, JiraProject project, JiraNamedEntity comp)
            : base(model, facade, server, comp.Name, 0) {
            this.project = project;
            this.comp = comp;

            ContextMenuStrip = new ContextMenuStrip();

            menuItems.Add(new ToolStripMenuItem("Open \"Set Component to " + comp.Name + "\" Drop Zone", null, createDropZone));

            ContextMenuStrip.Items.Add("dummy");

            ContextMenuStrip.Items.AddRange(MenuItems.ToArray());

            ContextMenuStrip.Opening += contextMenuStripOpening;
            ContextMenuStrip.Opened += contextMenuStripOpened;

        }

        public override List<ToolStripItem> MenuItems { get { return menuItems; } }

        public override string getUrl(string authString) {
            return Server.Url + "/browse/" + project.Key + "/component/" + comp.Id + "?" + authString; 
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
            JiraField componentsField = new JiraField("components", null) { Values = new List<string>() };
            if (add) {
                List<JiraNamedEntity> projectComponents = Facade.getComponents(issue.Server, project);
                ICollection<string> issueComponents = issue.Components;
                foreach (string issueComponent in issueComponents) {
                    foreach (JiraNamedEntity ver in projectComponents) {
                        if (!ver.Name.Equals(issueComponent)) continue;
                        componentsField.Values.Add(ver.Id.ToString());
                        break;
                    }
                }
            }

            // skip if issue already has this component
            if (componentsField.Values.Contains(comp.Id.ToString())) return;

            componentsField.Values.Add(comp.Id.ToString());
            Facade.updateIssue(issue, new List<JiraField> { componentsField });
        }

        public string ZoneName { get { return "Component: " + comp.Name; } }

        public string ZoneKey { get { return Server.GUID + "_project_" + project.Key + "_component_" + comp.Id; } }

        public bool CanAdd { get { return true; } }

        public string IssueWillBeAddedText { get { return "Component " + comp.Name + " will be added to components for issue"; } }

        public string issueWillBeMovedText { get { return "Component " + comp.Name + " will be set as component for issue"; } }

        public string InitialText { 
            get {
                return "Drag issues here to set their component to \"" + comp.Name + "\", Ctrl-drag to add to the list of components";
            } 
        }
    }
}

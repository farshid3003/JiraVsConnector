using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    sealed class VersionNode : AbstractNavigableTreeNodeWithServer {
        private readonly JiraProject project;
        private readonly JiraNamedEntity version;
        private readonly List<ToolStripItem> menuItems = new List<ToolStripItem>();

        public VersionNode(JiraIssueListModel model, AbstractJiraServerFacade facade, JiraServer server, JiraProject project, JiraNamedEntity version)
            : base(model, facade, server, version.Name, 0) {
            this.project = project;
            this.version = version;

            ContextMenuStrip = new ContextMenuStrip();

            menuItems.Add(new ToolStripMenuItem("Open \"Set Fix For " + version.Name + "\" Drop Zone", null, createFixForDropZone));
            menuItems.Add(new ToolStripMenuItem("Open \"Set Affects " + version.Name + "\" Drop Zone", null, createAffectsDropZone));

            ContextMenuStrip.Items.Add("dummy");

            ContextMenuStrip.Items.AddRange(MenuItems.ToArray());

            ContextMenuStrip.Opening += contextMenuStripOpening;
            ContextMenuStrip.Opened += contextMenuStripOpened;
        }

        private void contextMenuStripOpened(object sender, EventArgs e) {
            ContextMenuStrip.Items.Clear();
            foreach (ToolStripItem item in MenuItems) {
                ContextMenuStrip.Items.Add(item);
            }
        }

        private static void contextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = false;
        }

        public override List<ToolStripItem> MenuItems { get { return menuItems; } }

        private void createAffectsDropZone(object sender, EventArgs e) {
            DropZone.showDropZoneFor(Model, Server, Facade, new AffectsDropZoneWorker(this));
        }

        private void createFixForDropZone(object sender, EventArgs e) {
            DropZone.showDropZoneFor(Model, Server, Facade, new FixForDropZoneWorker(this));
        }

        public override string getUrl(string authString) {
            return Server.Url + "/browse/" + project.Key + "/fixforversion/" + version.Id + "?" + authString; 
        }

        public override void onClick(StatusLabel status) { }

        private abstract class AbstractVersionDropZoneWorker : DropZone.DropZoneWorker {
            protected VersionNode Parent { get; private set; }

            protected AbstractVersionDropZoneWorker(VersionNode parent) {
                Parent = parent;
            }

            public DropZone.PerformAction Action { get { return dropAction; } }

            protected abstract string FieldName { get; }

            protected abstract ICollection<string> getIssueVersions(JiraIssue issue);

            private void dropAction(JiraIssue issue, bool add) {
                JiraField versionField = new JiraField(FieldName, null) {Values = new List<string>()};
                if (add) {
                    List<JiraNamedEntity> projectVersions = Parent.Facade.getVersions(issue.Server, Parent.project);
                    ICollection<string> issueVersions = getIssueVersions(issue);
                    foreach (string issueVersion in issueVersions) {
                        foreach (JiraNamedEntity ver in projectVersions) {
                            if (!ver.Name.Equals(issueVersion)) continue;
                            versionField.Values.Add(ver.Id.ToString());
                            break;
                        }
                    }
                }

                // skip if issue already has this version
                if (versionField.Values.Contains(Parent.version.Id.ToString())) return;

                versionField.Values.Add(Parent.version.Id.ToString());
                Parent.Facade.updateIssue(issue, new List<JiraField> { versionField });
            }

            public bool CanAdd { get { return true; } }
            public abstract string IssueWillBeAddedText { get; }
            public abstract string issueWillBeMovedText { get; }
            public abstract string InitialText { get; }
            public abstract string ZoneKey { get; }
            public abstract string ZoneName { get; }
        }

        private class FixForDropZoneWorker : AbstractVersionDropZoneWorker {
            public FixForDropZoneWorker(VersionNode parent) : base(parent) {}
            protected override string FieldName { get { return "fixVersions"; } }
            public override string ZoneName { get { return "Fix For Version: " + Parent.version.Name; } }

            public override string IssueWillBeAddedText {
                get { return "Version " + Parent.version.Name + " will be added to fix versions for issue"; }
            }

            public override string issueWillBeMovedText {
                get { return "Version " + Parent.version.Name + " will be set as fix version for issue"; }
            }

            public override string InitialText {
                get {
                    return "Drag issues here to set their fix version to \"" + Parent.version.Name 
                        + "\", Ctrl-drag to add to the list of fix versions";
                }
            }

            public override string ZoneKey { get { return Parent.Server.GUID + "_project_" + Parent.project.Key + "_fixversion_" + Parent.version.Id; } }

            protected override ICollection<string> getIssueVersions(JiraIssue issue) { return issue.FixVersions; }
        }

        private class AffectsDropZoneWorker : AbstractVersionDropZoneWorker {
            public AffectsDropZoneWorker(VersionNode parent) : base(parent) { }
            protected override string FieldName { get { return "versions"; } }
            public override string ZoneName { get { return "Affects Version: " + Parent.version.Name; } }

            public override string IssueWillBeAddedText {
                get { return "Version " + Parent.version.Name + " will be added to affect versions for issue"; }
            }

            public override string issueWillBeMovedText {
                get { return "Version " + Parent.version.Name + " will be set as affect version for issue"; }
            }

            public override string InitialText { 
                get { 
                    return "Drag issues here to set their affect version to \"" + Parent.version.Name 
                        + "\", Ctrl-drag to add to the list of affect versions"; 
                }
            }

            public override string ZoneKey { get { return Parent.Server.GUID + "_project_" + Parent.project.Key + "_affectsversion_" + Parent.version.Id; } }

            protected override ICollection<string> getIssueVersions(JiraIssue issue) { return issue.Versions; }
        }
    }
}

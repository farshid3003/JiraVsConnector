using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.explorer.treeNodes;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.util;

namespace Atlassian.plvs.explorer {
    public sealed partial class JiraServerExplorer : Form {
        private readonly JiraIssueListModel model;
        private readonly JiraServer server;
        private readonly AbstractJiraServerFacade facade;

        private readonly StatusLabel status;

        private static readonly Dictionary<string, JiraServerExplorer> activeExplorers = new Dictionary<string, JiraServerExplorer>();

        public static void showJiraServerExplorerFor(JiraIssueListModel model, JiraServer server, AbstractJiraServerFacade facade) {
            if (activeExplorers.ContainsKey(server.GUID.ToString())) {
                activeExplorers[server.GUID.ToString()].BringToFront();
            } else {
                new JiraServerExplorer(model, server, facade).Show();
            }
        }

        private JiraServerExplorer(JiraIssueListModel model, JiraServer server, AbstractJiraServerFacade facade) {
            this.model = model;
            this.server = server;
            this.facade = facade;

            InitializeComponent();

            status = new StatusLabel(statusStrip, labelPath);

            Text = "JIRA Server Explorer: " + server.Name + " (" + server.Url + ")";

            StartPosition = FormStartPosition.CenterParent;

            dropDownActions.Enabled = false;

            dropDownActions.DropDownItems.Add("phony");

            webJira.Title = "";
            webJira.ErrorString = Resources.explorer_navigator_error_html;


        }

        public static void closeAll() {
            List<JiraServerExplorer> l = new List<JiraServerExplorer>(activeExplorers.Values);
            foreach (JiraServerExplorer explorer in l) {
                explorer.Close();
            }
        }

        protected override void OnLoad(EventArgs e) {

            activeExplorers[server.GUID.ToString()] = this;

            webJira.Browser.Navigate(server.Url + "?" + CredentialUtils.getOsAuthString(server));

            treeJira.Nodes.Add(new PrioritiesNode(this, model, facade, server));
            treeJira.Nodes.Add(new UsersNode(model, facade, server));
            treeJira.Nodes.Add(new ProjectsNode(this, model, facade, server));

            treeJira.SelectedNode = null;
        }

        private void treeJira_AfterSelect(object sender, TreeViewEventArgs e) {
            AbstractNavigableTreeNodeWithServer node = treeJira.SelectedNode as AbstractNavigableTreeNodeWithServer;
            dropDownActions.DropDownItems.Add("phony");
            if (node != null) {
                node.onClick(status);
                string url = node.getUrl(CredentialUtils.getOsAuthString(server));
                webJira.Browser.Navigate(url);

                ICollection<ToolStripItem> menuItems = node.MenuItems;
                dropDownActions.Enabled = menuItems != null && menuItems.Count > 0;
            } else {
                dropDownActions.Enabled = false;
            }
        }

        private void jiraServerExplorerFormClosed(object sender, FormClosedEventArgs e) {
            activeExplorers.Remove(server.GUID.ToString());
        }

        //
        // this is a stupid trick that prevents the first node to be selected 
        // (and hence - expanded) on first loading of the form.
        // Beats me why TreeView behaves like that - go complain to Microsoft
        //
        private bool firstSelect = true;
        private void treeJira_BeforeSelect(object sender, TreeViewCancelEventArgs e) {
            if (!firstSelect) return;
            firstSelect = false;
            e.Cancel = true;
        }

        private void dropDownActions_DropDownOpened(object sender, EventArgs e) {
            AbstractNavigableTreeNodeWithServer node = treeJira.SelectedNode as AbstractNavigableTreeNodeWithServer;
            if (node == null) return;

            ICollection<ToolStripItem> menuItems = node.MenuItems;
            if (menuItems == null) return;

            dropDownActions.DropDownItems.Clear();
            foreach (ToolStripItem item in menuItems) {
                dropDownActions.DropDownItems.Add(item);
            }
            PlvsUtils.addPhonyMenuItemFixingPlvs109(dropDownActions);
        }
    }
}

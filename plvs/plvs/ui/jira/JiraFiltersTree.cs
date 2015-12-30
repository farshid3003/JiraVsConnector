using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.gh;
using Atlassian.plvs.dialogs.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.models.jira.presetFilters;
using Atlassian.plvs.store;
using Atlassian.plvs.ui.jira.issuefilternodes;
using Atlassian.plvs.ui.jira.issues.menus;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.jira {
    public sealed class JiraFiltersTree : TreeView {

        private const string FILTERS_TREE_RECENT_PARAM = "JiraFilterTree.recentlyViewed";
        private const string FILTERS_TREE_SERVER_PARAM = "JiraFilterTree.server";
        private const string FILTERS_TREE_FILTER_GROUP_PARAM = "JiraFilterTree.filter.group";
        private const string FILTERS_TREE_FILTER_PARAM = "JiraFilterTree.filter";

        private const string PRESET_FILTER_GROUP_NAME = "PRESETS";
        private const string CUSTOM_FILTER_GROUP_NAME = "CUSTOM";
        private const string SAVED_FILTER_GROUP_NAME = "SAVED";
        private const string GH_SPRINT_GROUP_NAME = "GH";

        private readonly ImageList filterTreeImages = new ImageList();
        private ToolTip filtersTreeToolTip;
        private Action reloadIssues;
        private JiraServerModel model;

        public TreeNodeCollapseExpandStatusManager CollapseExpandManager { get; set; }

        public JiraFiltersTree() {
            initImageList();
            AfterCollapse += jiraFiltersTreeAfterCollapse;
            AfterExpand += jiraFiltersTreeAfterExpand;
        }

        public bool RecentlyViewedSelected {
            get { return SelectedNode is RecentlyOpenIssuesTreeNode; }
        }

        public bool FilterOrRecentlyViewedSelected {
            get { return SelectedNode != null 
                         && (SelectedNode is JiraSavedFilterTreeNode
                             || SelectedNode is GhSprintTreeNode
                             || SelectedNode is RecentlyOpenIssuesTreeNode
                             || SelectedNode is JiraCustomFilterTreeNode
                             || SelectedNode is JiraPresetFilterTreeNode);
            }
        }

        public bool NodeWithServerSelected {
            get { return SelectedNode != null && SelectedNode is TreeNodeWithJiraServer; }
        }

        private void initImageList() {
            filterTreeImages.Images.Clear();

            filterTreeImages.Images.Add(Resources.jira_blue_16);
            filterTreeImages.Images.Add(Resources.ico_jira_filter);
            filterTreeImages.Images.Add(Resources.ico_jira_saved_filter);
            filterTreeImages.Images.Add(Resources.ico_jira_custom_filter);
            filterTreeImages.Images.Add(Resources.ico_jira_preset_filter);
            filterTreeImages.Images.Add(Resources.ico_jira_recent_issues);
            filterTreeImages.Images.Add(Resources.ico_jira_filter);
            filterTreeImages.Images.Add(Resources.kanban16);
            filterTreeImages.Images.Add(Resources.scrum16);

            ImageList = filterTreeImages;
        }

        public void clear() {
            Nodes.Clear();
        }

        public void addServerNodes(List<JiraServer> servers) {
            if (model == null) return;
            foreach (JiraServer server in servers) {
                Nodes.Add(new JiraServerTreeNode(model, server, 0));
            }
        }

        public void addRecentlyViewedNode() {
            Nodes.Add(new RecentlyOpenIssuesTreeNode(5));
        }

        private JiraServerTreeNode findServerNode(JiraServer server) {
            foreach (TreeNode node in Nodes) {
                JiraServerTreeNode tn = node as JiraServerTreeNode;
                if (tn == null) continue;
                if (tn.Server.GUID.Equals(server.GUID)) {
                    return tn;
                }
            }
            return null;
        }

        public void getAndCastSelectedNode(
            out JiraSavedFilterTreeNode saved, out RecentlyOpenIssuesTreeNode recent, 
            out JiraCustomFilterTreeNode custom, out JiraPresetFilterTreeNode preset, 
            out GhSprintTreeNode sprint) {

            saved = SelectedNode as JiraSavedFilterTreeNode;
            recent = SelectedNode as RecentlyOpenIssuesTreeNode;
            custom = SelectedNode as JiraCustomFilterTreeNode;
            preset = SelectedNode as JiraPresetFilterTreeNode;
            sprint = SelectedNode as GhSprintTreeNode;
        }

        public TreeNodeWithJiraServer findGroupNode(JiraServer server, Type type) {
            JiraServerTreeNode serverNode = findServerNode(server);
            if (serverNode == null) return null;
            foreach (TreeNode groupNode in serverNode.Nodes) {
                if (type.IsAssignableFrom(groupNode.GetType())) {
                    return (TreeNodeWithJiraServer)groupNode;
                }
            }
            return null;
        }

        public JiraServer CurrentlySelectedServer { get {
                TreeNodeWithJiraServer node = SelectedNode as TreeNodeWithJiraServer;
                return node == null ? null : node.Server;
            }
        }

        public void addGhNodes(JiraServer server) {
            var node = findServerNode(server);
            if (node == null) return;

            var groupTreeNode = new GhGroupTreeNode(server, 6);
            node.Nodes.Add(groupTreeNode);
            foreach (var rapidBoard in JiraServerCache.Instance.getGhBoards(server).Values) {
                if (rapidBoard.Sprints == null || rapidBoard.Sprints.Count == 0) continue;
                var boardTreeNode = new GhBoardTreeNode(server, rapidBoard, 7);
                groupTreeNode.Nodes.Add(boardTreeNode);
                foreach (var sprint in rapidBoard.Sprints) {
                    boardTreeNode.Nodes.Add(new GhSprintTreeNode(server, sprint, 8));
                }
            }
        }

        public void addSavedFilterNodes(JiraServer server, IEnumerable<JiraSavedFilter> filters) {
            TreeNodeWithJiraServer node = findGroupNode(server, typeof(JiraSavedFiltersGroupTreeNode));
            if (node == null) {
                return;
            }
            foreach (JiraSavedFilter filter in filters) {
                node.Nodes.Add(new JiraSavedFilterTreeNode(server, filter, 1, new SavedFilterContextMenu(filter)));
            }
        }

        public void addCustomFilterNodes(JiraServer server) {
            TreeNodeWithJiraServer node = findGroupNode(server, typeof(JiraCustomFiltersGroupTreeNode));
            if (node == null) {
                return;
            }
            foreach (JiraCustomFilter filter in JiraCustomFilter.getAll(server)) {
                addCustomFilterTreeNode(server, node, filter);
            }
        }

        private JiraCustomFilterTreeNode addCustomFilterTreeNode(JiraServer server, TreeNode node, JiraCustomFilter filter) {
            JiraCustomFilterTreeNode cfNode = new JiraCustomFilterTreeNode(server, filter, 1);
            cfNode.ContextMenuStrip = new CustomFilterContextMenu(server, cfNode, editCustomFilter, removeCustomFilter);

            node.Nodes.Add(cfNode);
            return cfNode;
        }

        public void addCustomFilter(TreeNodeWithJiraServer node) {
            if (node == null) {
                return;
            }
            JiraCustomFilter newFilter = new JiraCustomFilter(node.Server);
            EditCustomFilter ecf = new EditCustomFilter(node.Server, newFilter, false);
            ecf.ShowDialog();
            if (!ecf.Changed) return;
            JiraCustomFilter.add(newFilter);
            JiraCustomFilterTreeNode newNode = addCustomFilterTreeNode(node.Server, node, newFilter);
            SelectedNode = newNode;
        }

        public void addCustomFilter() {
            JiraServer server = CurrentlySelectedServer;
            if (server == null) {
                return;
            }
            TreeNodeWithJiraServer node = findGroupNode(server, typeof(JiraCustomFiltersGroupTreeNode));
            addCustomFilter(node);
        }

        public void addFilterGroupNodes(JiraServer server) {
            JiraServerTreeNode node = findServerNode(server);
            if (node == null) return;

            JiraPresetFiltersGroupTreeNode presetFiltersGroupTreeNode = new JiraPresetFiltersGroupTreeNode(server, 4);
            presetFiltersGroupTreeNode.ContextMenuStrip = new PresetFilterGroupContextMenu(presetFiltersGroupTreeNode,
                                                                                           setAllPresetFiltersProject,
                                                                                           clearAllPresetFiltersProject);
            node.Nodes.Add(presetFiltersGroupTreeNode);
            node.Nodes.Add(new JiraSavedFiltersGroupTreeNode(server, 2));
            JiraCustomFiltersGroupTreeNode customFiltersGroupTreeNode = new JiraCustomFiltersGroupTreeNode(server, 3);
            customFiltersGroupTreeNode.ContextMenuStrip = new CustomFilterGroupContextMenu(customFiltersGroupTreeNode, addCustomFilter);
            node.Nodes.Add(customFiltersGroupTreeNode);
        }

        public void addPresetFilterNodes(JiraServer server) {
            TreeNodeWithJiraServer node = findGroupNode(server, typeof(JiraPresetFiltersGroupTreeNode));
            if (node == null) {
                return;
            }
            node.Nodes.Add(buildPresetFilterNode(server, new JiraPresetFilterUnscheduled(server)));
            node.Nodes.Add(buildPresetFilterNode(server, new JiraPresetFilterOutstanding(server)));
            node.Nodes.Add(buildPresetFilterNode(server, new JiraPresetFilterAssignedToMe(server)));
            node.Nodes.Add(buildPresetFilterNode(server, new JiraPresetFilterReportedByMe(server)));
            node.Nodes.Add(buildPresetFilterNode(server, new JiraPresetFilterRecentlyResolved(server)));
            node.Nodes.Add(buildPresetFilterNode(server, new JiraPresetFilterRecentlyAdded(server)));
            node.Nodes.Add(buildPresetFilterNode(server, new JiraPresetFilterRecentlyUpdated(server)));
            node.Nodes.Add(buildPresetFilterNode(server, new JiraPresetFilterMostImportant(server)));
        }

        private JiraPresetFilterTreeNode buildPresetFilterNode(JiraServer server, JiraPresetFilter filter) {
            return new JiraPresetFilterTreeNode(server, filter, setPresetFilterProject, clearPresetFilterProject, 1);
        }

        private void setPresetFilterProject(JiraPresetFilterTreeNode filterNode) {
            SelectJiraProject dlg = new SelectJiraProject(JiraServerCache.Instance.getProjects(filterNode.Server).Values, filterNode.Filter.Project);
            dlg.ShowDialog();
            JiraProject project = dlg.getSelectedProject();
            if (project == null) return;
            filterNode.setProject(project);
            SelectedNode = filterNode;
            reloadIssues();
        }

        private void clearPresetFilterProject(JiraPresetFilterTreeNode filterNode) {
            filterNode.setProject(null);
            SelectedNode = filterNode;
            reloadIssues();
        }

        private void setAllPresetFiltersProject(JiraPresetFiltersGroupTreeNode groupNode) {
            SelectJiraProject dlg = new SelectJiraProject(JiraServerCache.Instance.getProjects(groupNode.Server).Values, groupNode.Project);
            dlg.ShowDialog();
            JiraProject project = dlg.getSelectedProject();
            if (project == null) return;

            foreach (var n in groupNode.Nodes) {
                JiraPresetFilterTreeNode node = n as JiraPresetFilterTreeNode;
                if (node == null) continue;
                node.setProject(project);
                if (SelectedNode == node) {
                    reloadIssues();
                }
            }
        }

        private void clearAllPresetFiltersProject(JiraPresetFiltersGroupTreeNode groupNode) {
            DialogResult result = MessageBox.Show(
                "Do you really want to clear projects from all preset filters?",
                Constants.QUESTION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (DialogResult.Yes != result) return;

            foreach (var n in groupNode.Nodes) {
                JiraPresetFilterTreeNode node = n as JiraPresetFilterTreeNode;
                if (node == null) continue;
                node.setProject(null);
                if (SelectedNode == node) {
                    reloadIssues();
                }
            }
        }

        public delegate void OnEditOrRemove();

        public void removeCustomFilter(JiraCustomFilterTreeNode node) {
            if (node == null) {
                return;
            }
            DialogResult result =
                MessageBox.Show("Do you really want to remove this local filter?", 
                Constants.QUESTION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (DialogResult.Yes != result) return;

            TreeNodeWithJiraServer groupNode = findGroupNode(node.Server, typeof(JiraCustomFiltersGroupTreeNode));
            if (groupNode == null) return;

            groupNode.Nodes.Remove(node);
            SelectedNode = groupNode;
            JiraCustomFilter.remove(node.Filter);
            reloadIssues();
        }

        public void editCustomFilter(JiraCustomFilterTreeNode node) {
            if (node == null) {
                return;
            }

            EditCustomFilter ecf = new EditCustomFilter(node.Server, node.Filter, true);
            ecf.ShowDialog();
            if (!ecf.Changed) return;

            node.setFilterName(node.Filter.Name);

            JiraCustomFilter.save();
            reloadIssues();
        }

        //
        // from http://support.microsoft.com/kb/322634
        //
        private void filtersTree_MouseMove(object sender, MouseEventArgs e) {
            // Get the node at the current mouse pointer location.
            TreeNode theNode = GetNodeAt(e.X, e.Y);

            // Set a ToolTip only if the mouse pointer is actually paused on a node.
            if ((theNode != null)) {
                // Verify that the tag property is not "null".
                if (theNode.Tag != null) {
                    // Change the ToolTip only if the pointer moved to a new node.
                    if (theNode.Tag.ToString() != filtersTreeToolTip.GetToolTip(this)) {
                        filtersTreeToolTip.SetToolTip(this, theNode.Tag.ToString());
                    }
                } else {
                    filtersTreeToolTip.SetToolTip(this, "");
                }
            } else // Pointer is not over a node so clear the ToolTip.
            {
                filtersTreeToolTip.SetToolTip(this, "");
            }
        }

        public void rememberLastSelectedFilterItem() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            bool recentlyViewed = SelectedNode is RecentlyOpenIssuesTreeNode;
            store.storeParameter(FILTERS_TREE_RECENT_PARAM, recentlyViewed ? 1 : 0);

            TreeNodeWithJiraServer nodeWithJiraServer = SelectedNode as TreeNodeWithJiraServer;
            if (nodeWithJiraServer != null) {
                store.storeParameter(FILTERS_TREE_SERVER_PARAM, nodeWithJiraServer.Server.GUID.ToString());
            }
            JiraPresetFiltersGroupTreeNode pgtn = SelectedNode as JiraPresetFiltersGroupTreeNode;
            JiraCustomFiltersGroupTreeNode cgtn = SelectedNode as JiraCustomFiltersGroupTreeNode;
            JiraSavedFiltersGroupTreeNode sgtn = SelectedNode as JiraSavedFiltersGroupTreeNode;
            GhSprintTreeNode ghtn = SelectedNode as GhSprintTreeNode;

            if (ghtn != null) {
                store.storeParameter(FILTERS_TREE_FILTER_GROUP_PARAM, GH_SPRINT_GROUP_NAME);
                store.storeParameter(FILTERS_TREE_FILTER_PARAM, ghtn.Sprint.BoardId + "/" + ghtn.Sprint.Id);
            } else if (pgtn != null) {
                store.storeParameter(FILTERS_TREE_FILTER_GROUP_PARAM, PRESET_FILTER_GROUP_NAME);
                store.storeParameter(FILTERS_TREE_FILTER_PARAM, null);
            } else {
                if (cgtn != null) {
                    store.storeParameter(FILTERS_TREE_FILTER_GROUP_PARAM, CUSTOM_FILTER_GROUP_NAME);
                    store.storeParameter(FILTERS_TREE_FILTER_PARAM, null);
                } else {
                    if (sgtn != null) {
                        store.storeParameter(FILTERS_TREE_FILTER_GROUP_PARAM, SAVED_FILTER_GROUP_NAME);
                        store.storeParameter(FILTERS_TREE_FILTER_PARAM, null);
                    } else {
                        JiraPresetFilterTreeNode ptn = SelectedNode as JiraPresetFilterTreeNode;
                        JiraCustomFilterTreeNode ctn = SelectedNode as JiraCustomFilterTreeNode;
                        JiraSavedFilterTreeNode stn = SelectedNode as JiraSavedFilterTreeNode;
                        if (ptn != null) {
                            store.storeParameter(FILTERS_TREE_FILTER_GROUP_PARAM, PRESET_FILTER_GROUP_NAME);
                            store.storeParameter(FILTERS_TREE_FILTER_PARAM, ptn.Filter.GetType().ToString());
                        } else if (ctn != null) {
                            store.storeParameter(FILTERS_TREE_FILTER_GROUP_PARAM, CUSTOM_FILTER_GROUP_NAME);
                            store.storeParameter(FILTERS_TREE_FILTER_PARAM, ctn.Filter.Guid.ToString());
                        } else if (stn != null) {
                            store.storeParameter(FILTERS_TREE_FILTER_GROUP_PARAM, SAVED_FILTER_GROUP_NAME);
                            store.storeParameter(FILTERS_TREE_FILTER_PARAM, stn.Filter.Id);
                        } else {
                            store.storeParameter(FILTERS_TREE_FILTER_GROUP_PARAM, null);
                            store.storeParameter(FILTERS_TREE_FILTER_PARAM, null);
                        }
                    }
                }
            }
        }

        public void restoreLastSelectedFilterItem() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            bool recentlyViewed = store.loadParameter(FILTERS_TREE_RECENT_PARAM, 0) != 0;
            if (recentlyViewed) {
                foreach (TreeNode node in Nodes) {
                    if (!(node is RecentlyOpenIssuesTreeNode)) continue;
                    SelectedNode = node;
                    break;
                }
            } else {
                string serverGuid = store.loadParameter(FILTERS_TREE_SERVER_PARAM, null);
                foreach (var node in Nodes) {
                    TreeNodeWithJiraServer tnws = node as TreeNodeWithJiraServer;
                    if (tnws == null) continue;
                    if (!tnws.Server.GUID.ToString().Equals(serverGuid)) continue;
                    string group = store.loadParameter(FILTERS_TREE_FILTER_GROUP_PARAM, null);
                    if (group == null) {
                        SelectedNode = tnws;
                    } else {
                        string filter = store.loadParameter(FILTERS_TREE_FILTER_PARAM, null);
                        TreeNodeWithJiraServer groupNode;
                        switch (group) {
                            case GH_SPRINT_GROUP_NAME:
                                groupNode = findGroupNode(tnws.Server, typeof (GhGroupTreeNode));
                                if (selectFilterNode(filter, findGhBoardGroupNode(groupNode as GhGroupTreeNode, filter), compareGhSprint)) {
                                    return;
                                }
                                break;
                            case PRESET_FILTER_GROUP_NAME:
                                groupNode = findGroupNode(tnws.Server, typeof(JiraPresetFiltersGroupTreeNode));
                                if (selectFilterNode(filter, groupNode, comparePresetFilterNodeToString)) {
                                    return;
                                }
                                break;
                            case SAVED_FILTER_GROUP_NAME:
                                groupNode = findGroupNode(tnws.Server, typeof(JiraSavedFiltersGroupTreeNode));
                                if (selectFilterNode(filter, groupNode, compareSavedFilterNodeToString)) {
                                    return;
                                }
                                break;
                            case CUSTOM_FILTER_GROUP_NAME:
                                groupNode = findGroupNode(tnws.Server, typeof(JiraCustomFiltersGroupTreeNode));
                                if (selectFilterNode(filter, groupNode, compareCustomFilterNodeToString)) {
                                    return;
                                }
                                break;
                        }
                    }
                }
            }
        }

        private static TreeNode findGhBoardGroupNode(GhGroupTreeNode group, string filter) {
            var strings = filter.Split('/');
            return group.Nodes.OfType<GhBoardTreeNode>().FirstOrDefault(b => ("" + b.Board.Id).Equals(strings[0]));
        }

        private bool selectFilterNode(string filter, TreeNode groupNode, CompareFilterNodeToString compare) {
            if (filter == null) {
                SelectedNode = groupNode;
                return true;
            }
            if (groupNode == null) {
                return false;
            }
            foreach (TreeNode fn in groupNode.Nodes.Cast<TreeNode>().Where(fn => compare(fn, filter))) {
                SelectedNode = fn;
                return true;
            }
            return false;
        }

        private static bool compareCustomFilterNodeToString(TreeNode node, string filter) {
            return filter.Equals((((JiraCustomFilterTreeNode)node).Filter.Guid.ToString()));
        }

        private static bool compareSavedFilterNodeToString(TreeNode node, string filter) {
            return filter.Equals((((JiraSavedFilterTreeNode)node).Filter.Id).ToString());
        }

        private static bool compareGhSprint(TreeNode node, string filter) {
            var sprint = ((GhSprintTreeNode)node).Sprint;
            return filter.Equals(sprint.BoardId + "/" + sprint.Id);
        }

        private static bool comparePresetFilterNodeToString(TreeNode node, string filter) {
            return filter.Equals(((JiraPresetFilterTreeNode)node).Filter.GetType().ToString());
        }

        private delegate bool CompareFilterNodeToString(TreeNode node, string filter);

        public void addToolTip(ToolTip tip) {
            tip.SetToolTip(this, "");
            tip.Active = true;
            filtersTreeToolTip = tip;
            MouseMove += filtersTree_MouseMove;
        }

        public void setReloadIssuesCallback(Action reload) {
            reloadIssues = reload;
        }

        public void setModel(JiraServerModel m) {
            model = m;
        }

        private bool isRestoring;

        public void restoreExpandCollapseStates() {
            if (CollapseExpandManager == null) {
                ExpandAll();
                return;
            }
            isRestoring = true;
            foreach (var node in Nodes) {
                restoreExpandCollapseState(node as TreeNode);    
            }
            isRestoring = false;
        }

        private void jiraFiltersTreeAfterExpand(object sender, TreeViewEventArgs e) {
            if (isRestoring) return;
            rememberExpandCollapseState(e.Node);
        }

        private void jiraFiltersTreeAfterCollapse(object sender, TreeViewEventArgs e) {
            if (isRestoring) return;
            rememberExpandCollapseState(e.Node);
        }

        private void rememberExpandCollapseState(TreeNode node) {
            if (node == null) return;
            if (CollapseExpandManager == null) return;
            CollapseExpandManager.rememberNodeState(node);
            foreach (var childNode in node.Nodes) {
                rememberExpandCollapseState(childNode as TreeNode);
            }
        }

        private void restoreExpandCollapseState(TreeNode node) {
            if (node == null) return;
            if (CollapseExpandManager == null) return;
            CollapseExpandManager.restoreNodeState(node);
            foreach (var childNode in node.Nodes) {
                restoreExpandCollapseState(childNode as TreeNode);
            }
        }
    }
}
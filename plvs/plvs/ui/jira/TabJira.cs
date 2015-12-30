using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.dialogs.jira;
using Atlassian.plvs.explorer;
using Atlassian.plvs.models;
using Atlassian.plvs.models.jira;
using Aga.Controls.Tree;
using Atlassian.plvs.store;
using Atlassian.plvs.ui.jira.issuefilternodes;
using Atlassian.plvs.ui.jira.issues;
using Atlassian.plvs.ui.jira.issues.treemodels;
using Atlassian.plvs.util;
using Atlassian.plvs.windows;
using EnvDTE;
using Process=System.Diagnostics.Process;
using SearchIssue = Atlassian.plvs.dialogs.jira.SearchIssue;
using Thread=System.Threading.Thread;

namespace Atlassian.plvs.ui.jira {
    public partial class TabJira : UserControl, AddNewServerLink {

        private const string GROUP_SUBTASKS_UNDER_PARENT = "JiraIssueListGroupSubtasksUnderParent";
        private const string FILTER_PANEL_VISIBLE = "JiraIssueListFilterPanelVisible";
        private const string HIDE_FILTERS = "Hide Filters";
        private const string SHOW_FILTERS = "Show Filters";

        private JiraIssueTree issuesTree;

        private readonly JiraIssueListModelBuilder builder;

        private static readonly JiraIssueListModel MODEL = JiraIssueListModelImpl.Instance;

        private readonly JiraIssueListSearchingModel searchingModel = new JiraIssueListSearchingModel(MODEL);

        private readonly StatusLabel status;

        private LinkLabel linkAddJiraServer;

        private JiraIssue lastSelectedIssue;

        private int currentGeneration;
        private bool metadataFetched;
        public JiraActiveIssueManager ActiveIssueManager { get; private set; }

        private ToolStripButton buttonShowHideFilters;

        private bool initialFilterSelected;
        private int baseSpliterDistance;

        public TabJira() {
            InitializeComponent();
            setupGroupByCombo();

            status = new StatusLabel(statusStrip, jiraStatus);

            registerIssueModelListener();
            builder = new JiraIssueListModelBuilder(Facade);

            filtersTree.setReloadIssuesCallback(reloadIssues);
            filtersTree.addToolTip(filtersTreeToolTip);
            filtersTree.setModel(JiraServerModel.Instance);

            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            bool groupSubtasks = store.loadParameter(GROUP_SUBTASKS_UNDER_PARENT, 1) != 0;
            buttonGroupSubtasks.Checked = groupSubtasks;

            buttonServerExplorer.Visible = GlobalSettings.JiraServerExplorerEnabled;
            buttonServerExplorer.Enabled = GlobalSettings.JiraServerExplorerEnabled;

            GlobalSettings.SettingsChanged += globalSettingsSettingsChanged;

            getMoreIssues.VisibleChanged += (s, e) => resizeStatusBar();

            statusStrip.Items.Clear();
            initializeShowHideFiltersButton(store.loadParameter(FILTER_PANEL_VISIBLE, 1) != 0);
            initializeActiveIssueToolStrip();
            resizeStatusBar();
        }

        private void initializeShowHideFiltersButton(bool visible) {
            buttonShowHideFilters = new ToolStripButton {
                                        Image = Resources.ico_jira_filter,
                                        CheckOnClick = true,
                                        Text = visible ? HIDE_FILTERS : SHOW_FILTERS,
                                        DisplayStyle = ToolStripItemDisplayStyle.Image,
                                        AutoToolTip = true,
                                        Checked = visible
                                    };
            buttonShowHideFilters.Click += buttonShowHideFilters_Click;
            statusStrip.Items.Add(buttonShowHideFilters);
            statusStrip.Items.Add(new ToolStripSeparator());
            baseSpliterDistance = jiraSplitter.SplitterDistance;
            jiraSplitter.Panel1MinSize = visible ? 25 : 1;
            // we can't just collapse the left panel right away, because issue list retrieval
            // is triggered by a filter tree selection event. It turns out that this event is
            // not fired until the tree is actually visible - seems like the tree control is not
            // instantiated until the parent panel is uncollapsed. Or something.
            // So I am cheating and make the filter tree "almost invisible" by making it 1 pixel narrow
            if (!visible) {
                jiraSplitter.SplitterDistance = 1;
            }
        }

        private void buttonShowHideFilters_Click(object sender, EventArgs e) {
            bool visible = buttonShowHideFilters.Checked;
            buttonShowHideFilters.Text = visible ? HIDE_FILTERS : SHOW_FILTERS;
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            store.storeParameter(FILTER_PANEL_VISIBLE, visible ? 1 : 0);
            jiraSplitter.Panel1MinSize = visible ? 25 : 1;
            if (initialFilterSelected) {
                jiraSplitter.Panel1Collapsed = !visible;
            }
            if (visible) {
                jiraSplitter.SplitterDistance = baseSpliterDistance;
            } else {
                baseSpliterDistance = jiraSplitter.SplitterDistance;
                jiraSplitter.SplitterDistance = 1;
            }
        }

        private void initializeActiveIssueToolStrip() {
            ActiveIssueManager = new JiraActiveIssueManager(statusStrip, status);
            statusStrip.Items.Add(jiraStatus);
            statusStrip.Items.Add(getMoreIssues);
            ActiveIssueManager.ActiveIssueChanged += activeIssueManager_ActiveIssueChanged;
            ActiveIssueManager.ToolbarWidthChanged += (s, e) => resizeStatusBar();
        }

        private void activeIssueManager_ActiveIssueChanged(object sender, EventArgs e) {
            updateStartStopButton();
            resizeStatusBar();
        }

        private void updateStartStopButton() {
            JiraActiveIssueManager.ActiveIssue activeIssue = ActiveIssueManager.CurrentActiveIssue;

            TreeNodeAdv node = issuesTree.SelectedNode;
            if (node == null || !(node.Tag is IssueNode)) {
                return;
            }
            JiraIssue issue = (node.Tag as IssueNode).Issue;
            if (activeIssue == null || !issue.Key.Equals(activeIssue.Key) || !issue.Server.GUID.ToString().Equals(activeIssue.ServerGuid)) {
                buttonStartStopWork.Image = Resources.ico_activateissue;
                buttonStartStopWork.Text = "Start Work";
            } else {
                buttonStartStopWork.Image = Resources.ico_inactiveissue;
                buttonStartStopWork.Text = "Stop Work";
            }
        }

        public event EventHandler<EventArgs> AddNewServerLinkClicked;

        public AbstractJiraServerFacade Facade {
            get { return SmartJiraServerFacade.Instance; }
        }

        private void setupGroupByCombo() {
            foreach (JiraIssueGroupByComboItem.GroupBy groupBy in Enum.GetValues(typeof (JiraIssueGroupByComboItem.GroupBy))) {
                comboGroupBy.Items.Add(new JiraIssueGroupByComboItem(groupBy, searchingModel, buttonGroupSubtasks));
            }
            comboGroupBy.SelectedIndexChanged += comboGroupBy_SelectedIndexChanged;
        }

        private void registerIssueModelListener() {
            searchingModel.ModelChanged += searchingModel_ModelChanged;
        }

        private const string RETRIEVING_ISSUES_FAILED = "Retrieving issues failed";

        private class ActiveIssueMenuItem : ToolStripMenuItem {
            private readonly JiraActiveIssueManager mgr;
            private readonly JiraIssueTree tree;

            public ActiveIssueMenuItem(JiraActiveIssueManager mgr, JiraIssueTree issueTree) {
                this.mgr = mgr;
                tree = issueTree;

                Click += activeIssueMenuItemClick;
            }

            void activeIssueMenuItemClick(object sender, EventArgs e) {
                TreeNodeAdv node = tree.SelectedNode;
                if (node == null || !(node.Tag is IssueNode)) {
                    return;
                }
                JiraIssue issue = (node.Tag as IssueNode).Issue;
                mgr.toggleActiveState(issue);
            }

            public override string Text { get {
                    TreeNodeAdv node = tree.SelectedNode;
                    if (node == null || !(node.Tag is IssueNode)) {
                        return "Should not happen";
                    }
                    JiraIssue issue = (node.Tag as IssueNode).Issue;
                    return mgr.isActive(issue) ? "Stop Work" : "Start Work";
                }
            }

            public override Image Image { get {
                    TreeNodeAdv node = tree.SelectedNode;
                    if (node == null || !(node.Tag is IssueNode)) {
                        return null;
                    }
                    JiraIssue issue = (node.Tag as IssueNode).Issue;
                    return mgr.isActive(issue) ? Resources.ico_inactiveissue : Resources.ico_activateissue;
                }
            }
        }

        private void initIssuesTree() {
            if (issuesTree != null) {
                issueTreeContainer.ContentPanel.Controls.Remove(issuesTree);
            }

            issuesTree = new JiraIssueTree(jiraSplitter, status, searchingModel, filtersTree.ItemHeight, filtersTree.Font);

            ToolStripMenuItem copyToClipboardItem = new ToolStripMenuItem("Copy to Clipboard", Resources.ico_copytoclipboard)
                                                    {
                                                        DropDown = createCopyIssueMenuItems()
                                                    };

            ((ContextMenuStrip) copyToClipboardItem.DropDown).ShowImageMargin = false;
            ((ContextMenuStrip) copyToClipboardItem.DropDown).ShowCheckMargin = false;

            issuesTree.addContextMenu(new ToolStripItem[]
                                  {
                                      new ToolStripMenuItem("Open in IDE", Resources.ico_editinide,
                                                            new EventHandler(openIssue)),
                                      new ActiveIssueMenuItem(ActiveIssueManager, issuesTree),
                                      new ToolStripMenuItem("View in Browser", Resources.view_in_browser,
                                                            new EventHandler(browseIssue)),
                                      new ToolStripMenuItem("Edit in Browser", Resources.ico_editinbrowser,
                                                            new EventHandler(browseEditIssue)),
                                      copyToClipboardItem,
                                      new ToolStripSeparator(),
                                      new ToolStripMenuItem("Log Work", Resources.ico_logworkonissue,
                                                            new EventHandler(logWork))
                                  });

            issuesTree.NodeMouseDoubleClick += issuesTree_NodeMouseDoubleClick;
            issuesTree.KeyPress += issuesTree_KeyPress;
            issuesTree.SelectionChanged += issuesTree_SelectionChanged;

            issueTreeContainer.ContentPanel.Controls.Add(issuesTree);

            updateIssueListButtons();
        }

        private ContextMenuStrip createCopyIssueMenuItems() {
            ContextMenuStrip strip = new ContextMenuStrip();
            strip.Items.Add("phony");

            strip.Opened += (s, e) => addCopyMenuActions(strip);

            return strip;
        }

        private void addCopyMenuActions(ToolStrip strip) {
            strip.Items.Clear();
            ToolStripMenuItem[] items = new ToolStripMenuItem[4];
            items[0] = new CopyToClipboardMenuItem(issuesTree, CopyToClipboardMenuItem.CopyType.KEY);
            items[1] = new CopyToClipboardMenuItem(issuesTree, CopyToClipboardMenuItem.CopyType.SUMMARY);
            items[2] = new CopyToClipboardMenuItem(issuesTree, CopyToClipboardMenuItem.CopyType.URL);
            items[3] = new CopyToClipboardMenuItem(issuesTree, CopyToClipboardMenuItem.CopyType.KEY_AND_SUMMARY);
            strip.Items.AddRange(items);
        }

        private class CopyToClipboardMenuItem : ToolStripMenuItem {
            private readonly JiraIssueTree issuesTree;
            private readonly CopyType type;

            internal enum CopyType {
                KEY,
                SUMMARY,
                URL,
                KEY_AND_SUMMARY
            }

            public CopyToClipboardMenuItem(JiraIssueTree issuesTree, CopyType type) {
                this.issuesTree = issuesTree;
                this.type = type;

                Click += (s, e) => Clipboard.SetText(Text); 
            }

            public override string Text {
                get {
                    if (issuesTree == null) {
                        return base.Text;
                    }
                    TreeNodeAdv node = issuesTree.SelectedNode;
                    if (node != null && node.Tag is IssueNode) {
                        JiraIssue issue = (node.Tag as IssueNode).Issue;

                        switch (type) {
                            case CopyType.KEY:
                                return issue.Key;
                            case CopyType.SUMMARY:
                                return issue.Summary;
                            case CopyType.URL:
                                return issue.Server.Url + "/browse/" + issue.Key;
                            case CopyType.KEY_AND_SUMMARY:
                                return issue.Key + ": " + issue.Summary;
                        }
                    }
                    return "No issue selected";
                }
            }
        }

        private void comboGroupBy_SelectedIndexChanged(object sender, EventArgs e) {
            updateIssuesTreeModel();
            updateIssueListButtons();
            issuesTree.restoreExpandCollapseStates();
        }

        private void updateIssuesTreeModel() {
            AbstractIssueTreeModel oldIssueTreeModel = issuesTree.Model as AbstractIssueTreeModel;
            if (oldIssueTreeModel != null) {
                oldIssueTreeModel.shutdown();
            }

            JiraIssueGroupByComboItem item = comboGroupBy.SelectedItem as JiraIssueGroupByComboItem;
            if (item == null) {
                return;
            }
            AbstractIssueTreeModel issueTreeModel = item.TreeModel;

            // just in case somebody reuses the old model object :)
            issueTreeModel.shutdown();
            issuesTree.Model = issueTreeModel;
            issueTreeModel.StructureChanged += issuesTree_StructureChanged;
            issueTreeModel.TreeAboutToChange += issueTreeModel_TreeAboutToChange;
            issueTreeModel.NodesChanged += issueTreeModel_NodesChanged;
            issueTreeModel.NodesInserted += issueTreeModel_NodesInserted;
            issueTreeModel.NodesRemoved += issueTreeModel_NodesRemoved;
            issueTreeModel.init();
        }

        private void updateIssueListButtons() {
            bool issueSelected = (issuesTree.SelectedNode != null && issuesTree.SelectedNode.Tag is IssueNode);
            buttonViewInBrowser.Enabled = issueSelected;
            buttonEditInBrowser.Enabled = issueSelected;
            buttonOpen.Enabled = issueSelected;
            buttonRefresh.Enabled = filtersTree.FilterOrRecentlyViewedSelected;
            buttonSearch.Enabled = CurrentlySelectedServerOrDefault != null;
            buttonCreate.Enabled = CurrentlySelectedServerOrDefault != null && metadataFetched;

            JiraIssueGroupByComboItem selected = comboGroupBy.SelectedItem as JiraIssueGroupByComboItem;
            Boolean notNone = selected != null && selected.By != JiraIssueGroupByComboItem.GroupBy.NONE;
            buttonExpandAll.Visible = notNone;
            buttonExpandAll.Enabled = notNone;
            buttonCollapseAll.Visible = notNone;
            buttonCollapseAll.Enabled = notNone;
            buttonEditFilter.Enabled = filtersTree.SelectedNode is JiraCustomFilterTreeNode;
            buttonRemoveFilter.Enabled = filtersTree.SelectedNode is JiraCustomFilterTreeNode;
            buttonAddFilter.Enabled = filtersTree.NodeWithServerSelected && metadataFetched;
            buttonServerExplorer.Enabled = filtersTree.NodeWithServerSelected;
            buttonStartStopWork.Enabled = issueSelected;
            updateStartStopButton();
        }

        private delegate void IssueAction(JiraIssue issue);

        private void runSelectedIssueAction(IssueAction action) {
            TreeNodeAdv node = issuesTree.SelectedNode;
            if (node == null || !(node.Tag is IssueNode)) {
                return;
            }
            action((node.Tag as IssueNode).Issue);
        }

        private void browseIssue(object sender, EventArgs e) {
            runSelectedIssueAction(browseSelectedIssue);
        }

        private static void browseSelectedIssue(JiraIssue issue) {
            try {
                PlvsUtils.runBrowser(issue.Server.Url + "/browse/" + issue.Key);
            } catch (Exception e) {
                Debug.WriteLine("TabJira.browseSelectedIssue() - exception: " + e.Message);
            }
            UsageCollector.Instance.bumpJiraIssuesOpen();
        }

        private void browseEditIssue(object sender, EventArgs e) {
            runSelectedIssueAction(browseEditSelectedIssue);
        }

        private void logWork(object sender, EventArgs e) {
            runSelectedIssueAction(logWorkOnSelectedIssue);
        }

        private static void browseEditSelectedIssue(JiraIssue issue) {
            try {
                PlvsUtils.runBrowser(issue.Server.Url + "/secure/EditIssue!default.jspa?id=" + issue.Id);
            } catch (Exception e) {
                Debug.WriteLine("TabJira.browseEditSelectedIssue() - exception: " + e.Message);
            }
            UsageCollector.Instance.bumpJiraIssuesOpen();
        }

        private void logWorkOnSelectedIssue(JiraIssue issue) {
            new LogWork(this, MODEL, Facade, issue, status, ActiveIssueManager).ShowDialog();
        }

        private void openIssue(object sender, EventArgs e) {
            runSelectedIssueAction(openSelectedIssue);
        }

        private void issuesTree_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Enter) {
                return;
            }
            runSelectedIssueAction(openSelectedIssue);
        }

        private void issuesTree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e) {
            runSelectedIssueAction(openSelectedIssue);
        }

        private void issuesTree_SelectionChanged(object sender, EventArgs e) {
            Invoke(new MethodInvoker(updateIssueListButtons));
            invokeSelectedIssueChanged();
        }

        private void openSelectedIssue(JiraIssue issue) {
            IssueDetailsWindow.Instance.openIssue(issue, ActiveIssueManager);
        }

        private void issuesTree_StructureChanged(object sender, TreePathEventArgs e) {
            issuesTree.restoreExpandCollapseStates();
            restoreExpandStatesAndSelectedIssue(lastSelectedIssue);
            invokeSelectedIssueChanged();
        }

        private void issueTreeModel_NodesInserted(object sender, TreeModelEventArgs e) {
            restoreExpandStatesAndSelectedIssue(lastSelectedIssue);
            invokeSelectedIssueChanged();
        }

        private void issueTreeModel_NodesChanged(object sender, TreeModelEventArgs e) {
            restoreExpandStatesAndSelectedIssue(lastSelectedIssue);
            invokeSelectedIssueChanged();
        }

        private void issueTreeModel_NodesRemoved(object sender, TreeModelEventArgs e) {
            restoreExpandStatesAndSelectedIssue(lastSelectedIssue);
            invokeSelectedIssueChanged();
        }

        void issueTreeModel_TreeAboutToChange(object sender, EventArgs e) {
            lastSelectedIssue = SelectedIssue;
        }

        public event EventHandler<SelectedIssueEventArgs> SelectedIssueChanged;

        private void invokeSelectedIssueChanged() {
            EventHandler<SelectedIssueEventArgs> handler = SelectedIssueChanged;
            if (handler == null) {
                return;
            }

            handler(this, new SelectedIssueEventArgs(SelectedIssue));
        }

        public JiraIssue SelectedIssue {
            get {
                bool issueSelected = (issuesTree.SelectedNode != null && issuesTree.SelectedNode.Tag is IssueNode);
                JiraIssue issue = null;
                if (issueSelected) {
                    issue = ((IssueNode) issuesTree.SelectedNode.Tag).Issue;
                }
                return issue;
            }
        }

        public void reloadKnownJiraServers() {

            if (linkAddJiraServer != null) {
                Controls.Remove(linkAddJiraServer);
            }

            AtlassianPanel.Instance.JiraTabVisible = JiraServerModel.Instance.getAllEnabledServers().Count > 0;

            if (JiraServerModel.Instance.getAllServers().Count == 0) {

                linkAddJiraServer = new LinkLabel
                                    {
                                        Dock = DockStyle.Fill,
                                        Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238),
                                        Image = Resources.jira_blue_16_with_padding,
                                        Location = new Point(0, 0),
                                        Name = "linkAddJiraServers",
                                        Size = new Size(1120, 510),
                                        TabIndex = 0,
                                        TabStop = true,
                                        Text = "Add JIRA Servers",
                                        BackColor = Color.White,
                                        TextAlign = ContentAlignment.MiddleCenter
                                    };

                linkAddJiraServer.LinkClicked += linkAddJiraServers_LinkClicked;
                jiraContainer.Visible = false;
                Controls.Add(linkAddJiraServer);
            } else {

                filtersTree.CollapseExpandManager = 
                    new TreeNodeCollapseExpandStatusManager(ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS));

                jiraContainer.Visible = true;

                filtersTree.clear();
                searchingModel.clear(true);

                getMoreIssues.Visible = false;

                // copy to local list so that we can reuse in our threads
                List<JiraServer> servers = new List<JiraServer>(JiraServerModel.Instance.getAllEnabledServers());
                if (servers.Count == 0) {
                    status.setInfo("No JIRA servers enabled");
                    return;
                }

                filtersTree.addServerNodes(servers);

                ++currentGeneration;

                metadataFetched = false;
                Thread metadataThread = PlvsUtils.createThread(() => reloadKnownServersWorker(servers, currentGeneration));
                metadataThread.Start();
            }
        }

        private void linkAddJiraServers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (AddNewServerLinkClicked != null) {
                AddNewServerLinkClicked(this, new EventArgs());
            }
        }

        private void reloadKnownServersWorker(IEnumerable<JiraServer> servers, int myGeneration) {

            try {

                JiraServerCache.Instance.clearGhBoards();
                JiraServerCache.Instance.clearProjects();
                JiraServerCache.Instance.clearIssueTypes();
                JiraServerCache.Instance.clearStatuses();
                JiraServerCache.Instance.clearPriorities();
                JiraServerCache.Instance.clearResolutions();

                foreach (JiraServer server in servers) {

                    status.setInfo("[" + server.Name + "] Authenticating...");
                    Facade.login(server);

                    status.setInfo("[" + server.Name + "] Loading Agile boards...");
                    bool supportsGh = Facade.supportsGh(server);
                    if (supportsGh) {
                        var rapidBoards = Facade.getGhBoards(server);
                        if (rapidBoards != null) {
                            foreach (var rapidBoard in rapidBoards) {
                                JiraServerCache.Instance.addRapidBoard(server, rapidBoard);
                                var ghSprints = Facade.getGhSprints(server, rapidBoard.Id);
                                if (ghSprints != null) {
                                    rapidBoard.Sprints.AddRange(ghSprints);
                                }
                            }
                        }
                    }

                    status.setInfo("[" + server.Name + "] Loading project definitions...");
                    var projects = Facade.getProjects(server);
                    foreach (JiraProject proj in projects) {
                        JiraServerCache.Instance.addProject(server, proj);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue types...");
                    List<JiraNamedEntity> issueTypes = Facade.getIssueTypes(server);
                    foreach (JiraNamedEntity type in issueTypes) {
                        JiraServerCache.Instance.addIssueType(server, type);
                        ImageCache.Instance.getImage(server, type.IconUrl);
                    }
                    List<JiraNamedEntity> subtaskIssueTypes = Facade.getSubtaskIssueTypes(server);
                    foreach (JiraNamedEntity type in subtaskIssueTypes) {
                        JiraServerCache.Instance.addIssueType(server, type);
                        ImageCache.Instance.getImage(server, type.IconUrl);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue priorities...");
                    List<JiraNamedEntity> priorities = Facade.getPriorities(server);
                    foreach (JiraNamedEntity prio in priorities) {
                        JiraServerCache.Instance.addPriority(server, prio);
                        ImageCache.Instance.getImage(server, prio.IconUrl);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue resolutions...");
                    List<JiraNamedEntity> resolutions = Facade.getResolutions(server);
                    foreach (JiraNamedEntity res in resolutions) {
                        JiraServerCache.Instance.addResolution(server, res);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue statuses...");
                    List<JiraNamedEntity> statuses = Facade.getStatuses(server);
                    foreach (JiraNamedEntity s in statuses) {
                        JiraServerCache.Instance.addStatus(server, s);
                        ImageCache.Instance.getImage(server, s.IconUrl);
                    }

                    status.setInfo("[" + server.Name + "] Loading saved filters...");
                    PlvsLogger.log("reloadKnownServersWorker() - [" + server.Name + "] Loading saved filters...");
                    List<JiraSavedFilter> filters = Facade.getSavedFilters(server);
                    PlvsLogger.log("reloadKnownServersWorker() - finished loading saved filters, invoking filter tree population");
                    JiraServer jiraServer = server;
                    Invoke(new MethodInvoker(delegate
                                                 {
                                                     // PLVS-59
                                                     if (myGeneration != currentGeneration) {
                                                         PlvsLogger.log("reloadKnownServersWorker() - generations don't match");
                                                         return;
                                                     }
                                                     PlvsLogger.log("reloadKnownServersWorker() - populating filter nodes");
                                                     if (supportsGh) {
                                                         filtersTree.addGhNodes(jiraServer);
                                                     }
                                                     filtersTree.addFilterGroupNodes(jiraServer);
                                                     filtersTree.addPresetFilterNodes(jiraServer);
                                                     filtersTree.addSavedFilterNodes(jiraServer, filters);
                                                     PlvsLogger.log("reloadKnownServersWorker() - populated filter nodes");
                                                     status.setInfo("Loaded saved filters for server " + jiraServer.Name);
                                                     filtersTree.addCustomFilterNodes(jiraServer);
                                                 }));
                }
                Invoke(new MethodInvoker(delegate
                                             {
                                                 // PLVS-59
                                                 if (myGeneration != currentGeneration) {
                                                     return;
                                                 }
                                                 metadataFetched = true;
                                                 filtersTree.addRecentlyViewedNode();
                                                 filtersTree.restoreExpandCollapseStates();
                                                 filtersTree.restoreLastSelectedFilterItem();
                                                 updateIssueListButtons();

                                                 ActiveIssueManager.init();
                                             }));
            } catch (Exception e) {
                status.setError("Failed to load JIRA server information", e);
            }
        }

        private void searchingModel_ModelChanged(object sender, EventArgs e) {
            this.safeInvoke(new MethodInvoker(delegate
                                         {
                                             if (!(filtersTree.FilterOrRecentlyViewedSelected)) {
                                                 return;
                                             }
                                             status.setInfo("Loaded " + MODEL.Issues.Count + " issues");
                                             getMoreIssues.Visible = !(filtersTree.RecentlyViewedSelected) && MODEL.Issues.Count > 0 &&
                                                                     probablyHaveMoreIssues();
                                             updateIssueListButtons();
                                         }));
        }

        private void globalSettingsSettingsChanged(object sender, EventArgs e) {
            buttonServerExplorer.Visible = GlobalSettings.JiraServerExplorerEnabled;
            buttonServerExplorer.Enabled = GlobalSettings.JiraServerExplorerEnabled;
        }

        private static bool probablyHaveMoreIssues() {
            return MODEL.Issues.Count % GlobalSettings.JiraIssuesBatch == 0;
        }

        private void buttonRefreshAll_Click(object sender, EventArgs e) {
            comboFind.Text = "";
            reloadKnownJiraServers();
        }

        private void filtersTree_AfterSelect(object sender, TreeViewEventArgs e) {
            comboFind.Text = "";
            updateIssueListButtons();
            updateIssuesTreeModel();
            if (filtersTree.FilterOrRecentlyViewedSelected) {
                reloadIssues();
            } else {
                searchingModel.clear(true);
            }

            filtersTree.rememberLastSelectedFilterItem();

            if (SelectedServerChanged != null) {
                SelectedServerChanged(this, new EventArgs());
            }

            if (!initialFilterSelected) {
                initialFilterSelected = true;
                if (!buttonShowHideFilters.Checked) {
                    jiraSplitter.Panel1Collapsed = true;
                }
            }
        }

        public event EventHandler<EventArgs> SelectedServerChanged;

        private void restoreExpandStatesAndSelectedIssue(JiraIssue issue) {
            this.safeInvoke(new MethodInvoker(() => {
                                                  issuesTree.restoreExpandCollapseStates();

                                                  if (issue == null) {
                                                      return;
                                                  }

                                                  IEnumerable<TreeNodeAdv> nodes = issuesTree.AllNodes;
                                                  foreach (TreeNodeAdv node in nodes) {
                                                      IssueNode tag = node.Tag as IssueNode;
                                                      if (tag == null || !tag.Issue.Key.Equals(issue.Key) || !tag.Issue.Server.GUID.Equals(issue.Server.GUID)) {
                                                          continue;
                                                      }

                                                      issuesTree.SelectedNode = node;
                                                      break;
                                                  }
                                              }));
        }

        private void reloadIssues() {

            issuesTree.CollapseExpandManager =
                new TreeNodeCollapseExpandStatusManager(ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS));

            JiraSavedFilterTreeNode savedFilterNode;
            RecentlyOpenIssuesTreeNode recentIssuesNode;
            JiraCustomFilterTreeNode customFilterNode;
            JiraPresetFilterTreeNode presetFilterNode;
            GhSprintTreeNode sprintFilterNode;

            filtersTree.getAndCastSelectedNode(out savedFilterNode, out recentIssuesNode, out customFilterNode, out presetFilterNode, out sprintFilterNode);

            Thread issueLoadThread = null;

            JiraIssue selectedIssue = SelectedIssue;

            if (savedFilterNode != null) {
                issueLoadThread = reloadIssuesWithSavedFilter(savedFilterNode, () => restoreExpandStatesAndSelectedIssue(selectedIssue));
            } else if (customFilterNode != null && !customFilterNode.Filter.Empty) {
                issueLoadThread = reloadIssuesWithCustomFilter(customFilterNode, () => restoreExpandStatesAndSelectedIssue(selectedIssue));
            } else if (presetFilterNode != null) {
                issueLoadThread = reloadIssuesWithPresetFilter(presetFilterNode, () => restoreExpandStatesAndSelectedIssue(selectedIssue));
            } else if (recentIssuesNode != null) {
                issueLoadThread = reloadIssuesWithRecentlyViewedIssues(() => restoreExpandStatesAndSelectedIssue(selectedIssue));
            } else if (sprintFilterNode != null) {
                issueLoadThread = reloadIssuesWithSprint(sprintFilterNode, () => restoreExpandStatesAndSelectedIssue(selectedIssue));
            }

            loadIssuesInThread(issueLoadThread);
        }

        private void loadIssuesInThread(Thread issueLoadThread) {
            if (issueLoadThread == null) {
                searchingModel.clear(true);
                return;
            }

            status.setInfo("Loading issues...");
            getMoreIssues.Visible = false;
            issueLoadThread.Start();
        }

        private Thread reloadIssuesWithRecentlyViewedIssues(Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.rebuildModelWithRecentlyViewedIssues(MODEL);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread reloadIssuesWithSavedFilter(JiraSavedFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.rebuildModelWithSavedFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread updateIssuesWithSavedFilter(JiraSavedFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.updateModelWithSavedFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread reloadIssuesWithSprint(GhSprintTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                try {
                    builder.rebuildModelWithSprint(MODEL, node.Server, node.Sprint);
                } catch (Exception ex) {
                    status.setError(RETRIEVING_ISSUES_FAILED, ex);
                } finally {
                    reloadCompleted();
                }
            });
        }

        private Thread updateIssuesWithSprint(GhSprintTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                try {
                    builder.updateModelWithSprint(MODEL, node.Server, node.Sprint);
                } catch (Exception ex) {
                    status.setError(RETRIEVING_ISSUES_FAILED, ex);
                } finally {
                    reloadCompleted();
                }
            });
        }

        private Thread reloadIssuesWithPresetFilter(JiraPresetFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.rebuildModelWithPresetFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread updateIssuesWithPresetFilter(JiraPresetFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.updateModelWithPresetFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread reloadIssuesWithCustomFilter(JiraCustomFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.rebuildModelWithCustomFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread updateIssuesWithCustomFilter(JiraCustomFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.updateModelWithCustomFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private void getMoreIssues_Click(object sender, EventArgs e) {
            JiraSavedFilterTreeNode savedFilterNode;
            RecentlyOpenIssuesTreeNode recentIssuesNode;
            JiraCustomFilterTreeNode customFilterNode;
            JiraPresetFilterTreeNode presetFilterNode;
            GhSprintTreeNode sprintTreeNode;

            filtersTree.getAndCastSelectedNode(out savedFilterNode, out recentIssuesNode, out customFilterNode, out presetFilterNode, out sprintTreeNode);

            Thread issueLoadThread = null;

            JiraIssue selectedIssue = SelectedIssue;

            if (savedFilterNode != null) {
                issueLoadThread = updateIssuesWithSavedFilter(savedFilterNode, () => restoreExpandStatesAndSelectedIssue(selectedIssue));
            } else if (customFilterNode != null && !customFilterNode.Filter.Empty) {
                issueLoadThread = updateIssuesWithCustomFilter(customFilterNode, () => restoreExpandStatesAndSelectedIssue(selectedIssue));
            } else if (presetFilterNode != null) {
                issueLoadThread = updateIssuesWithPresetFilter(presetFilterNode, () => restoreExpandStatesAndSelectedIssue(selectedIssue));
            } else if (sprintTreeNode != null) {
                issueLoadThread = updateIssuesWithSprint(sprintTreeNode, () => restoreExpandStatesAndSelectedIssue(selectedIssue));
            }

            loadIssuesInThread(issueLoadThread);
        }

        private void buttonOpen_Click(object sender, EventArgs e) {
            runSelectedIssueAction(openSelectedIssue);
        }

        private void buttonViewInBrowser_Click(object sender, EventArgs e) {
            runSelectedIssueAction(browseSelectedIssue);
        }

        private void buttonEditInBrowser_Click(object sender, EventArgs e) {
            runSelectedIssueAction(browseEditSelectedIssue);
        }

        private void buttonRefresh_Click(object sender, EventArgs e) {
            comboFind.Text = "";
            reloadIssues();
        }

        private void buttonCreate_Click(object sender, EventArgs e) {
            createIssue();
        }

        public void createIssue() {
            if (!metadataFetched) return;
            JiraServer server = CurrentlySelectedServerOrDefault;
            CreateIssue.createDialogOrBringToFront(server);
        }

        private void buttonSearch_Click(object sender, EventArgs e) {
            searchIssue();
        }

        public void searchIssue() {
            JiraServer server = CurrentlySelectedServerOrDefault;
            if (server == null) {
                return;
            }
            SearchIssue dlg = new SearchIssue(server, MODEL, status);
            dlg.ShowDialog(this);
        }

        public delegate void FindFinished(bool success, string message, Exception e);

        public void findAndOpenIssue(string key, FindFinished onFinish) {
            findAndOpenIssue(key, CurrentlySelectedServerOrDefault, onFinish);
        }

        public void findAndOpenIssue(string key, JiraServer server, FindFinished onFinish) {
            if (server == null) {
                if (onFinish != null) {
                    onFinish(false, "No JIRA server selected", null);
                }
                return;
            }
            Thread runner = PlvsUtils.createThread(() => finishAndOpenIssueWorker(key, server, onFinish));
            runner.Start();
        }

        private void finishAndOpenIssueWorker(string key, JiraServer server, FindFinished onFinish) {
            try {
                status.setInfo("Fetching issue " + key + "...");
                JiraIssue issue =
                    SmartJiraServerFacade.Instance.getIssue(server, key);
                if (issue != null) {
                    status.setInfo("Issue " + key + " found");
                    Invoke(new MethodInvoker(delegate
                                                 {
                                                     if (onFinish != null) {
                                                         onFinish(true, null, null);
                                                     }
                                                     IssueDetailsWindow.Instance.openIssue(issue, ActiveIssueManager);
                                                 }));
                }
            } catch (Exception ex) {
                status.setError("Failed to find issue " + key, ex);
                Invoke(new MethodInvoker(delegate
                                             {
                                                 string message = "Unable to find issue " + key + " on server \"" + server.Name + "\"";
                                                 if (onFinish != null) {
                                                     onFinish(false, message, ex);
                                                 }
                                             }));
            }
        }


        private void buttonExpandAll_Click(object sender, EventArgs e) {
            expandIssuesTree();
        }

        private void buttonCollapseAll_Click(object sender, EventArgs e) {
            collapseIssuesTree();
        }

        private void expandIssuesTree() {
            if (issuesTree != null) {
                issuesTree.ExpandAll();
            }
        }

        private void collapseIssuesTree() {
            if (issuesTree != null) {
                issuesTree.CollapseAll();
            }
        }

        private void comboFind_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Enter) {
                addFindComboText(comboFind.Text);
            }
        }

        private void updateSearchingModel(string text) {
            searchingModel.Query = text;
        }

        private void addFindComboText(string text) {
            foreach (var item in comboFind.Items) {
                if (item.ToString().Equals(text)) {
                    return;
                }
            }
            if (text.Length > 0) {
                comboFind.Items.Add(text);
            }
        }

        private void comboFind_SelectedIndexChanged(object sender, EventArgs e) {
            updateSearchingModel(comboFind.Text);
        }

        private void comboFind_TextChanged(object sender, EventArgs e) {
            updateSearchingModel(comboFind.Text);
        }

        private void buttonAddFilter_Click(object sender, EventArgs e) {
            filtersTree.addCustomFilter();
        }

        private void buttonRemoveFilter_Click(object sender, EventArgs e) {
            JiraCustomFilterTreeNode node = filtersTree.SelectedNode as JiraCustomFilterTreeNode;
            filtersTree.removeCustomFilter(node);
        }

        private void buttonEditFilter_Click(object sender, EventArgs e) {
            JiraCustomFilterTreeNode node = filtersTree.SelectedNode as JiraCustomFilterTreeNode;
            filtersTree.editCustomFilter(node);
        }

        public JiraServer CurrentlySelectedServerOrDefault {
            get {
                JiraServer server = filtersTree.CurrentlySelectedServer ?? JiraServerModel.Instance.DefaultServer;
                if (server == null && JiraServerModel.Instance.getAllEnabledServers().Count == 1) {
                    server = JiraServerModel.Instance.getAllEnabledServers().First();
                }
                return server;
            }
        }

        public class SelectedIssueEventArgs : EventArgs {
            public SelectedIssueEventArgs(JiraIssue issue) {
                Issue = issue;
            }

            public JiraIssue Issue { get; private set; }
        }

        public void reinitialize(DTE dte) {
            if (dte != null) {
                PlvsUtils.updateKeyBindingsInformation(dte, new Dictionary<string, ToolStripItem>
                                                        {
                                                            { "Tools.FindIssue", buttonSearch },
                                                            { "Tools.CreateIssue", buttonCreate }
                                                        });
            }

            searchingModel.reinit(MODEL);
            registerIssueModelListener();
            if (InvokeRequired) {
                this.safeInvoke(new MethodInvoker(() => {
                    initIssuesTree();
                    reloadKnownJiraServers();
                    comboGroupBy.restoreSelectedIndex();
                }));
            } else {
                initIssuesTree();
                reloadKnownJiraServers();
                comboGroupBy.restoreSelectedIndex();
            }
        }

        public void shutdown() {}

        private void buttonGroupSubtasks_Click(object sender, EventArgs e) {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            store.storeParameter(GROUP_SUBTASKS_UNDER_PARENT, buttonGroupSubtasks.Checked ? 1 : 0);
        }

        private void buttonServerExplorer_Click(object sender, EventArgs e) {
            TreeNodeWithJiraServer node = filtersTree.SelectedNode as TreeNodeWithJiraServer;
            if (node != null) {
                JiraServerExplorer.showJiraServerExplorerFor(MODEL, node.Server, Facade);
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e) {
            try {
                PlvsUtils.runBrowser("http://confluence.atlassian.com/display/IDEPLUGIN/Using+JIRA+in+the+Visual+Studio+Connector");
            } catch (Exception ex) {
                Debug.WriteLine("TabJira.buttonHelp_Click() - exception: " + ex.Message);
            }
        }

        private void buttonStartStopWork_Click(object sender, EventArgs e) {
            runSelectedIssueAction(ActiveIssueManager.toggleActiveState);
        }

        private void tabJiraResize(object sender, EventArgs e) {
            resizeStatusBar();
        }

        private void resizeStatusBar() {
            if (buttonShowHideFilters == null) {
                return;
            }
            int height = jiraStatus.Height;
            jiraStatus.Size = new Size(
                statusStrip.Width 
                - (getMoreIssues.Visible ? getMoreIssues.Width : 0) 
                - (ActiveIssueManager != null ? ActiveIssueManager.ToolbarWidth : 0)
                - buttonShowHideFilters.Width - new ToolStripSeparator().Width, 
                height);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.dialogs.bamboo;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.store;
using Atlassian.plvs.ui.bamboo.treemodels;
using Atlassian.plvs.util;
using Atlassian.plvs.util.bamboo;
using Atlassian.plvs.windows;
using Timer=System.Timers.Timer;

namespace Atlassian.plvs.ui.bamboo {
    public partial class TabBamboo : UserControl, AddNewServerLink {

        private readonly Timer pollTimer;

        private readonly Timer infoTimer;

        private DateTime? lastPollTime;

        private DateTime? nextPollTime;

        private SplitContainer splitter;

        private BambooBuildTree buildTree;

        private BambooBuildHistoryTable buildHistoryTable;

        private readonly StatusLabel status;

        private bool? summaryStatusOk;

        private LinkLabel linkAddBambooServers;

        public TabBamboo() {
            InitializeComponent();

            status = new StatusLabel(statusStrip, statusLabel);

            pollTimer = new Timer();
            pollTimer.Elapsed += pollTimerElapsed;

            infoTimer = new Timer();
            infoTimer.Elapsed += infoTimerElapsed;

            GlobalSettings.SettingsChanged += globalSettingsChanged;

            notifyBuildStatus.Icon = Resources.bamboo_grey_161;
            notifyBuildStatus.Visible = false;
            notifyBuildStatus.BalloonTipTitle = "Atlassian Bamboo Notification";
            notifyBuildStatus.Text = "No build information retrieved yet";
            setupGroupByCombo();
        }

        public BambooServerFacade Facade { get { return BambooServerFacade.Instance; } }

        private void globalSettingsChanged(object sender, EventArgs e) {
            reinitialize();
        }

        public void init() {
            lastPollTime = null;
            pollTimer.Interval = 30000; // first poll in 30 seconds, next after a defined poll interval
            pollTimer.AutoReset = false;

            pollTimer.Enabled = false;
            pollTimer.Start();
            notifyBuildStatus.Visible = BambooServerModel.Instance.getAllEnabledServers().Count > 0;
            AtlassianPanel.Instance.BambooTabVisible = BambooServerModel.Instance.getAllEnabledServers().Count > 0;

            setNextPollTime();

            infoTimer.Interval = 10000;
            infoTimer.AutoReset = true;
            infoTimer.Start();

            this.safeInvoke(new MethodInvoker(initBuildTree));
        }

        private void setupGroupByCombo() {
            foreach (BambooBuildGroupByComboItem.GroupBy groupBy in Enum.GetValues(typeof(BambooBuildGroupByComboItem.GroupBy))) {
                comboGroupBy.Items.Add(new BambooBuildGroupByComboItem(groupBy, BambooBuildListModelImpl.Instance));
            }
            comboGroupBy.SelectedIndexChanged += comboGroupBySelectedIndexChanged;
        }

        private void comboGroupBySelectedIndexChanged(object sender, EventArgs e) {
            updateBuildTreeModel();
        }

        private void updateBuildTreeModel() {
            var item = comboGroupBy.SelectedItem as BambooBuildGroupByComboItem;
            if (item == null) {
                return;
            }
            
            var oldModel = buildTree.Model as AbstractBuildTreeModel;
            if (oldModel != null) {
                oldModel.shutdown();
            }

            var buildTreeModel = item.TreeModel;

            buildTree.Model = buildTreeModel;
            buildTreeModel.init();
        }

        private void initBuildTree() {
            if (splitter != null) {
                toolStripContainer.ContentPanel.Controls.Remove(splitter);
            }

            if (linkAddBambooServers != null) {
                Controls.Remove(linkAddBambooServers);
            }

            if (BambooServerModel.Instance.getAllServers().Count == 0) {
                toolStripContainer.Visible = false;

                linkAddBambooServers = new LinkLabel
                                       {
                                           Dock = DockStyle.Fill,
                                           Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238),
                                           Image = Resources.bamboo_blue_16_with_padding,
                                           Location = new Point(0, 0),
                                           Name = "linkAddBambooServers",
                                           Size = new Size(1120, 510),
                                           TabIndex = 0,
                                           TabStop = true,
                                           Text = "Add Bamboo Servers",
                                           TextAlign = ContentAlignment.MiddleCenter
                                       };

                linkAddBambooServers.LinkClicked += linkAddBambooServers_LinkClicked;

                Controls.Add(linkAddBambooServers);
            } else {
                toolStripContainer.Visible = true;

                splitter = new SplitContainer();
                
                buildTree = new BambooBuildTree(splitter) {
                    CollapseExpandManager =
                        new TreeNodeCollapseExpandStatusManager(
                            ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS))
                };

                updateBuildTreeModel();
                splitter.Panel1.Controls.Add(buildTree);

                buildHistoryTable = new BambooBuildHistoryTable(status);
                buildHistoryTable.showHistoryForBuild(null);

                splitter.SplitterDistance = Width - 300;
                splitter.Panel2.Controls.Add(buildHistoryTable);

                splitter.Orientation = Orientation.Vertical;
                splitter.Dock = DockStyle.Fill;
                toolStripContainer.ContentPanel.Controls.Add(splitter);

                updateBuildListButtons();

                buildTree.SelectionChanged += buildTreeSelectionChanged;
                buildTree.MouseDoubleClick += buildTreeMouseDoubleClick;
                buildTree.KeyPress += buildTreeKeyPress;

                int count = BambooServerModel.Instance.getAllEnabledServers().Count;
                if (count == 0) {
                    status.setInfo("No Bamboo servers enabled");
                } else {
                    showPollTimeInfo();
                }

                comboGroupBy.restoreSelectedIndex();
            }
        }

        private void buildTreeKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Enter) {
                openSelectedBuild();
            }
        }

        private void buildTreeMouseDoubleClick(object sender, MouseEventArgs e) {
            openSelectedBuild();
        }

        private void openSelectedBuild() {
            runOnSelectedNode(build => BuildDetailsWindow.Instance.openBuild(build));
        }

        private void buildTreeSelectionChanged(object sender, EventArgs e) {
            updateBuildListButtons();
            BuildNode bn = getSelectedBuildNode();
            buildHistoryTable.showHistoryForBuild(bn != null ? bn.Build : null);
        }

        private void updateBuildListButtons() {
            BuildNode n = getSelectedBuildNode();
            buttonViewInBrowser.Enabled = n != null;
            buttonOpen.Enabled = n != null;
            buttonRunBuild.Enabled = n != null;
        }

        public void shutdown() {
            notifyBuildStatus.Visible = false;
            pollTimer.Stop();
            pollTimer.Enabled = false;

            infoTimer.Stop();
            infoTimer.Enabled = false;

            showPollResults(null, null);
        }

        public void reinitialize() {
            shutdown();
            init();
        }

        void pollTimerElapsed(object sender, System.Timers.ElapsedEventArgs e) {
            lastPollTime = null;
            nextPollTime = null;
            Thread t = PlvsUtils.createThread(() => pollRunner(true));
            t.Start();
        }

        void infoTimerElapsed(object sender, System.Timers.ElapsedEventArgs e) {
            showPollTimeInfo();
        }

        private void pollRunner(bool rescheduleTimer) {
            List<BambooBuild> allBuilds = new List<BambooBuild>();
            List<Exception> allExceptions = new List<Exception>();

            ICollection<BambooServer> servers = BambooServerModel.Instance.getAllEnabledServers();
            if (servers == null || servers.Count == 0) return;

            status.setInfo("Polling all servers...");

            foreach (BambooServer server in servers) {
                try {
                    if (server.UseFavourites) {
                        ICollection<BambooBuild> builds = Facade.getLatestBuildsForFavouritePlans(server);
                        allBuilds.AddRange(builds);
                    } else if (server.PlanKeys != null && server.PlanKeys.Count > 0) {
                        ICollection<BambooBuild> builds = Facade.getLatestBuildsForPlanKeys(server, server.PlanKeys);
                        allBuilds.AddRange(builds);
                    }
                } catch (Exception e) {
                    allExceptions.Add(new Exception(server.Name + ": " + e.Message));
                }
            }
            
            try {
                Invoke(new MethodInvoker(delegate {
                                             if (rescheduleTimer) {
                                                 pollTimer.Interval = 1000*GlobalSettings.BambooPollingInterval;
                                                 setNextPollTime();
                                                 pollTimer.Start();
                                             }
                                             showPollResults(allBuilds, allExceptions);
                                         }));
            } catch (Exception e) {
                Debug.WriteLine("Exception while trying to show poll results: " + e.Message);
            }
        }

        private void setNextPollTime() {
            nextPollTime = DateTime.Now.AddMilliseconds(pollTimer.Interval);
        }

        private void showPollResults(ICollection<BambooBuild> builds, ICollection<Exception> exceptions) {
            if (buildTree == null) return;

            if (builds == null && exceptions == null) {
                BambooBuildListModelImpl.Instance.clear(true);
                buildHistoryTable.showHistoryForBuild(null);
                status.setInfo("No builds to poll found");
                return;
            }

            BambooBuildListModelImpl.Instance.clear(false);
            BambooBuildListModelImpl.Instance.addBuilds(builds);

            buildTree.restoreExpandCollapseStates();

            var buildNode = getSelectedBuildNode();
            buildHistoryTable.showHistoryForBuild(buildNode != null ? buildNode.Build : null );

            var haveExceptions = exceptions != null && exceptions.Count > 0;
            if (haveExceptions) {
                status.setError("Failed to poll some of the servers", exceptions);
            } else {
                lastPollTime = DateTime.Now;
                showPollTimeInfo();
            }
            bool? allpassing = null;
            if (builds != null) {
                allpassing = true;
                foreach (var b in builds.Where(b => allpassing.Value && b.Result != BambooBuild.BuildResult.SUCCESSFUL)) {
                    allpassing = false;
                }
            }
            bool allOk = allpassing != null && allpassing.Value;
            if (summaryStatusOk != null && summaryStatusOk == allOk) return;

            notifyBuildStatus.Icon = allpassing != null
                                         ? (allOk && !haveExceptions ? Resources.bamboo_green_161 : Resources.bamboo_red_161)
                                         : Resources.bamboo_grey_161;
            notifyBuildStatus.BalloonTipText = allOk && !haveExceptions
                                                   ? "All builds are passing"
                                                   : (haveExceptions  
                                                        ? "Failed to retrieve build information from some of the servers"
                                                        : "Some of the monitored builds failed");
            notifyBuildStatus.Text = notifyBuildStatus.BalloonTipText;
            notifyBuildStatus.BalloonTipIcon = allOk ? ToolTipIcon.Info : ToolTipIcon.Warning;
            notifyBuildStatus.ShowBalloonTip(30000);
            summaryStatusOk = allOk;
        }

        private void showPollTimeInfo() {
            if (status.HaveErrors) return;

            DateTime? last = lastPollTime;
            DateTime? next = nextPollTime;

            if (last != null && next != null) {
                var minutes = DateTime.Now.Subtract(last.Value).Minutes;
                status.setInfo("Last poll finished at "
                               + last.Value.ToShortDateString() + " " + last.Value.ToLongTimeString()
                               + " (" + minutes + (minutes == 1 ? " minute" : " minutes") + " ago)" + getNextPollTimeInfo());
            } else if (next != null) {
                var servers = BambooServerModel.Instance.getAllEnabledServers();
                var count = servers != null ? servers.Count : 0;
                if (count > 0) {
                    status.setInfo("" + count + " Bamboo " + (count == 1 ? "server" : "servers") + " enabled" + getNextPollTimeInfo());
                } else {
                    status.setInfo("No Bamboo servers enabled");
                }
            }
        }

        private string getNextPollTimeInfo() {
            if (nextPollTime == null) {
                return "";
            }
            TimeSpan nextPoll = nextPollTime.Value.Subtract(DateTime.Now);
            StringBuilder sb = new StringBuilder();
            if (nextPoll.Hours > 0) {
                sb.Append(nextPoll.Hours).Append(nextPoll.Hours == 1 ? " hour" : " hours");
            }
            if (nextPoll.Minutes > 0) {
                if (sb.Length > 0) {
                    sb.Append(", ");
                }
                sb.Append(nextPoll.Minutes).Append(nextPoll.Minutes == 1 ? " minute" : " minutes");
            }
            if (nextPoll.Seconds > 0) {
                if (sb.Length > 0) {
                    sb.Append(", ");
                }
                sb.Append(nextPoll.Seconds).Append(nextPoll.Seconds == 1 ? " second" : " seconds");
            }
            return ", next poll in " + sb;
        }

        private void buttonPollClick(object sender, EventArgs e) {
            var t = PlvsUtils.createThread(() => pollRunner(false));
            t.Start();
        }

        private void buttonOpenClick(object sender, EventArgs e) {
            runOnSelectedNode(build => BuildDetailsWindow.Instance.openBuild(build));
        }

        private void buttonViewInBrowserClick(object sender, EventArgs e) {
            runOnSelectedNode(delegate(BambooBuild b) {
                                  try {
                                      PlvsUtils.runBrowser(b.Server.Url + "/browse/" + b.Key);
                                  } catch (Exception ex) {
                                      Debug.WriteLine("buttonViewInBrowser_Click - exception: " + ex.Message);
                                  }
                                  UsageCollector.Instance.bumpBambooBuildsOpen();
                              });
        }

        private void buttonRunBuildClick(object sender, EventArgs e) {
            runOnSelectedNode(delegate(BambooBuild b) {
                                  string key = BambooBuildUtils.getPlanKey(b);
                                  status.setInfo("Adding build " + key + " to the build queue...");
                                  Thread t = PlvsUtils.createThread(() => runBuildWorker(b, key));
                                  t.Start();
                              });
        }

        private void runBuildWorker(BambooBuild b, string key) {
            try {
                Facade.runBuild(b.Server, key);
                status.setInfo("Added build " + key + " to the build queue");
                UsageCollector.Instance.bumpBambooBuildsOpen();
                Thread t = PlvsUtils.createThread(() => pollRunner(true));
                t.Start();
            } catch (Exception ex) {
                status.setError("Failed to add build " + key + " to the build queue", ex);
            }
        }

        private delegate void SelectedBuildAction(BambooBuild b);

        private void runOnSelectedNode(SelectedBuildAction action) {
            BuildNode n = getSelectedBuildNode();
            if (n != null) action(n.Build);
        }

        private BuildNode getSelectedBuildNode() {
            TreeNodeAdv node = buildTree.SelectedNode;
            BuildNode n = node == null ? null : node.Tag as BuildNode;
            return n;
        }

        public event EventHandler<EventArgs> AddNewServerLinkClicked;

        private void linkAddBambooServers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (AddNewServerLinkClicked != null) {
                AddNewServerLinkClicked(this, new EventArgs());
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e) {
            try {
                PlvsUtils.runBrowser("http://confluence.atlassian.com/display/IDEPLUGIN/Using+Bamboo+in+the+Visual+Studio+Connector");
            } catch (Exception ex) {
                Debug.WriteLine("TabBamboo.buttonHelp_Click() - exception: " + ex.Message);
            }
        }

        private void buttonFindBuild_Click(object sender, EventArgs e) {
            SearchBuild dlg = new SearchBuild(status);
            dlg.ShowDialog();
        }
    }
}

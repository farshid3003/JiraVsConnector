using System;
using System.Linq;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.ui.jira.issues;
using EnvDTE;

namespace Atlassian.plvs.ui.bamboo {
    public sealed partial class BuildDetailsWindow : ToolWindowFrame, ToolWindowStateMonitor {
        public static BuildDetailsWindow Instance { get; private set; }

        public Solution Solution { get; set; }

        public event EventHandler<EventArgs> ToolWindowShown;
        public event EventHandler<EventArgs> ToolWindowHidden;

        public BuildDetailsWindow() {
            InitializeComponent();

            Instance = this;

            ShownOrHidden += (s, e) => notifyWindowVisibility(e.Visible);

            tabBuilds.ImageList = new ImageList();
            tabBuilds.ImageList.Images.Add(Resources.icn_plan_passed);
            tabBuilds.ImageList.Images.Add(Resources.icn_plan_failed);
            tabBuilds.ImageList.Images.Add(Resources.icn_plan_disabled);
        }

        public void clearAllBuilds() {
            if (ToolWindowHidden != null) {
                ToolWindowHidden(this, new EventArgs());
            }
            tabBuilds.TabPages.Clear();
        }

        public void openBuild(BambooBuild build) {
            FrameVisible = true;

            string key = getBuildTabKey(build);
            if (!tabBuilds.TabPages.ContainsKey(key)) {
                TabPage buildTab = new TabPage { Name = key, Text = build.Key };
//                BuildDetailsPanel buildPanel = new BuildDetailsPanel(Solution, build, buildTab, this, buttonCloseClicked);
                BuildDetailsPanel buildPanel = new BuildDetailsPanel(Solution, build, buildTab, this);
                buildTab.Controls.Add(buildPanel);
                buildPanel.Dock = DockStyle.Fill;
                buildTab.ToolTipText = Resources.MIDDLE_CLICK_TO_CLOSE;
                tabBuilds.TabPages.Add(buildTab);
                tabBuilds.PostRemoveTabPage = idx => {
                    if (tabBuilds.TabPages.Count == 0) {
                        Instance.FrameVisible = false;
                    }
                };
            }
            tabBuilds.SelectTab(key);
            UsageCollector.Instance.bumpBambooBuildsOpen();
        }

//        private void buttonCloseClicked(TabPage tab) {
//            tabBuilds.TabPages.Remove(tab);
//            if (tabBuilds.TabPages.Count == 0) {
//                Instance.FrameVisible = false;
//            }
//        }

        private static string getBuildTabKey(BambooBuild build) {
            return build.Server.GUID + build.Key;
        }

        private void notifyWindowVisibility(bool visible) {
            if (visible) {
                if (ToolWindowShown != null) {
                    ToolWindowShown(this, new EventArgs());
                }
            } else {
                if (ToolWindowHidden != null) {
                    ToolWindowHidden(this, new EventArgs());
                }
            }
        }

        public void setMyTabIconFromBuildResult(BambooBuild.BuildResult result, TabPage tab) {
            switch (result) {
                case BambooBuild.BuildResult.SUCCESSFUL:
                    tab.ImageIndex = 0;
                    break;
                case BambooBuild.BuildResult.FAILED:
                    tab.ImageIndex = 1;
                    break;
                default:
                    tab.ImageIndex = 2;
                    break;
            }
        }
    }
}

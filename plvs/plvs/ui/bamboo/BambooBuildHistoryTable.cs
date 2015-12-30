using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.util;
using Atlassian.plvs.util.bamboo;

namespace Atlassian.plvs.ui.bamboo {
    public sealed partial class BambooBuildHistoryTable : UserControl {

        private const int MAX_HISTORY = 10;

        private BambooBuild lastBuild;

        private int generation;
        private readonly StatusLabel statusLabel;

        public BambooBuildHistoryTable(StatusLabel statusLabel) {
            this.statusLabel = statusLabel;
            InitializeComponent();
            Dock = DockStyle.Fill;
            updateButtons(false);
        }

        private void updateButtons(bool enabled) {
            buttonOpen.Enabled = enabled;
            buttonViewInBrowser.Enabled = enabled;
        }

        public void showHistoryForBuild(BambooBuild build) {
            lastBuild = build;
            updateButtons(false);

            ++generation;
            grid.Rows.Clear();
            
            if (build == null) return;

            Thread t = PlvsUtils.createThread(() => getHistoryWorker(lastBuild, generation));
            t.Start();
        }

        private void getHistoryWorker(BambooBuild build, int myGeneration) {
            try {
                ICollection<BambooBuild> builds = 
                    BambooServerFacade.Instance.getLastNBuildsForPlan(build.Server, BambooBuildUtils.getPlanKey(build), MAX_HISTORY);
                this.safeInvoke(new MethodInvoker(delegate {
                                                      if (myGeneration != generation) return;
                                                      fillHistory(builds);
                                                  }));
            } catch (Exception e) {
                statusLabel.setError("Unable to retrieve build history", e);
            }
        }

        private void fillHistory(IEnumerable<BambooBuild> builds) {
            int i = 0;
            foreach (object[] b in
                builds.Select(build => new BuildNode(build)).Select(bn => new object[] {bn.Icon, bn.Key, bn.Completed, bn.Build})) {
                grid.Rows.Insert(i, b);
                ++i;
            }
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            openBuild(getSelectedBuild());
        }

        private void buttonOpen_Click(object sender, EventArgs e) {
            openBuild(getSelectedBuild());
        }

        private void grid_SelectionChanged(object sender, EventArgs e) {
            updateButtons(grid.SelectedRows.Count > 0);
        }

        private void buttonViewInBrowser_Click(object sender, EventArgs e) {
            BambooBuild b = getSelectedBuild();
            if (b == null) return;
            try {
                PlvsUtils.runBrowser(b.Server.Url + "/build/viewBuildResults.action?buildKey="
                              + BambooBuildUtils.getPlanKey(b) + "&buildNumber=" + b.Number);
            } catch (Exception ex) {
                Debug.WriteLine("buttonViewInBrowser_Click - exception: " + ex.Message);
            }
            UsageCollector.Instance.bumpBambooBuildsOpen();

        }

        private void grid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                openBuild(getSelectedBuild());
            }
        }

        private BambooBuild getSelectedBuild() {
            if (grid.SelectedRows.Count == 0) return null;
            return grid.SelectedRows[0].Cells[3].Value as BambooBuild;
        }

        private void grid_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                e.Handled = true;
            }
        }

        private static void openBuild(BambooBuild build) {
            if (build == null) return;
            BuildDetailsWindow.Instance.openBuild(build);
        }
    }
}

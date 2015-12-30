using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.api.bamboo.rest;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs.bamboo {
    public partial class AddOrEditBambooServer : Form {
        private readonly BambooServerFacade facade;
        private static int invocations;

        private readonly bool editing;

        private readonly BambooServer server;

        private bool gettingPlans;

        private readonly List<string> planKeys = new List<string>();

        public AddOrEditBambooServer(BambooServer server, BambooServerFacade facade) {
            this.facade = facade;
            InitializeComponent();

            editing = server != null;

            this.server = new BambooServer(server);

            Text = editing ? "Edit Bamboo Server" : "Add Bamboo Server";
            buttonAddOrEdit.Text = editing ? "Apply Changes" : "Add Server";

            if (editing) {
                if (server != null) {
                    name.Text = server.Name;
                    url.Text = server.Url;
                    user.Text = server.UserName;
                    password.Text = server.Password;

                    radioUseFavourites.Checked = server.UseFavourites;
                    radioSelectManually.Checked = !server.UseFavourites;
                    if (server.PlanKeys != null) {
                        planKeys.AddRange(server.PlanKeys);
                    }

                    checkShowBranches.Checked = server.ShowBranches;
                    checkMyBranches.Checked = server.ShowMyBranchesOnly;
                    checkShowBranches.Enabled = false;
                    checkMyBranches.Enabled = false;

                    getServerVersion();

                    checkEnabled.Checked = server.Enabled;
                    checkShared.Checked = server.IsShared;
                    checkDontUseProxy.Checked = server.NoProxy;
                }
            } else {
                ++invocations;
                name.Text = "Bamboo Server #" + invocations;
                buttonAddOrEdit.Enabled = false;

                radioUseFavourites.Checked = true;
                buttonGetBuilds.Enabled = false;
                checkedListBuilds.Enabled = false;

                checkEnabled.Checked = true;
                checkShared.Checked = false;
                checkDontUseProxy.Checked = false;
                checkShowBranches.Checked = true;
                checkShowBranches.Enabled = true;
                checkMyBranches.Enabled = true;
            }

            StartPosition = FormStartPosition.CenterParent;
            toolTip.SetToolTip(checkMyBranches, "Only show branches where the last commit is mine");
        }

        public override sealed string Text {
            get { return base.Text; }
            set { base.Text = value; }
        }

        protected override void OnLoad(EventArgs e) {
            if (editing && server != null && !server.UseFavourites) {
                getPlans();
            }
        }

        private void buttonAddOrEditClick(object sender, EventArgs e) {
            fillServerData();

            server.UseFavourites = radioUseFavourites.Checked;

            planKeys.Clear();

            CheckedListBox.CheckedIndexCollection indices = checkedListBuilds.CheckedIndices;
            for (int i = 0; i < checkedListBuilds.Items.Count; i++) {
                if (indices.Contains(i)) {
                    planKeys.Add(((BambooPlan) checkedListBuilds.Items[i]).Key);
                }
            }

            server.PlanKeys = planKeys;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void fillServerData() {
            server.Name = name.Text.Trim();
            string fixedUrl = url.Text.Trim();
            if (!(fixedUrl.StartsWith("http://") || fixedUrl.StartsWith("https://"))) {
                fixedUrl = "http://" + fixedUrl;
            }
            if (fixedUrl.EndsWith("/")) {
                fixedUrl = fixedUrl.Substring(0, fixedUrl.Length - 1);
            }
            server.Url = fixedUrl;
            server.UserName = user.Text.Trim();
            server.Password = password.Text;
            server.Enabled = checkEnabled.Checked;
            server.IsShared = checkShared.Checked;
            server.NoProxy = checkDontUseProxy.Checked;
            server.ShowBranches = checkShowBranches.Checked;
            server.ShowMyBranchesOnly = checkMyBranches.Checked;
        }

        private void nameTextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void urlTextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void userTextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void checkIfValid() {
            buttonAddOrEdit.Enabled = name.Text.Trim().Length > 0 && url.Text.Trim().Length > 0 &&
                                      user.Text.Trim().Length > 0 && buttonCancel.Enabled;
            buttonTestConnection.Enabled = buttonAddOrEdit.Enabled && buttonCancel.Enabled;
            buttonGetBuilds.Enabled = buttonAddOrEdit.Enabled && radioSelectManually.Checked;
            checkedListBuilds.Enabled = buttonAddOrEdit.Enabled && radioSelectManually.Checked;
        }

        public BambooServer Server {
            get { return server; }
        }

        private void addOrEditServerKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Escape) return;
            if (gettingPlans) return;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void radioUseFavouritesCheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void radioSelectManuallyCheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void buttonGetBuildsClick(object sender, EventArgs e) {
            getPlans();
        }

        private void getPlans() {
            gettingPlans = true;
//            buttonCancel.Enabled = false;
            buttonAddOrEdit.Enabled = false;
            radioUseFavourites.Enabled = false;
            radioSelectManually.Enabled = false;
            name.Enabled = false;
            url.Enabled = false;
            user.Enabled = false;
            password.Enabled = false;
            checkEnabled.Enabled = false;
            checkShared.Enabled = false;
            checkDontUseProxy.Enabled = false;

            buttonGetBuilds.Enabled = false;
            buttonTestConnection.Enabled = false;

            Thread t = PlvsUtils.createThread(getPlansWorker);
            t.Start();
        }

        private void getServerVersion() {
            var t = PlvsUtils.createThread(() => {
                try {
                    var buildNumber = facade.getServerBuildNumber(server);
                    this.safeInvoke(new MethodInvoker(delegate {
                        checkShowBranches.Enabled = RestSession.BUILD_NUMBER_4_0 <= buildNumber;
                        checkMyBranches.Enabled = RestSession.BUILD_NUMBER_5_0 <= buildNumber;
                    }));
                } catch(Exception e) {
                    this.safeInvoke(new MethodInvoker(() => PlvsUtils.showError("Unable to retrieve server version", e)));
                }

            });
            t.Start();
        }

        private void getPlansWorker() {
            fillServerData();
            try {
                ICollection<BambooPlan> plans  = facade.getPlanList(server);
                this.safeInvoke(new MethodInvoker(() => fillPlanList(plans)));
            } catch (Exception e) {
                this.safeInvoke(new MethodInvoker(() => PlvsUtils.showError("Unable to retrieve build plans", e)));
            } finally {
                gettingPlans = false;

                this.safeInvoke(new MethodInvoker(delegate {
//                    buttonCancel.Enabled = true;
                    radioUseFavourites.Enabled = true;
                    radioSelectManually.Enabled = true;
                    name.Enabled = true;
                    url.Enabled = true;
                    user.Enabled = true;
                    password.Enabled = true;
                    checkEnabled.Enabled = true;
                    checkShared.Enabled = true;
                    checkDontUseProxy.Enabled = true;
                    checkIfValid();
                }));
            }
        }

        private void fillPlanList(IEnumerable<BambooPlan> plans) {
            if (plans == null) {
                return;
            }
            checkedListBuilds.Items.Clear();
            int i = 0;
            foreach (var plan in plans) {
                checkedListBuilds.Items.Add(plan);
                BambooPlan planCopy = plan;
                if (planKeys.Any(key => planCopy.Key.Equals(key))) {
                    checkedListBuilds.SetItemChecked(i, true);
                }
                ++i;
            }
        }

        private void checkEnabledCheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void buttonTestConnectionClick(object sender, EventArgs e) {
            fillServerData();
            new TestBambooConnection(facade, server).ShowDialog();
        }

        private void checkUseProxyCheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void checkSharedCheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void checkShowBranchesCheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void checkMyBranchesCheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }
    }
}
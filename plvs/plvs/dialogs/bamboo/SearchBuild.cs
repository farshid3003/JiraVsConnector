using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.bamboo;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs.bamboo {
    public sealed partial class SearchBuild : Form {
        public StatusLabel status;

        private static readonly Regex buildKeyRegex = new Regex(@"^\S+-\d+$");

        public SearchBuild(StatusLabel status) {
            this.status = status;
            InitializeComponent();

            Text = "Find Build";

            buttonOk.Enabled = false;
            StartPosition = FormStartPosition.CenterParent;
        }

        private void textQueryString_TextChanged(object sender, EventArgs e) {
            bool isMatch = buildKeyRegex.IsMatch(textQueryString.Text.Trim());
            buttonOk.Enabled = isMatch;
            textQueryString.ForeColor = isMatch ? Color.Black : Color.Red;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            executeSearchAndClose();
        }

        private void textQueryString_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Enter) return;
            executeSearchAndClose();
        }

        private void executeSearchAndClose() {
            string buildKey = textQueryString.Text.Trim().ToUpper();
            if (buildKey.Length == 0) return;
            bool isMatch = buildKeyRegex.IsMatch(buildKey);
            if (!isMatch) return;

            setAllEnabled(false);
            ICollection<BambooServer> servers = BambooServerModel.Instance.getAllEnabledServers();
            Thread t = PlvsUtils.createThread(() => findBuildWorker(servers, buildKey));
            t.Start();
        }

        private void setAllEnabled(bool enabled) {
            foreach (Control ctrl in new Control[] { textQueryString, buttonOk, buttonCancel }) {
                ctrl.Enabled = enabled;
            }
        }

        private void findBuildWorker(IEnumerable<BambooServer> servers, string buildKey) {
            List<BambooBuild> foundBuilds = new List<BambooBuild>();
            List<Exception> errors = new List<Exception>();
            foreach (BambooServer server in servers) {
                try {
                    status.setInfo("Searching for build " + buildKey + " on server \"" + server.Name + "\"");
                    BambooBuild build = BambooServerFacade.Instance.getBuildByKey(server, buildKey);
                    foundBuilds.Add(build);
                } catch (Exception e) {
                    errors.Add(e);
                }
            }
            this.safeInvoke(new MethodInvoker(delegate {
                                                  if (foundBuilds.Count > 0) {
                                                      status.setInfo("");
                                                      foreach (BambooBuild build in foundBuilds) {
                                                          BuildDetailsWindow.Instance.openBuild(build);
                                                      }
                                                  } else if (errors.Count > 0) {
                                                      PlvsUtils.showErrors("Build key " + buildKey + " not found", errors);
                                                      status.setError("Build key " + buildKey + " not found", errors);
                                                  } else if (foundBuilds.Count == 0) {
                                                      PlvsUtils.showError("Build key " + buildKey + " not found", null);
                                                      status.setInfo("Build key " + buildKey + " not found");
                                                  }
                                                  DialogResult = DialogResult.OK;
                                                  Close();
                                              }));
        }

        private void searchBuildKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape && buttonCancel.Enabled) {
                Close();
            }
        }
    }
}
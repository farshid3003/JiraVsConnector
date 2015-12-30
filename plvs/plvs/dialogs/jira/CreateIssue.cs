using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models;
using Atlassian.plvs.store;
using Atlassian.plvs.ui;
using Atlassian.plvs.util;
using Atlassian.plvs.windows;
using Timer = System.Threading.Timer;

namespace Atlassian.plvs.dialogs.jira {
    public partial class CreateIssue : Form {
        private readonly JiraServer server;

        private JiraIssue parent = null;

        private const string PROJECT = "createIssueDialog_selectedProject_";
        private const string ISSUE_TYPE = "createIssueDialog_selectedIssueType_";
        private const string PRIORITY = "createIssueDialog_selectedPriority_";
        private const string COMPS_SIZE = "createIssueDialog_selectedComponentsSize_";
        private const string COMPS_SEL = "createIssueDialog_selectedComponent_";
        private const string AFFECTS_SIZE = "createIssueDialog_selectedAffectsVersionsSize_";
        private const string AFFECTS_SEL = "createIssueDialog_selectedAffectsVersion_";
        private const string FIXES_SIZE = "createIssueDialog_selectedFixVersionsSize_";
        private const string FIXES_SEL = "createIssueDialog_selectedFixVersion_";

        private bool initialUpdate;

        private static CreateIssue instance;

        public static void createDialogOrBringToFront(JiraServer server) {
            createDialogOrBringToFront(server, null);
        }

        public static void createDialogOrBringToFront(JiraServer server, JiraIssue parent) {
            if (instance == null || parent != instance.parent) {
                if (instance != null) {
                    instance.Close();
                }
                instance = new CreateIssue(server, parent);
                instance.Show();
            } else {
                instance.BringToFront();
            }
        }

        private CreateIssue(JiraServer server, JiraIssue parent) {
            this.server = server;
            this.parent = parent;
            InitializeComponent();

            if (parent == null) {
                labelParentIssueKey.Visible = false;
            }
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            buttonCreate.Enabled = false;

            SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(server);
            if (projects != null) {
                foreach (var project in projects.Values) {
                    comboProjects.Items.Add(project);
                }

                List<JiraNamedEntity> priorities = JiraServerCache.Instance.getPriorities(server);
                if (priorities != null) {
                    ImageList imageList = new ImageList();
                    int i = 0;
                    foreach (var priority in priorities) {
                        imageList.Images.Add(ImageCache.Instance.getImage(server, priority.IconUrl).Img);
                        comboPriorities.Items.Add(new ComboBoxWithImagesItem<JiraNamedEntity>(priority, i));
                        ++i;
                    }
                    comboPriorities.ImageList = imageList;

                    if (priorities.Count > 0) {
                        int idx = store.loadParameter(PRIORITY + server.GUID, -1);
                        if (idx != -1 && comboPriorities.Items.Count > idx) {
                            comboPriorities.SelectedIndex = idx;
                        } else {
                            comboPriorities.SelectedIndex = priorities.Count / 2;
                        }
                    }

                    if (projects.Count > 0) {
                        int idx = store.loadParameter(PROJECT + server.GUID, -1);
                        if (idx != -1 && comboProjects.Items.Count > idx) {
                            initialUpdate = true;
                            comboProjects.SelectedIndex = idx;
                        }
                    }
                }

                if (parent != null) {
                    comboProjects.Visible = false;
                    labelProject.Visible = false;
                    labelParentIssueKey.Visible = true;
                    labelParentIssueKey.Text = "Parent issue key: " + parent.Key;
                    foreach (var item in comboProjects.Items.Cast<object>()
                        .Where(item => ((JiraProject) item).Key.Equals(parent.ProjectKey))) {
                        comboProjects.SelectedItem = item;
                        break;
                    }
                }

            }

            StartPosition = FormStartPosition.CenterParent;

            jiraAssigneePicker.init(server, null, true);

            textDescription.Server = server;
            textDescription.Facade = SmartJiraServerFacade.Instance;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void comboProjects_SelectedIndexChanged(object sender, EventArgs e) {
            setAllEnabled(false);
            textDescription.Project = (JiraProject)comboProjects.SelectedItem;
            startProjectUpdateThread();
        }

        private void startProjectUpdateThread() {
            JiraProject project = (JiraProject) comboProjects.SelectedItem;
            Thread t = PlvsUtils.createThread(() => projectUpdateWorker(project));
            t.Start();
        }

        private void projectUpdateWorker(JiraProject project) {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            List<JiraNamedEntity> issueTypes = parent != null
                ? SmartJiraServerFacade.Instance.getSubtaskIssueTypes(server, project)
                : SmartJiraServerFacade.Instance.getIssueTypes(server, project);
            List<JiraNamedEntity> comps = SmartJiraServerFacade.Instance.getComponents(server, project);
            List<JiraNamedEntity> versions = SmartJiraServerFacade.Instance.getVersions(server, project);
            // newest versions first
            versions.Reverse();
            this.safeInvoke(new MethodInvoker(delegate {
                                         fillIssueTypes(issueTypes, store);
                                         fillComponents(comps, store);
                                         fillVersions(versions, store);
                                         setAllEnabled(true);
                                         updateButtons();
                                         initialUpdate = false;
                                     }));
        }

        private void fillVersions(IEnumerable<JiraNamedEntity> versions, ParameterStore store) {
            listAffectsVersions.Items.Clear();
            listFixVersions.Items.Clear();
            foreach (var version in versions) {
                listAffectsVersions.Items.Add(version);
                listFixVersions.Items.Add(version);
            }
            if (!initialUpdate) {
                return;
            }
            int cnt = store.loadParameter(AFFECTS_SIZE + server.GUID, 0);
            if (cnt > 0) {
                for (int i = 0; i < cnt; ++i) {
                    int sel = store.loadParameter(AFFECTS_SEL + i + "_" + server.GUID, -1);
                    if (sel == -1) {
                        continue;
                    }
                    if (listAffectsVersions.Items.Count > sel) {
                        listAffectsVersions.SelectedIndices.Add(sel);
                    }
                }
            }
            cnt = store.loadParameter(FIXES_SIZE + server.GUID, 0);
            if (cnt > 0) {
                for (int i = 0; i < cnt; ++i) {
                    int sel = store.loadParameter(FIXES_SEL + i + "_" + server.GUID, -1);
                    if (sel == -1) {
                        continue;
                    }
                    if (listFixVersions.Items.Count > sel) {
                        listFixVersions.SelectedIndices.Add(sel);
                    }
                }
            }
        }

        private void fillComponents(IEnumerable<JiraNamedEntity> comps, ParameterStore store) {
            listComponents.Items.Clear();
            foreach (var comp in comps) {
                listComponents.Items.Add(comp);
            }
            if (!initialUpdate) {
                return;
            }
            int cnt = store.loadParameter(COMPS_SIZE + server.GUID, 0);
            if (cnt <= 0) {
                return;
            }
            for (int i = 0; i < cnt; ++i) {
                int sel = store.loadParameter(COMPS_SEL + i + "_" + server.GUID, -1);
                if (sel == -1) {
                    continue;
                }
                if (listComponents.Items.Count > sel) {
                    listComponents.SelectedIndices.Add(sel);
                }
            }
        }

        private void fillIssueTypes(ICollection<JiraNamedEntity> issueTypes, ParameterStore store) {
            ImageList imageList = new ImageList();
            comboTypes.Items.Clear();
            int i = 0;
            foreach (var type in issueTypes) {
                ComboBoxWithImagesItem<JiraNamedEntity> item = new ComboBoxWithImagesItem<JiraNamedEntity>(type, i);
                imageList.Images.Add(ImageCache.Instance.getImage(server, type.IconUrl).Img);
                comboTypes.Items.Add(item);
                ++i;
            }
            comboTypes.ImageList = imageList;

            textDescription.IssueType = -1;

            if (initialUpdate) {
                if (issueTypes.Count > 0) {
                    int idx = store.loadParameter(ISSUE_TYPE + server.GUID, -1);
                    if (idx != -1 && comboTypes.Items.Count > idx) {
                        comboTypes.SelectedIndex = idx;
                    }
                }
            }
        }

        private void setAllEnabled(bool enabled) {
            comboProjects.Enabled = enabled;
            comboTypes.Enabled = enabled;
            listAffectsVersions.Enabled = enabled;
            listFixVersions.Enabled = enabled;
            listComponents.Enabled = enabled;
            comboPriorities.Enabled = enabled;
            textSummary.Enabled = enabled;
            textDescription.Enabled = enabled;
            jiraAssigneePicker.Enabled = enabled;
            buttonCreate.Enabled = enabled;
            buttonCreateAndClose.Enabled = enabled;
        }

        private void updateButtons() {
            buttonCreate.Enabled =
                textSummary.Text.Length > 0
                && comboProjects.SelectedItem != null
                && comboTypes.SelectedItem != null
                && comboPriorities.SelectedItem != null;
            buttonCreateAndClose.Enabled = buttonCreate.Enabled;
        }

        private void textSummary_TextChanged(object sender, EventArgs e) {
            updateButtons();
        }

        private void comboTypes_SelectedIndexChanged(object sender, EventArgs e) {
            object selectedItem = comboTypes.SelectedItem;
            if (selectedItem != null) {
                textDescription.IssueType = ((ComboBoxWithImagesItem<JiraNamedEntity>)selectedItem).Value.Id;
            } else {
                textDescription.IssueType = -1;
            }
            updateButtons();
        }

        private void buttonCreate_Click(object sender, EventArgs e) {
            createIssue(true);
        }

        private void buttonCreateAndClose_Click(object sender, EventArgs e) {
            createIssue(false);
        }

        private void createIssue(bool keepDialogOpen) {
            saveSelectedValues();
            JiraIssue issue = createIssueTemplate();
            setAllEnabled(false);
            buttonCancel.Enabled = false;
            Thread t = PlvsUtils.createThread(() => createIssueWorker(issue, keepDialogOpen));
            t.Start();
        }

        private void saveSelectedValues() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            store.storeParameter(PROJECT + server.GUID, comboProjects.SelectedIndex);
            store.storeParameter(ISSUE_TYPE + server.GUID, comboTypes.SelectedIndex);
            store.storeParameter(PRIORITY + server.GUID, comboPriorities.SelectedIndex);
            store.storeParameter(COMPS_SIZE + server.GUID, listComponents.SelectedIndices.Count);
            int i = 0;
            foreach (int index in listComponents.SelectedIndices) {
                store.storeParameter(COMPS_SEL + (i++) + "_" + server.GUID, index);
            }

            store.storeParameter(AFFECTS_SIZE + server.GUID, listAffectsVersions.SelectedIndices.Count);
            i = 0;
            foreach (int index in listAffectsVersions.SelectedIndices) {
                store.storeParameter(AFFECTS_SEL + (i++) + "_" + server.GUID, index);
            }
            store.storeParameter(FIXES_SIZE + server.GUID, listFixVersions.SelectedIndices.Count);
            i = 0;
            foreach (int index in listFixVersions.SelectedIndices) {
                store.storeParameter(FIXES_SEL + (i++) + "_" + server.GUID, index);
            }
        }

        private JiraIssue createIssueTemplate() {
            JiraIssue issue = new JiraIssue
                              {
                                  Summary = textSummary.Text,
                                  Description = textDescription.Text,
                                  ProjectKey = parent != null ? parent.ProjectKey : ((JiraProject) comboProjects.SelectedItem).Key,
                                  IssueTypeId =
                                      ((ComboBoxWithImagesItem<JiraNamedEntity>) comboTypes.SelectedItem).Value.Id,
                                  PriorityId =
                                      ((ComboBoxWithImagesItem<JiraNamedEntity>) comboPriorities.SelectedItem).Value
                                      .Id
                              };

            if (listComponents.SelectedItems.Count > 0) {
                List<string> comps = (from object comp in listComponents.SelectedItems select comp.ToString()).ToList();
                issue.Components = comps;
            }

            if (listAffectsVersions.SelectedItems.Count > 0) {
                List<string> affects = (from object ver in listAffectsVersions.SelectedItems select ver.ToString()).ToList();
                issue.Versions = affects;
            }

            if (listFixVersions.SelectedItems.Count > 0) {
                List<string> fixes = (from object fix in listFixVersions.SelectedItems select fix.ToString()).ToList();
                issue.FixVersions = fixes;
            }

            string assignee = jiraAssigneePicker.Value;
            if (assignee.Length > 0) {
                issue.Assignee = assignee;
            }

            if (parent != null) {
                issue.ParentKey = parent.Key;
            }
            return issue;
        }

        private void createIssueWorker(JiraIssue issue, bool keepDialogOpen) {
            try {
                string key = SmartJiraServerFacade.Instance.createIssue(server, issue);
                this.safeInvoke(new MethodInvoker(delegate {
                                                      setAllEnabled(true);
                                                      buttonCancel.Enabled = true;
                                                      if (!keepDialogOpen) {
                                                          Close();
                                                      } else {
                                                          if (keepDialogOpen) {
                                                              textSummary.Text = "";
                                                              textDescription.Text = "";
                                                          }
                                                      }
                                                      AtlassianPanel.Instance.Jira.findAndOpenIssue(key, delegate {
                                                          if (keepDialogOpen) {
                                                              bringDialogToFront();
                                                          }
                                                      });
                }));
            } catch (Exception e) {
                this.safeInvoke(new MethodInvoker(delegate {
                                                      PlvsUtils.showError("Unable to create issue", e);
                                                      setAllEnabled(true);
                                                      buttonCancel.Enabled = true;
                                                  }));
            }
        }

        // I have no idea why just calling BringToFront does not work, but it does not. Go figure
        private void bringDialogToFront() {
            try {
                TopMost = true;
                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer { Interval = 100 };
                t.Tick += (s, e) => { TopMost = false; t.Stop(); };
                t.Start();
            } catch (Exception e) {
                // in case somebody closes the dialog before the timer fires up
                Debug.WriteLine("CreateIssue.bringDialogToFront() - exception: " + e.Message);
            }
        }

        private void createIssueKeyPress(object sender, KeyPressEventArgs e) {
            if (buttonCancel.Enabled && e.KeyChar == (char)Keys.Escape) {
                Close();
            }
        }

        protected override void OnClosed(EventArgs e) {
            instance = null;
        }
    }
}
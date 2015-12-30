using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs.jira {
    public sealed partial class EditCustomFilter : Form {
        private readonly JiraServer server;
        private readonly JiraCustomFilter filter;
        
        private const string NAME_COLUMN = "Name";

        public bool Changed { get; private set; }

        public EditCustomFilter(JiraServer server, JiraCustomFilter filter, bool editMode) {

            this.server = server;
            this.filter = filter;

            InitializeComponent();

            Text = editMode ? "Edit Local Filter" : "Add Local Filter";

            listViewProjects.Columns.Add(NAME_COLUMN, listViewProjects.Width - 30, HorizontalAlignment.Left);
            listViewAffectsVersions.Columns.Add(NAME_COLUMN, listViewAffectsVersions.Width - 30, HorizontalAlignment.Left);
            listViewFixForVersions.Columns.Add(NAME_COLUMN, listViewFixForVersions.Width - 30, HorizontalAlignment.Left);
            listViewComponents.Columns.Add(NAME_COLUMN, listViewComponents.Width - 30, HorizontalAlignment.Left);
            listViewResolutions.Columns.Add(NAME_COLUMN, listViewResolutions.Width - 30, HorizontalAlignment.Left);
            listViewIssueTypes.Columns.Add(NAME_COLUMN, listViewIssueTypes.Width - 30, HorizontalAlignment.Left);
            listViewPriorities.Columns.Add(NAME_COLUMN, listViewPriorities.Width - 30, HorizontalAlignment.Left);
            listViewStatuses.Columns.Add(NAME_COLUMN, listViewStatuses.Width - 30, HorizontalAlignment.Left);

            StartPosition = FormStartPosition.CenterParent;

            textBoxFilterName.Text = filter.Name;

            listViewProjects.SelectedIndexChanged += listViewProjects_SelectedValueChanged;
        }

        private void editCustomFilterShown(object sender, EventArgs e) {
            SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(server);
            SortedDictionary<int, JiraNamedEntity> statuses = JiraServerCache.Instance.getStatues(server);
            SortedDictionary<int, JiraNamedEntity> resolutions = JiraServerCache.Instance.getResolutions(server);
            List<JiraNamedEntity> priorities = JiraServerCache.Instance.getPriorities(server);

            refillProjects(projects);
            refillStatuses(statuses);
            refillResolutions(resolutions);
            refillPriorities(priorities);
            refillReporter();
            refillAssignee();

            refillIssueTypes(null);
            refillFixFor(null);
            refillComponents(null);
            refillAffectsVersions(null);

            manageSelections();

            listViewProjects.Focus();

            // a hack to make all list views actuall set their selection. ListView is a spawn of satan
            tabControl1.SelectTab(2);
            tabControl1.SelectTab(1);
            tabControl1.SelectTab(0);
        }

        private void manageSelections() {
            foreach (var project in filter.Projects) {
                foreach (var item in listViewProjects.Items.Cast<ListViewItem>().Where(item => project.Key.Equals(((JiraProjectListViewItem) item).Project.Key))) {
                    item.Selected = true;
                    item.EnsureVisible();
                    break;
                }
            }
            foreach (var priority in filter.Priorities) {
                foreach (var item in listViewPriorities.Items.Cast<ListViewItem>().Where(item => priority.Id == (((JiraNamedEntityListViewItem) item).Entity.Id))) {
                    item.Selected = true;
                    item.EnsureVisible();
                    break;
                }
            }
            foreach (var status in filter.Statuses) {
                foreach (ListViewItem item in listViewStatuses.Items.Cast<ListViewItem>().Where(item => status.Id == (((JiraNamedEntityListViewItem) item).Entity.Id))) {
                    item.Selected = true;
                    item.EnsureVisible();
                    break;
                }
            }
            foreach (var resolution in filter.Resolutions) {
                foreach (ListViewItem item in listViewResolutions.Items.Cast<ListViewItem>().Where(item => resolution.Id == (((JiraNamedEntityListViewItem) item).Entity.Id))) {
                    item.Selected = true;
                    item.EnsureVisible();
                    break;
                }
            }

            comboBoxReporter.SelectedItem = comboBoxReporter.Items[0];
            foreach (var item in comboBoxReporter.Items.OfType<UserTypeComboBoxItem>().Where(item => item.Type == filter.Reporter)) {
                comboBoxReporter.SelectedItem = item;
                break;
            }

            comboBoxAssignee.SelectedItem = comboBoxAssignee.Items[0];
            foreach (var item in comboBoxAssignee.Items.OfType<UserTypeComboBoxItem>().Where(item => item.Type == filter.Assignee)) {
                comboBoxAssignee.SelectedItem = item;
                break;
            }

            setProjectRelatedValues();
        }

        private void refillProjects(SortedDictionary<string, JiraProject> projects) {
            listViewProjects.Items.Clear();
            if (projects == null) return;
            foreach (string projectKey in projects.Keys) {
                listViewProjects.Items.Add(new JiraProjectListViewItem(projects[projectKey]));
            }
        }

        private void refillAssignee() {
            comboBoxAssignee.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.UNDEFINED));
            comboBoxAssignee.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.ANY));
            comboBoxAssignee.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.CURRENT));
            comboBoxAssignee.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.UNASSIGNED));
        }

        private void refillReporter() {
            comboBoxReporter.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.UNDEFINED));
            comboBoxReporter.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.ANY));
            comboBoxReporter.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.CURRENT));
        }

        private void refillPriorities(IEnumerable<JiraNamedEntity> priorities) {
            listViewPriorities.Items.Clear();

            if (priorities == null) return;

            ImageList imageList = new ImageList();

            int i = 0;

            foreach (JiraNamedEntity prio in priorities) {
                imageList.Images.Add(ImageCache.Instance.getImage(server, prio.IconUrl).Img);
                ListViewItem lvi = new JiraNamedEntityListViewItem(prio, i);
                listViewPriorities.Items.Add(lvi);
                ++i;
            }
            listViewPriorities.SmallImageList = imageList;
        }

        private void refillStatuses(SortedDictionary<int, JiraNamedEntity> statuses) {
            listViewStatuses.Items.Clear();

            if (statuses == null) return;

            ImageList imageList = new ImageList();

            int i = 0;

            foreach (int key in statuses.Keys) {
                imageList.Images.Add(ImageCache.Instance.getImage(server, statuses[key].IconUrl).Img);
                ListViewItem lvi = new JiraNamedEntityListViewItem(statuses[key], i);
                listViewStatuses.Items.Add(lvi);
                ++i;
            }
            listViewStatuses.SmallImageList = imageList;
        }

        private void refillResolutions(SortedDictionary<int, JiraNamedEntity> resolutions) {
            listViewResolutions.Items.Clear();
            if (resolutions == null) return;
            foreach (int key in resolutions.Keys) {
                listViewResolutions.Items.Add(new JiraNamedEntityListViewItem(resolutions[key], 0));
            }
        }

        private void refillIssueTypes(ICollection<JiraNamedEntity> issueTypes) {
            listViewIssueTypes.Items.Clear();

            ImageList imageList = new ImageList();

            int i = 0;

            if (issueTypes == null) {
                issueTypes = JiraServerCache.Instance.getIssueTypes(server).Values;
            }
            foreach (JiraNamedEntity issueType in issueTypes) {
                imageList.Images.Add(ImageCache.Instance.getImage(server, issueType.IconUrl).Img);
                ListViewItem lvi = new JiraNamedEntityListViewItem(issueType, i);
                listViewIssueTypes.Items.Add(lvi);
                ++i;
            }
            listViewIssueTypes.SmallImageList = imageList;
        }

        private void refillFixFor(IEnumerable<JiraNamedEntity> fixFors) {
            listViewFixForVersions.Items.Clear();

            if (fixFors == null)
                return;
            foreach (JiraNamedEntity fixFor in fixFors)
                listViewFixForVersions.Items.Add(new JiraNamedEntityListViewItem(fixFor, 0));
        }

        private void refillComponents(IEnumerable<JiraNamedEntity> comps) {
            listViewComponents.Items.Clear();

            if (comps == null)
                return;
            foreach (JiraNamedEntity component in comps)
                listViewComponents.Items.Add(new JiraNamedEntityListViewItem(component, 0));
        }

        private void refillAffectsVersions(IEnumerable<JiraNamedEntity> versions) {
            listViewAffectsVersions.Items.Clear();

            if (versions == null)
                return;
            foreach (JiraNamedEntity version in versions)
                listViewAffectsVersions.Items.Add(new JiraNamedEntityListViewItem(version, 0));
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void listViewProjects_SelectedValueChanged(object sender, EventArgs e) {
            setProjectRelatedValues();
        }

        private JiraProjectListViewItem lastSelected;

        private void setProjectRelatedValues() {
            if (listViewProjects.SelectedItems.Count == 1) {
                setAllEnabled(false);
                JiraProjectListViewItem project = listViewProjects.SelectedItems[0] as JiraProjectListViewItem;
                if (project != lastSelected) {
                    lastSelected = project;
                    Thread runner = PlvsUtils.createThread(() => setProjectRelatedValuesRunner(project != null ? project.Project : null));
                    runner.Start();
                }
            } else {
                lastSelected = null;
                refillIssueTypes(null);
                refillComponents(null);
                refillFixFor(null);
                refillAffectsVersions(null);

                setProjectRelatedSelections();
            }
        }

        private void setProjectRelatedValuesRunner(JiraProject project) {
            try {
                List<JiraNamedEntity> issueTypes = SmartJiraServerFacade.Instance.getIssueTypes(server, project);
                issueTypes.AddRange(SmartJiraServerFacade.Instance.getSubtaskIssueTypes(server, project));
                List<JiraNamedEntity> comps = SmartJiraServerFacade.Instance.getComponents(server, project);
                List<JiraNamedEntity> versions = SmartJiraServerFacade.Instance.getVersions(server, project);

                versions.Reverse();
                this.safeInvoke(new MethodInvoker(delegate {
                                             refillIssueTypes(issueTypes);
                                             refillComponents(comps);
                                             refillFixFor(versions);
                                             refillAffectsVersions(versions);

                                             setProjectRelatedSelections();

                                             setAllEnabled(true);
                                         }));
            }
            catch (Exception ex) {
                this.safeInvoke(new MethodInvoker(delegate { PlvsUtils.showError("Unable to retrieve project-related data", ex); }));
            }
        }

        private void setProjectRelatedSelections() {
            foreach (JiraNamedEntity issueType in filter.IssueTypes) {
                foreach (ListViewItem item in listViewIssueTypes.Items) {
                    if (issueType.Id != (((JiraNamedEntityListViewItem) item).Entity.Id)) continue;
                    item.Selected = true;
                    item.EnsureVisible();
                    break;
                }
            }

            foreach (JiraNamedEntity fixFor in filter.FixForVersions) {
                foreach (ListViewItem item in listViewFixForVersions.Items) {
                    if (fixFor.Id != (((JiraNamedEntityListViewItem)item).Entity.Id)) continue;
                    item.Selected = true;
                    item.EnsureVisible();
                    break;
                }
            }

            foreach (JiraNamedEntity comp in filter.Components) {
                foreach (ListViewItem item in listViewComponents.Items) {
                    if (comp.Id != (((JiraNamedEntityListViewItem)item).Entity.Id)) continue;
                    item.Selected = true;
                    item.EnsureVisible();
                    break;
                }
            }

            foreach (JiraNamedEntity affectVersion in filter.AffectsVersions) {
                foreach (ListViewItem item in listViewAffectsVersions.Items) {
                    if (affectVersion.Id != (((JiraNamedEntityListViewItem)item).Entity.Id)) continue;
                    item.Selected = true;
                    item.EnsureVisible();
                    break;
                }
            }

            listViewProjects.Focus();
        }

        private void setAllEnabled(bool enabled) {
            listViewProjects.Enabled = enabled;
            listViewIssueTypes.Enabled = enabled;
            listViewFixForVersions.Enabled = enabled;
            listViewComponents.Enabled = enabled;
            listViewAffectsVersions.Enabled = enabled;
            comboBoxReporter.Enabled = enabled;
            comboBoxAssignee.Enabled = enabled;
            listViewStatuses.Enabled = enabled;
            listViewResolutions.Enabled = enabled;
            listViewPriorities.Enabled = enabled;

            buttonClear.Enabled = enabled;
            textBoxFilterName.Enabled = enabled;
            buttonOk.Enabled = enabled && textBoxFilterName.Text.Length > 0;

            // crude as hell, but that's what people will mostly 
            // want to see - focused projects listview
            listViewProjects.Focus();
        }

        private void buttonClear_Click(object sender, EventArgs e) {
//            clearFilterValues();
            clearSelections();
//            Changed = true;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            clearFilterValues();
            
            filter.Name = textBoxFilterName.Text.Trim();

            foreach (var item in listViewProjects.SelectedItems) {
                JiraProjectListViewItem proj = item as JiraProjectListViewItem;
                if (proj != null)
                    filter.Projects.Add(proj.Project);
            }
            foreach (var item in listViewIssueTypes.SelectedItems) {
                JiraNamedEntityListViewItem itlvi = item as JiraNamedEntityListViewItem;
                if (itlvi != null)
                    filter.IssueTypes.Add(itlvi.Entity);
            }
            foreach (var item in listViewFixForVersions.SelectedItems) {
                JiraNamedEntityListViewItem version = item as JiraNamedEntityListViewItem;
                if (version != null)
                    filter.FixForVersions.Add(version.Entity);
            }
            foreach (var item in listViewAffectsVersions.SelectedItems) {
                JiraNamedEntityListViewItem version = item as JiraNamedEntityListViewItem;
                if (version != null)
                    filter.AffectsVersions.Add(version.Entity);
            }
            foreach (var item in listViewComponents.SelectedItems) {
                JiraNamedEntityListViewItem comp = item as JiraNamedEntityListViewItem;
                if (comp != null)
                    filter.Components.Add(comp.Entity);
            }
            foreach (var item in listViewPriorities.SelectedItems) {
                JiraNamedEntityListViewItem itlvi = item as JiraNamedEntityListViewItem;
                if (itlvi != null)
                    filter.Priorities.Add(itlvi.Entity);
            }
            foreach (var item in listViewStatuses.SelectedItems) {
                JiraNamedEntityListViewItem itlvi = item as JiraNamedEntityListViewItem;
                if (itlvi != null)
                    filter.Statuses.Add(itlvi.Entity);
            }
            foreach (var item in listViewResolutions.SelectedItems) {
                JiraNamedEntityListViewItem resolution = item as JiraNamedEntityListViewItem;
                if (resolution != null)
                    filter.Resolutions.Add(resolution.Entity);
            }
            var reporter = comboBoxReporter.SelectedItem;
            if (reporter != null) {
                UserTypeComboBoxItem item = reporter as UserTypeComboBoxItem;
                if (item != null) {
                    filter.Reporter = item.Type;
                }
            }
            else {
                filter.Reporter = JiraCustomFilter.UserType.UNDEFINED;
            }
            var assignee = comboBoxAssignee.SelectedItem;
            if (assignee != null) {
                UserTypeComboBoxItem item = assignee as UserTypeComboBoxItem;
                if (item != null) {
                    filter.Assignee = item.Type;
                }
            } else {
                filter.Assignee = JiraCustomFilter.UserType.UNDEFINED;
            }

            Changed = true;
            Close();
        }

        private void clearSelections() {
            listViewIssueTypes.SelectedItems.Clear();
            listViewFixForVersions.SelectedItems.Clear();
            listViewComponents.SelectedItems.Clear();
            listViewAffectsVersions.SelectedItems.Clear();
            listViewResolutions.SelectedItems.Clear();
            listViewStatuses.SelectedItems.Clear();
            listViewPriorities.SelectedItems.Clear();
            comboBoxReporter.SelectedItem = comboBoxReporter.Items[0];
            comboBoxAssignee.SelectedItem = comboBoxAssignee.Items[0];
            // make it last, so that project-related updates are not triggered too early
            listViewProjects.SelectedItems.Clear();
            listViewIssueTypes.SelectedItems.Clear();
        }

        private void clearFilterValues() {
            filter.Projects.Clear();
            filter.IssueTypes.Clear();
            filter.AffectsVersions.Clear();
            filter.FixForVersions.Clear();
            filter.Components.Clear();
            filter.Reporter = JiraCustomFilter.UserType.UNDEFINED;
            filter.Assignee = JiraCustomFilter.UserType.UNDEFINED;
            filter.Statuses.Clear();
            filter.Priorities.Clear();
            filter.Resolutions.Clear();
        }

        private void editCustomFilterKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape) {
                Close();
            }
        }

        private void textBoxFilterName_TextChanged(object sender, EventArgs e) {
            buttonOk.Enabled = textBoxFilterName.Text.Trim().Length > 0;
        }
    }
}
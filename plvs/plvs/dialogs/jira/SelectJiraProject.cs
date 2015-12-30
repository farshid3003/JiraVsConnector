using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.dialogs.jira {
    public partial class SelectJiraProject : Form {
        public SelectJiraProject(IEnumerable<JiraProject> projects, JiraProject selectedProject) {

            InitializeComponent();

            foreach (JiraProject project in projects) {
                listProjects.Items.Add(project);
            }
            if (selectedProject != null) {
                listProjects.SelectedItem = selectedProject;
            }

            buttonOk.Enabled = selectedProject != null;

            StartPosition = FormStartPosition.CenterParent;
        }

        private void listProjects_SelectedValueChanged(object sender, System.EventArgs e) {
            buttonOk.Enabled = listProjects.SelectedItem != null;
        }

        public JiraProject getSelectedProject() {
            if (DialogResult != DialogResult.OK) {
                return null;
            }
            return listProjects.SelectedItem as JiraProject;
        }
    }
}
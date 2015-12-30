using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira {
    internal class JiraProjectListViewItem : ListViewItem {
        public JiraProject Project { get; private set; }

        public JiraProjectListViewItem(JiraProject project) : base(project.ToString(), 0) {
            Project = project;
        }
    }
}
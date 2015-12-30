using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.issues.issuegroupnodes;

namespace Atlassian.plvs.ui.jira.issues.treemodels {
    internal class GroupedByProjectIssueTreeModel : AbstractGroupingIssueTreeModel {

        private readonly SortedDictionary<string, AbstractIssueGroupNode> groupNodes = 
            new SortedDictionary<string, AbstractIssueGroupNode>();

        public GroupedByProjectIssueTreeModel(JiraIssueListModel model, ToolStripButton groupSubtasksButton)
            : base(model, groupSubtasksButton) {
        }

        protected override AbstractIssueGroupNode findGroupNode(JiraIssue issue) {
            if (!groupNodes.ContainsKey(issue.ProjectKey)) {
                SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(issue.Server);
                if (!projects.ContainsKey(issue.ProjectKey)) {
                    return null;
                }
                JiraProject jiraProject = projects[issue.ProjectKey];
                groupNodes[issue.ProjectKey] = new ByProjectIssueGroupNode(jiraProject);
            }
            return groupNodes[issue.ProjectKey];
        }

        protected override IEnumerable<AbstractIssueGroupNode> getGroupNodes() {
            return groupNodes.Values;
        }

        protected override void clearGroupNodes() {
            groupNodes.Clear();
        }
    }
}
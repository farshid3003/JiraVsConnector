using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.issues.issuegroupnodes;

namespace Atlassian.plvs.ui.jira.issues.treemodels {
    internal class GroupedByTypeIssueTreeModel : AbstractGroupingIssueTreeModel {

        private readonly SortedDictionary<int, AbstractIssueGroupNode> groupNodes = 
            new SortedDictionary<int, AbstractIssueGroupNode>();

        public GroupedByTypeIssueTreeModel(JiraIssueListModel model, ToolStripButton groupSubtasksButton)
            : base(model, groupSubtasksButton) {
        }

        protected override AbstractIssueGroupNode findGroupNode(JiraIssue issue) {
            if (!groupNodes.ContainsKey(issue.IssueTypeId)) {
                SortedDictionary<int, JiraNamedEntity> issueTypes = JiraServerCache.Instance.getIssueTypes(issue.Server);
                if (!issueTypes.ContainsKey(issue.IssueTypeId)) {
                    return null;
                }
                JiraNamedEntity issueType = issueTypes[issue.IssueTypeId];
                groupNodes[issue.IssueTypeId] = new ByTypeIssueGroupNode(issue.Server, issueType);
            }
            return groupNodes[issue.IssueTypeId];
        }

        protected override IEnumerable<AbstractIssueGroupNode> getGroupNodes() {
            return groupNodes.Values;
        }

        protected override void clearGroupNodes() {
            groupNodes.Clear();
        }
    }
}
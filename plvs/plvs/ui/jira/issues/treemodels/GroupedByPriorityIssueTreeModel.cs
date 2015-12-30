using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.issues.issuegroupnodes;

namespace Atlassian.plvs.ui.jira.issues.treemodels {
    /// <summary>
    /// the whole reason why this class is so damn ugly and complicated is the fact
    /// that issue priorities can be reordered by the user - and the order matters a lot here.
    /// So we *must not* assume that priority IDs can define the sorting order
    /// </summary>

    internal class GroupedByPriorityIssueTreeModel : AbstractGroupingIssueTreeModel {

        private readonly SortedDictionary<int, AbstractIssueGroupNode> groupNodes = 
            new SortedDictionary<int, AbstractIssueGroupNode>();

        private readonly List<JiraIssue> issuesWithUnknownPriority = new List<JiraIssue>();

        private static readonly JiraNamedEntity UNKNOWN_PRIORITY = new JiraNamedEntity(-1, "No Priority", null);
        private readonly ByPriorityIssueGroupNode unknownPriorityNode = new ByPriorityIssueGroupNode(null, UNKNOWN_PRIORITY);

        public GroupedByPriorityIssueTreeModel(JiraIssueListModel model, ToolStripButton groupSubtasksButton)
            : base(model, groupSubtasksButton) {
        }

        protected override AbstractIssueGroupNode findGroupNode(JiraIssue issue) {
            JiraNamedEntity prio = UNKNOWN_PRIORITY;
            if (!groupNodes.ContainsKey(issue.PriorityId)) {
                List<JiraNamedEntity> priorities = JiraServerCache.Instance.getPriorities(issue.Server);
                foreach (JiraNamedEntity priority in priorities) {
                    if (priority.Id != issue.PriorityId) {
                        continue;
                    }
                    prio = priority;
                    break;
                }
                ByPriorityIssueGroupNode groupNode = 
                    prio.Id == UNKNOWN_PRIORITY.Id ? unknownPriorityNode : new ByPriorityIssueGroupNode(issue.Server, prio);
                groupNodes[prio.Id] = groupNode;
                if (prio.Id == UNKNOWN_PRIORITY.Id) {
                    issuesWithUnknownPriority.Add(issue);
                }
                return groupNode;
            }
            // all issues with unknown priority must be put in one bucket, once it is created
            foreach (var i in issuesWithUnknownPriority) {
                if (i.Id == issue.Id && i.Server.GUID.Equals(issue.Server.GUID)) {
                    return groupNodes[UNKNOWN_PRIORITY.Id];
                }
            }
            return groupNodes[issue.PriorityId];
        }

        protected override IEnumerable<AbstractIssueGroupNode> getGroupNodes() {
            return getSortedPriorityNodes();
        }

        private IEnumerable<AbstractIssueGroupNode> getSortedPriorityNodes() {
            IssueNode node = null;
            foreach (var groupNode in groupNodes) {
                node = groupNode.Value.IssueNodes[0];
            }
            if (node != null) {
                var sortedPrioIds = JiraServerCache.Instance.getPriorities(node.Issue.Server);

                List<AbstractIssueGroupNode> list = new List<AbstractIssueGroupNode>();

                foreach (var prio in sortedPrioIds) {
                    foreach (var prioGroup in groupNodes) {
                        if (prio.Id == prioGroup.Key) {
                            list.Add(prioGroup.Value);
                        }
                    }
                }
                // everthing else lands in the "unknown" priority bucket
                if (groupNodes.ContainsKey(UNKNOWN_PRIORITY.Id)) {
                    list.Add(groupNodes[UNKNOWN_PRIORITY.Id]);
                }
                return list;
            }
            return null;
        }

        protected override void clearGroupNodes() {
            groupNodes.Clear();
            issuesWithUnknownPriority.Clear();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.issues.issuegroupnodes;

namespace Atlassian.plvs.ui.jira.issues.treemodels {
    internal class GroupedByLastUpdatedIssueTreeModel : AbstractGroupingIssueTreeModel {

        private readonly List<AbstractIssueGroupNode> nodes = new List<AbstractIssueGroupNode>
                                                              {
                                                                  new ByLastUpdatedIssueGroupNode("Today"),
                                                                  new ByLastUpdatedIssueGroupNode("Yesterday"),
                                                                  new ByLastUpdatedIssueGroupNode("Last Week"),
                                                                  new ByLastUpdatedIssueGroupNode("Last Month"),
                                                                  new ByLastUpdatedIssueGroupNode("Older Than Last Month"),
                                                              };

        public GroupedByLastUpdatedIssueTreeModel(JiraIssueListModel model, ToolStripButton groupSubtasksButton)
            : base(model, groupSubtasksButton) {
        }

        protected override AbstractIssueGroupNode findGroupNode(JiraIssue issue) {
            DateTime time = issue.UpdateDate;
            if (time.Date.Equals(DateTime.Today.Date)) {
                return nodes[0];
            }
            if (time.Date.AddDays(1).Equals(DateTime.Today.Date)) {
                return nodes[1];
            }
            if (time.Date.AddDays(7) > DateTime.Today.Date) {
                return nodes[2];
            }
            if (time.Date.AddMonths(1) > DateTime.Today.Date) {
                return nodes[3];
            }
            return nodes[4];
        }

        protected override IEnumerable<AbstractIssueGroupNode> getGroupNodes() {
            List<AbstractIssueGroupNode> result = new List<AbstractIssueGroupNode>();
            foreach (var node in nodes) {
                if (node.IssueNodes.Count > 0) {
                    result.Add(node);
                }
            }
            return result;
        }

        protected override void clearGroupNodes() {
            foreach (var node in nodes) {
                node.IssueNodes.Clear();
            }
        }
    }
}
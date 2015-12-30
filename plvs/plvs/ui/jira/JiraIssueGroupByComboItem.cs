using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.attributes;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.issues.treemodels;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.jira {
    internal class JiraIssueGroupByComboItem {
        private readonly JiraIssueListModel model;
        private readonly ToolStripButton groupSubtasksButton;
        public GroupBy By { get; private set; }

        public enum GroupBy {
            [StringValue("None")] NONE,
            [StringValue("Project")] PROJECT,
            [StringValue("Type")] TYPE,
            [StringValue("Status")] STATUS,
            [StringValue("Priority")] PRIORITY,
            [StringValue("Last Updated")] LAST_UPDATED
        }

        private delegate AbstractIssueTreeModel CreateTreeModel(JiraIssueListModel model, ToolStripButton button);

        private static readonly SortedDictionary<GroupBy, CreateTreeModel> TREE_MODEL_CREATORS =
            new SortedDictionary<GroupBy, CreateTreeModel>
            {
                {GroupBy.NONE, (model, button) => new FlatIssueTreeModel(model, button)},
                {GroupBy.PROJECT, (model, button) => new GroupedByProjectIssueTreeModel(model, button)},
                {GroupBy.TYPE, (model, button) => new GroupedByTypeIssueTreeModel(model, button)},
                {GroupBy.STATUS, (model, button) => new GroupedByStatusIssueTreeModel(model, button)},
                {GroupBy.PRIORITY, (model, button) => new GroupedByPriorityIssueTreeModel(model, button)},
                {GroupBy.LAST_UPDATED, (model, button) => new GroupedByLastUpdatedIssueTreeModel(model, button)},
            };

        public JiraIssueGroupByComboItem(GroupBy groupBy, JiraIssueListModel model, ToolStripButton groupSubtasksButton) {
            this.model = model;
            this.groupSubtasksButton = groupSubtasksButton;
            By = groupBy;
        }

        public override string ToString() {
            return By.GetStringValue();
        }

        public AbstractIssueTreeModel TreeModel {
            get {
                return TREE_MODEL_CREATORS.ContainsKey(By) 
                    ? TREE_MODEL_CREATORS[By](model, groupSubtasksButton) 
                    : new FlatIssueTreeModel(model, groupSubtasksButton);
            }
        }
    }
}
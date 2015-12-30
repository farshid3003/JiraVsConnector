using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;

namespace Atlassian.plvs.ui.jira.issues.treemodels {
    public abstract class AbstractIssueTreeModel : ITreeModel {
        private readonly ToolStripButton groupSubtasksButton;

        protected bool GroupSubtasksUnderParent { get { return groupSubtasksButton.Checked; } }

        protected JiraIssueListModel Model { get; private set; }

        protected AbstractIssueTreeModel(JiraIssueListModel model, ToolStripButton groupSubtasksButton) {
            this.groupSubtasksButton = groupSubtasksButton;
            Model = model;
        }

        public void init() {
            fillModel(Model.Issues);
            Model.ModelChanged += modelModelChanged;
            Model.IssueChanged += modelIssueChanged;
            groupSubtasksButton.CheckedChanged += groupSubtasksButtonCheckedChanged;
        }

        protected void groupSubtasksButtonCheckedChanged(object sender, EventArgs e) {
            fillModel(Model.Issues);
        }

        protected abstract void modelIssueChanged(object sender, IssueChangedEventArgs e);

        protected abstract void modelModelChanged(object sender, EventArgs e);

        public void shutdown() {
            Model.ModelChanged -= modelModelChanged;
            Model.IssueChanged -= modelIssueChanged;
            groupSubtasksButton.CheckedChanged -= groupSubtasksButtonCheckedChanged;
        }

        protected abstract void fillModel(IEnumerable<JiraIssue> issues);

        #region ITreeModel Members

        public abstract IEnumerable GetChildren(TreePath treePath);
        public abstract bool IsLeaf(TreePath treePath);
        public abstract event EventHandler<TreeModelEventArgs> NodesChanged;
        public abstract event EventHandler<TreeModelEventArgs> NodesInserted;
        public abstract event EventHandler<TreeModelEventArgs> NodesRemoved;
        public abstract event EventHandler<TreePathEventArgs> StructureChanged;

        public abstract event EventHandler<EventArgs> TreeAboutToChange;

        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;

namespace Atlassian.plvs.ui.jira.issues.treemodels {
    internal class FlatIssueTreeModel : AbstractIssueTreeModel {
        private readonly List<IssueNode> nodes = new List<IssueNode>();

        public FlatIssueTreeModel(JiraIssueListModel model, ToolStripButton groupSubtasksButton) : base(model, groupSubtasksButton) {}

        protected override void fillModel(IEnumerable<JiraIssue> issues) {
            nodes.Clear();

            List<JiraIssue> subs = new List<JiraIssue>();
            List<JiraIssue> orphanSubs = new List<JiraIssue>();

            foreach (var issue in issues) {
                if (!(issue.IsSubtask && GroupSubtasksUnderParent)) {
                    nodes.Add(new IssueNode(issue));
                } else {
                    subs.Add(issue);
                }
            }

            foreach (JiraIssue sub in subs) {
                IssueNode parent = null;
                foreach (IssueNode n in nodes) {
                    if (!sub.ParentKey.Equals(n.Issue.Key)) {
                        continue;
                    }
                    parent = n;
                    break;
                }
                if (parent != null) {
                    parent.SubtaskNodes.Add(new IssueNode(sub));
                } else {
                    orphanSubs.Add(sub);
                }
            }

            // orphaned subtasks go at the end of the tree. 
            // Not really kosher, priority order is lost :(
            foreach (JiraIssue sub in orphanSubs) {
                nodes.Add(new IssueNode(sub));
            }

            if (TreeAboutToChange != null) {
                TreeAboutToChange(this, new EventArgs());
            }
            
            if (StructureChanged != null) {
                StructureChanged(this, new TreePathEventArgs(TreePath.Empty));
            }
        }

        public override IEnumerable GetChildren(TreePath treePath) {
            if (treePath.IsEmpty()) {
                return nodes;
            }
            IssueNode n = treePath.LastNode as IssueNode;
            return n != null ? n.SubtaskNodes : null;
        }

        public override bool IsLeaf(TreePath treePath) {
            if (GroupSubtasksUnderParent) {
                IssueNode n = treePath.LastNode as IssueNode;
                if (n != null) {
                    return n.Issue.IsSubtask || !n.Issue.HasSubtasks;
                }
            }
            return true;
        }

        protected override void modelModelChanged(object sender, EventArgs e) {
            fillModel(Model.Issues);
        }

        protected override void modelIssueChanged(object sender, IssueChangedEventArgs e) {
            foreach (var node in nodes) {
                if (GroupSubtasksUnderParent && e.Issue.IsSubtask) {
                    if (!node.Issue.Key.Equals(e.Issue.ParentKey)) continue;

                    foreach (IssueNode subNode in node.SubtaskNodes) {
                        if (subNode.Issue.Id != e.Issue.Id) continue;

                        subNode.Issue = e.Issue;
                        if (TreeAboutToChange != null) {
                            TreeAboutToChange(this, new EventArgs());
                        }
                        if (NodesChanged != null) {
                            NodesChanged(this, new TreeModelEventArgs(new TreePath(node), new object[] { subNode }));
                        }
                        return;
                    }
                }

                if (node.Issue.Id != e.Issue.Id) continue;

                node.Issue = e.Issue;

                if (TreeAboutToChange != null) {
                    TreeAboutToChange(this, new EventArgs());
                }
                if (NodesChanged != null) {
                    NodesChanged(this, new TreeModelEventArgs(TreePath.Empty, new object[] { node }));
                }

                return;
            }
        }

        public override event EventHandler<TreeModelEventArgs> NodesChanged;
        public override event EventHandler<TreePathEventArgs> StructureChanged;

        public override event EventHandler<EventArgs> TreeAboutToChange;

#pragma warning disable 67
        public override event EventHandler<TreeModelEventArgs> NodesInserted;
        public override event EventHandler<TreeModelEventArgs> NodesRemoved;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.issues.issuegroupnodes;

namespace Atlassian.plvs.ui.jira.issues.treemodels {
    internal abstract class AbstractGroupingIssueTreeModel : AbstractIssueTreeModel {

        protected AbstractGroupingIssueTreeModel(JiraIssueListModel model, ToolStripButton groupSubtasksButton)
            : base(model, groupSubtasksButton) {
        }

        protected override void fillModel(IEnumerable<JiraIssue> issues) {
            clearGroupNodes();

            List<JiraIssue> subs = new List<JiraIssue>();
            List<JiraIssue> orphanSubs = new List<JiraIssue>();

            foreach (var issue in issues) {
                if (!(issue.IsSubtask && GroupSubtasksUnderParent)) {
                    AbstractIssueGroupNode group = findGroupNode(issue);
                    if (group != null) {
                        group.IssueNodes.Add(new IssueNode(issue));
                    }
                } else {
                    subs.Add(issue);
                }
            }

            foreach (JiraIssue sub in subs) {
                IssueNode parent = findIssueNodeByKey(sub.ParentKey);
                if (parent != null) {
                    parent.SubtaskNodes.Add(new IssueNode(sub));
                } else {
                    orphanSubs.Add(sub);
                }
            }

            // orphaned subtasks go at the end of the tree. 
            // Not really kosher, priority order is lost :(
            foreach (JiraIssue sub in orphanSubs) {
                AbstractIssueGroupNode group = findGroupNode(sub);
                if (group != null) {
                    group.IssueNodes.Add(new IssueNode(sub));
                }
            }

            if (StructureChanged != null) {
                StructureChanged(this, new TreePathEventArgs(TreePath.Empty));
            }
        }

        private IssueNode findIssueNodeByKey(IEquatable<string> key) {
            foreach (AbstractIssueGroupNode n in getGroupNodes()) {
                foreach (IssueNode issueNode in n.IssueNodes) {
                    if (!key.Equals(issueNode.Issue.Key)) {
                        continue;
                    }
                    return issueNode;
                }
            }
            return null;
        }

        protected abstract void clearGroupNodes();

        protected abstract AbstractIssueGroupNode findGroupNode(JiraIssue issue);

        public override IEnumerable GetChildren(TreePath treePath) {
            if (treePath.IsEmpty()) {
                return getGroupNodes();
            }
            AbstractIssueGroupNode groupNode = treePath.LastNode as AbstractIssueGroupNode;
            if (groupNode != null) {
                return groupNode.IssueNodes;
            }
            IssueNode issueNode = treePath.LastNode as IssueNode;
            return issueNode != null ? issueNode.SubtaskNodes : null;
        }

        protected abstract IEnumerable<AbstractIssueGroupNode> getGroupNodes();

        public override bool IsLeaf(TreePath treePath) {
            if (!(treePath.LastNode is IssueNode)) {
                return false;
            }

            if (GroupSubtasksUnderParent) {
                IssueNode n = treePath.LastNode as IssueNode;
                if (n != null) {
                    return n.Issue.IsSubtask || !n.Issue.HasSubtasks;
                }
            }

            return true;
        }

        protected override void modelModelChanged(object sender, EventArgs e) {
            if (TreeAboutToChange != null) {
                TreeAboutToChange(this, new EventArgs());
            }

            fillModel(Model.Issues);
        }

        protected override void modelIssueChanged(object sender, IssueChangedEventArgs e) {
            foreach (var groupNode in getGroupNodes()) {

                if (GroupSubtasksUnderParent && e.Issue.IsSubtask) {
                    foreach (var issueNode in groupNode.IssueNodes) {
                        if (!issueNode.Issue.Key.Equals(e.Issue.ParentKey)) continue;
                        if (issueNode.SubtaskNodes == null) continue;
                        foreach (IssueNode subNode in issueNode.SubtaskNodes) {
                            if (subNode.Issue.Id != e.Issue.Id) continue;

                            subNode.Issue = e.Issue;

                            if (TreeAboutToChange != null) {
                                TreeAboutToChange(this, new EventArgs());
                            }

                            if (NodesChanged != null) {
                                NodesChanged(this, new TreeModelEventArgs(new TreePath(new object[] {groupNode, issueNode}), new object[] {subNode}));
                            }
                            return;
                        }
                    }
                }
                foreach (var issueNode in groupNode.IssueNodes) {
                    if (issueNode.Issue.Id != e.Issue.Id) continue;

                    if (TreeAboutToChange != null) {
                        TreeAboutToChange(this, new EventArgs());
                    }

                    if (findGroupNode(e.Issue) != groupNode) {
                        fillModel(Model.Issues);
                    } else if (NodesChanged != null) {
                        issueNode.Issue = e.Issue;
                        NodesChanged(this, new TreeModelEventArgs(new TreePath(groupNode), new object[] {issueNode}));
                    }
                    return;
                }
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
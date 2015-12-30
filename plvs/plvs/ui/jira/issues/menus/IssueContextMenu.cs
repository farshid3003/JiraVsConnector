using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.dialogs.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.util;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.ui.jira.issues.menus {
    public sealed class IssueContextMenu : ContextMenuStrip {
        private readonly JiraIssueListModel model;
        private readonly StatusLabel status;
        private readonly TreeViewAdv tree;
        private readonly ToolStripItem[] items;
        private JiraIssue issue;

        private int menuOpenGeneration;

        public IssueContextMenu(JiraIssueListModel model, StatusLabel status, TreeViewAdv tree,
                                ToolStripItem[] items) {
            this.model = model;
            this.status = status;
            this.tree = tree;
            this.items = items;

            Items.Add("dummy");

            Opened += issueContextMenuOpened;
            Opening += issueContextMenuOpening;
        }

        private void issueContextMenuOpening(object sender, CancelEventArgs e) {
            var selected = tree.SelectedNode;
            if (selected == null || !(selected.Tag is IssueNode)) {
                e.Cancel = true;
                return;
            }
            issue = ((IssueNode) selected.Tag).Issue;
        }

        private void issueContextMenuOpened(object sender, EventArgs e) {
            Items.Clear();
            Items.AddRange(items);

            Thread loaderThread = PlvsUtils.createThread(() => addIssueActionItems(++menuOpenGeneration));
            loaderThread.Start();
        }


        private void addIssueActionItems(int generation) {
            List<JiraNamedEntity> actions = null;
            if (!issue.IsSubtask && issue.Server.BuildNumber > 0) {
                this.safeInvoke(new MethodInvoker(delegate {
                    if (generation != menuOpenGeneration) return;
                    Items.Add(new ToolStripSeparator());
                    Items.Add(new ToolStripMenuItem("Add Subtask", Resources.add_jira, new EventHandler(
                        delegate {
                            CreateIssue.createDialogOrBringToFront(issue.Server, issue);
                        }
                    )));
                }));
            }
            try {
                actions = SmartJiraServerFacade.Instance.getActionsForIssue(issue);
            } catch (Exception e) {
                status.setError("Failed to retrieve issue actions", e);
            }
            if (actions == null || actions.Count == 0) return;

            this.safeInvoke(new MethodInvoker(delegate {
                                         // PLVS-39 - only update current menu, skip results of previous getActionsForIssue()
                                         // in case the user quickly opens context menu more than once
                                         if (generation != menuOpenGeneration) return;

                                         Items.Add(new ToolStripSeparator());
                                         foreach (var action in actions) {
                                             var actionCopy = action;
                                             ToolStripMenuItem item = new ToolStripMenuItem(
                                                 action.Name, null,
                                                 new EventHandler(
                                                     delegate {
                                                         IssueActionRunner.runAction(this, actionCopy, model, issue, status, null);
                                                     }));
                                             Items.Add(item);
                                         }
                                     }));
        }

    }
}
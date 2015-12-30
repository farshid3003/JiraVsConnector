using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.store;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.dialogs.jira {
    public class DeactivateIssue : LogWork {
        private readonly ParameterStore store;
        private readonly Action onFinished;
        private readonly CheckBox checkBoxLogWork;
        private CheckBox checkBoxRunAction;
        private ComboBox cbActions;
        private const string DEACTIVATE_ISSUE_LAST_ACTION = "deactivateIssueLastAction";
        private const string DEACTIVATE_ISSUE_RUN_ACTION_CHECKED = "deactivateIssueRunActionChecked";
        private const string DEACTIVATE_ISSUE_LOG_WORK_CHECKED = "deactivateIssueLogWorkChecked";

        public DeactivateIssue(
            string explanationText, Control parent, JiraIssueListModel model, AbstractJiraServerFacade facade, ParameterStore store,
            JiraIssue issue, StatusLabel status, JiraActiveIssueManager activeIssueManager, 
            IEnumerable<JiraNamedEntity> actions, Action onFinished) 
            : base(parent, model, facade, issue, status, activeIssueManager) {
            this.store = store;
            this.onFinished = onFinished;

            setOkButtonName("Stop Work");

            SuspendLayout();

            Controls.Remove(LogWorkPanel);

            int explanationY = 0;
            if (explanationText != null) {
                Label l = new Label
                          {
                              Text = explanationText,
                              Location = new Point(10, 10),
                              AutoSize = false,
                              Width = LogWorkPanel.Width,
                              Height = 40,
                              TextAlign = ContentAlignment.MiddleCenter
                          };
                Controls.Add(l);
                explanationY = 50;
            }
            checkBoxLogWork = new CheckBox { AutoSize = true, Text = "Log Work", Location = new Point(10, 10 + explanationY) };
            checkBoxLogWork.CheckedChanged += (s, e) => {
                                                  LogWorkPanel.Enabled = checkBoxLogWork.Checked;
                                                  updateOkButtonState();
                                              };
            GroupBox group = new GroupBox {
                                 Size = new Size(LogWorkPanel.Width + 20, LogWorkPanel.Height + 20),
                                 Location = new Point(10, checkBoxLogWork.Location.Y + checkBoxLogWork.Height - 3)
                             };
            group.Controls.Add(LogWorkPanel);

            LogWorkPanel.Location = new Point(5, 15);
            LogWorkPanel.Enabled = checkBoxLogWork.Checked;

            checkBoxRunAction = new CheckBox {
                                                 AutoSize = true,
                                                 Text = "Run Issue Action",
                                                 Location = new Point(10, group.Location.Y + group.Height + 20)
                                             };
            cbActions = new ComboBox {
                                         DropDownStyle = ComboBoxStyle.DropDownList,
                                         Location = new Point(150, group.Location.Y + group.Height + 17)
                                     };
            checkBoxRunAction.CheckedChanged += (s, e) => {
                                                    cbActions.Enabled = checkBoxRunAction.Checked;
                                                    updateOkButtonState();
                                                };

            foreach (var action in actions) {
                cbActions.Items.Add(action);
            }
            cbActions.SelectedValueChanged += (s, e) => updateOkButtonState();
            cbActions.Enabled = checkBoxRunAction.Checked;

            Size = new Size(Size.Width + 40, Size.Height + 100 + explanationY);
            
            Controls.Add(checkBoxLogWork);
            Controls.Add(group);
            Controls.Add(checkBoxRunAction);
            Controls.Add(cbActions);

            ButtonOk.Location = new Point(ButtonOk.Location.X + 40, ButtonOk.Location.Y + 100 + explanationY);
            ButtonCancel.Location = new Point(ButtonCancel.Location.X + 40, ButtonCancel.Location.Y + 100 + explanationY);
            ResumeLayout(true);

            checkBoxLogWork.Checked = store.loadParameter(DEACTIVATE_ISSUE_LOG_WORK_CHECKED, 0) > 0;
            checkBoxRunAction.Checked = store.loadParameter(DEACTIVATE_ISSUE_RUN_ACTION_CHECKED, 0) > 0;
            int actionId = store.loadParameter(DEACTIVATE_ISSUE_LAST_ACTION, 0);
            foreach (var item in
                from object item in cbActions.Items
                let i = item as JiraNamedEntity
                where i != null && i.Id == actionId
                select item) {
                
                cbActions.SelectedItem = item;
                break;
            }
        }

        protected override void onOk(Action finished, bool closeDialogOnFinish) {
            store.storeParameter(DEACTIVATE_ISSUE_LOG_WORK_CHECKED, checkBoxLogWork.Checked ? 1 : 0);
            store.storeParameter(DEACTIVATE_ISSUE_RUN_ACTION_CHECKED, checkBoxRunAction.Checked ? 1 : 0);
            if (checkBoxLogWork.Checked && !checkBoxRunAction.Checked) {
                base.onOk(onFinished, true);
            } else if (checkBoxLogWork.Checked && checkBoxRunAction.Checked) {
                base.onOk(() => {
                              checkBoxLogWork.Checked = false; 
                              ButtonCancel.Enabled = true;
                              ButtonOk.Enabled = true;
                              runDeactivateIssueAction();
                          }, false);
            } else if (checkBoxRunAction.Checked) {
                runDeactivateIssueAction();
            } else {
                Close();
                onFinished();
            }
        }

        private void runDeactivateIssueAction() {
            JiraNamedEntity action = cbActions.SelectedItem as JiraNamedEntity;
            if (action == null) return;
            store.storeParameter(DEACTIVATE_ISSUE_LAST_ACTION, action.Id);
            IssueActionRunner.runAction(this, action, model, issue, status, () => { Close(); onFinished(); });
        }

        protected override void updateOkButtonState() {
            if (checkBoxLogWork.Checked) {
                base.updateOkButtonState();
                if (checkBoxRunAction.Checked && ButtonOk.Enabled) {
                    ButtonOk.Enabled = cbActions.SelectedItem != null;
                }
            } else {
                ButtonOk.Enabled = !checkBoxRunAction.Checked || cbActions.SelectedItem != null;
            }
        }

        protected override string getDialogName() {
            return "Stop Work on Issue " + issue.Key;
        }
    }
}

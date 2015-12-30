using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.util;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.dialogs.jira {
    public partial class LogWork : Form {
        private readonly Control parent;
        protected readonly JiraIssueListModel model;
        protected readonly AbstractJiraServerFacade facade;
        protected readonly JiraIssue issue;
        protected readonly StatusLabel status;
        private readonly JiraActiveIssueManager activeIssueManager;

        private DateTime endTime;

        protected Panel LogWorkPanel { get { return logWorkPanel; } }
        protected Button ButtonOk { get { return buttonOk; } }
        protected Button ButtonCancel { get { return buttonCancel; } }

        public LogWork(
            Control parent, JiraIssueListModel model, AbstractJiraServerFacade facade, JiraIssue issue, 
            StatusLabel status, JiraActiveIssueManager activeIssueManager) {

            this.parent = parent;
            this.model = model;
            this.facade = facade;
            this.issue = issue;
            this.status = status;
            this.activeIssueManager = activeIssueManager;
            InitializeComponent();

            endTime = DateTime.Now;

            setEndTimeLabelText();

            textRemainingEstimate.Enabled = false;
            radioAutoUpdate.Checked = true;
            textExplanation.Font = new Font(textExplanation.Font.FontFamily, textExplanation.Font.Size - 1);

            StartPosition = FormStartPosition.CenterParent;
        }

        protected void setOkButtonName(string name) {
            buttonOk.Text = name;    
        }

        private void setEndTimeLabelText() {
            labelEndTime.Text = endTime.ToShortDateString() + " " + endTime.ToShortTimeString();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void buttonChange_Click(object sender, EventArgs e) {
            SetEndTime dlg = new SetEndTime(endTime);
            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK) {
                return;
            }
            endTime = dlg.DateTime;
            setEndTimeLabelText();
        }

        private void radioUpdateManually_CheckedChanged(object sender, EventArgs e) {
            textRemainingEstimate.Enabled = radioUpdateManually.Checked;
            updateOkButtonState();
        }

        private void textTimeSpent_TextChanged(object sender, EventArgs e) {
            updateOkButtonState();
        }

        private void textRemainingEstimate_TextChanged(object sender, EventArgs e) {
            updateOkButtonState();
        }

        protected virtual void updateOkButtonState() {
            bool timeSpentOk;
            Regex regex = new Regex(Constants.TIME_TRACKING_REGEX);
            if (textTimeSpent.Text.Length > 0 && regex.IsMatch(textTimeSpent.Text)) {
                textTimeSpent.ForeColor = Color.Black;
                timeSpentOk = true;
            } else {
                textTimeSpent.ForeColor = Color.Red;
                timeSpentOk = false;
            }

            bool remainingOk;

            if (!radioUpdateManually.Checked
                || (textRemainingEstimate.Text.Length > 0 && regex.IsMatch(textRemainingEstimate.Text))) {
                textRemainingEstimate.ForeColor = Color.Black;
                remainingOk = true;
            } else {
                textRemainingEstimate.ForeColor = Color.Red;
                remainingOk = false;
            }

            buttonOk.Enabled = timeSpentOk && remainingOk;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            onOk(null, true);
        }

        protected virtual void onOk(Action finished, bool closeDialogOnFinish) {
            Action a;

            if (radioAutoUpdate.Checked) {
                a = logWorkAndAutoUpdateRemaining;
            } else if (radioLeaveUnchanged.Checked) {
                a = logWorkAndLeaveRemainingUnchanged;
            } else {
                a = logWorkAndUpdateRemainingManually;
            }

            logWorkPanel.Enabled = false;
            buttonOk.Enabled = false;
            buttonCancel.Enabled = false;
            Thread t = PlvsUtils.createThread(() => logWorkWorker(a, finished, closeDialogOnFinish));
            t.Start();
        }

        private void logWorkAndAutoUpdateRemaining() {
            facade.logWorkAndAutoUpdateRemaining(
                issue, JiraIssueUtils.addSpacesToTimeSpec(textTimeSpent.Text.Trim()), getStartTime(), textComment.Text.Trim());
        }

        private void logWorkAndLeaveRemainingUnchanged() {
            facade.logWorkAndLeaveRemainingUnchanged(
                issue, JiraIssueUtils.addSpacesToTimeSpec(textTimeSpent.Text.Trim()), getStartTime(), textComment.Text.Trim());
        }

        private void logWorkAndUpdateRemainingManually() {
            facade.logWorkAndUpdateRemainingManually(
                issue, JiraIssueUtils.addSpacesToTimeSpec(textTimeSpent.Text.Trim()), getStartTime(), 
                JiraIssueUtils.addSpacesToTimeSpec(textRemainingEstimate.Text.Trim()), textComment.Text.Trim());
        }

        private void logWorkWorker(Action action, Action finished, bool closeDialogOnFinish) {
            try {
                status.setInfo("Logging work for issue " + issue.Key + "...");
                action();
                status.setInfo("Logged work for issue " + issue.Key);
                UsageCollector.Instance.bumpJiraIssuesOpen();
                JiraIssue updatedIssue = facade.getIssue(issue.Server, issue.Key);
                parent.safeInvoke(new MethodInvoker(() => {
                                                        model.updateIssue(updatedIssue);
                                                        if (activeIssueManager.isActive(issue)) {
                                                            activeIssueManager.resetTimeSpent();
                                                        }
                                                    }));
            } catch (Exception e) {
                status.setError("Failed to log work for issue " + issue.Key, e);
            }
            parent.safeInvoke(new MethodInvoker(() => {
                                                    if (closeDialogOnFinish) Close();
                                                    if (finished != null) finished();
                                                }));
        }

        private DateTime getStartTime() {

            DateTime result = endTime;
#if false
            Regex regex = new Regex(Constants.TIME_TRACKING_REGEX);
            Match match = regex.Match(textTimeSpent.Text);
            Group @groupWeeks = match.Groups[2];
            Group @groupDays = match.Groups[4];
            Group @groupHours = match.Groups[6];
            Group @groupMinutes = match.Groups[8];

            if (groupWeeks != null && groupWeeks.Success) {
                result = result.AddDays(-7*double.Parse(groupWeeks.Value));
            }
            if (groupDays != null && groupDays.Success) {
                result = result.AddDays(-1*double.Parse(groupDays.Value));
            }
            if (groupHours != null && groupHours.Success) {
                result = result.AddHours(-1*double.Parse(groupHours.Value));
            }
            if (groupMinutes != null && groupMinutes.Success) {
                result = result.AddMinutes(-1*double.Parse(groupMinutes.Value));
            }
#endif
            return result;
        }

        private void logWorkKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Escape && buttonCancel.Enabled) {
                Close();
            }
        }

        private void logWorkLoad(object sender, EventArgs e) {
            Text = getDialogName();
            updateOkButtonState();

            if (!activeIssueManager.isActive(issue) || activeIssueManager.MinutesInProgress <= 0) return;

            int hours = activeIssueManager.MinutesInProgress / 60;
            textTimeSpent.Text = (hours > 0 ? hours + "h " : "") + activeIssueManager.MinutesInProgress % 60 + "m";
        }

        protected virtual string getDialogName() {
            return "Log Work for Issue " + issue.Key;
        }
    }
}
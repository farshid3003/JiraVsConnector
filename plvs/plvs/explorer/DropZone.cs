using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.util;

namespace Atlassian.plvs.explorer {
    public sealed partial class DropZone : Form {
        private readonly JiraIssueListModel model;
        private readonly JiraServer server;
        private readonly AbstractJiraServerFacade facade;
        private readonly DropZoneWorker worker;

        public delegate void PerformAction(JiraIssue issue, bool add);

        private static readonly Dictionary<string, DropZone> activeDropZones = new Dictionary<string, DropZone>();

        public interface DropZoneWorker {
            PerformAction Action { get; }
            string ZoneName { get; }
            string ZoneKey { get; }
            bool CanAdd { get; }
            string IssueWillBeAddedText { get; }
            string issueWillBeMovedText { get; }
            string InitialText { get; }
        }

        public static void showDropZoneFor(JiraIssueListModel model, JiraServer server, AbstractJiraServerFacade facade, DropZoneWorker worker) {
            if (activeDropZones.ContainsKey(worker.ZoneKey)) {
                activeDropZones[worker.ZoneKey].BringToFront();
            } else {
                new DropZone(model, server, facade, worker).Show();
            }
        }

        private DropZone(JiraIssueListModel model, JiraServer server, AbstractJiraServerFacade facade, DropZoneWorker worker) {
            this.model = model;
            this.server = server;
            this.facade = facade;
            this.worker = worker;

            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;

            Text = worker.ZoneName;

            updateModelAndResetToInitialState(null, false);
        }

        public static void closeAll() {
            List<DropZone> l = new List<DropZone>(activeDropZones.Values);
            foreach (DropZone zone in l) {
                zone.Close();
            }    
        }

        private static readonly Regex DROP_REGEX = new Regex(@"ISSUE:(\w+-\d+):SERVER:{(\S+)}");

        private void DropZone_DragEnter(object sender, DragEventArgs e) {
            if (!AllowDrop) return;

            e.Effect = DragDropEffects.None;

            labelInfo.Text = "Data format not accepted";

            if (!e.Data.GetDataPresent(DataFormats.UnicodeText))
                return;

            string txt = (string) e.Data.GetData(DataFormats.UnicodeText);

            if (!DROP_REGEX.IsMatch(txt)) return;

            string labelTxt = null;

            // "add to" is chosen with control key pressed
            if ((e.KeyState & 8) == 8 && (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) {
                e.Effect = worker.CanAdd ? DragDropEffects.Copy : DragDropEffects.None;
                labelTxt = worker.IssueWillBeAddedText;
            } else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) {
                e.Effect = DragDropEffects.Move;
                labelTxt = worker.issueWillBeMovedText;
            }

            if (labelTxt != null) {
                labelInfo.Text = labelTxt;
            }
        }

        private void DropZone_DragLeave(object sender, EventArgs e) {
            if (AllowDrop) {
                updateModelAndResetToInitialState(null, false);
            }
        }

        private void DropZone_DragDrop(object sender, DragEventArgs e) {

            if (!e.Data.GetDataPresent(DataFormats.UnicodeText)) return;

            string txt = (string)e.Data.GetData(DataFormats.UnicodeText);

            Match m = DROP_REGEX.Match(txt);
            if (m == null) return;

            Group @key = m.Groups[1];
            Group @guid = m.Groups[2];

            if (key == null || guid == null) return;

            labelInfo.Text = "Updating issue " + key.Value + "...";

            AllowDrop = false;

            bool add = (e.KeyState & 8) == 8 && (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy;

            Thread t = PlvsUtils.createThread(() => dropActionWorker(key.Value, guid.Value, add));
            t.Start();
        }

        private void dropActionWorker(string issueKey, string serverGuid, bool add) {
            try {
                if (!server.GUID.ToString().Equals(serverGuid)) {
                    throw new Exception("Only issues form server " + server.Name + "are accepted");
                }
                JiraIssue issue = facade.getIssue(server, issueKey);
                if (issue == null) {
                    throw new Exception("Issue " + issueKey + " not found on specified server");
                }
                worker.Action(issue, add);
                issue = facade.getIssue(server, issueKey);

                Invoke(new MethodInvoker(() => updateModelAndResetToInitialState(issue, add)));
            } catch (Exception e) {
                Invoke(new MethodInvoker(() => showErrorAndReset(e)));
            }
        }

        private void showErrorAndReset(Exception e) {
            PlvsUtils.showError("Failed to perform drop action", e);
            updateModelAndResetToInitialState(null, false);
        }

        private void updateModelAndResetToInitialState(JiraIssue issue, bool add) {
            if (issue != null) {
                model.updateIssue(issue);
                textHistory.Text = 
                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() 
                    + " - " + issue.Key + (worker.CanAdd ? (add ? " [added]" : " [moved]") : "") + "\r\n" + textHistory.Text;
            }
            AllowDrop = true;
            labelInfo.Text = worker.InitialText;
        }

        private void DropZone_Load(object sender, EventArgs e) {
            activeDropZones[worker.ZoneKey] = this;
        }

        private void DropZone_FormClosed(object sender, FormClosedEventArgs e) {
            activeDropZones.Remove(worker.ZoneKey);
        }
    }
}

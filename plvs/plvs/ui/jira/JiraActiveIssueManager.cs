using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs.jira;
using Atlassian.plvs.models;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.store;
using Atlassian.plvs.util;
using Atlassian.plvs.windows;
using Timer = System.Windows.Forms.Timer;

namespace Atlassian.plvs.ui.jira {
    public class JiraActiveIssueManager {
        private readonly ToolStrip container;
        private readonly StatusLabel jiraStatus;
        private readonly ToolStripButton buttonComment;
        private readonly ToolStripButton buttonLogWork;
        private readonly ToolStripButton buttonPause;
        private readonly ToolStripButton buttonStop;
        private readonly ToolStripSplitButton activeIssueDropDown;
        private readonly ToolStripSeparator separator;
        private readonly ToolStripLabel labelMinuteTimer;

        private const string ACTIVE_ISSUE_SERVER_GUID = "activeIssueServerGuid";
        private const string ACTIVE_ISSUE_KEY = "activeIssueKey";
        private const string ACTIVE_ISSUE_TIMER_VALUE = "activeIssueTimerValue";
        private const string ACTIVE_ISSUE_IS_PAUSED = "activeIssueIsPaused";
        private const string PAST_ACTIVE_ISSUE_COUNT = "activeIssuePastIssueCount";
        private const string PAST_ACTIVE_ISSUE_SERVER_GUID = "activeIssuePastServerGuid_";
        private const string PAST_ACTIVE_ISSUE_KEY = "activeIssuePastIssueKey_";

        public event EventHandler<EventArgs> ActiveIssueChanged;
        public event EventHandler<EventArgs> ToolbarWidthChanged;

        public class ActiveIssue {
            public ActiveIssue(string key, string serverGuid) {
                Key = key;
                ServerGuid = serverGuid;
                Enabled = true;
            }

            public string Key { get; private set; }
            public string ServerGuid { get; private set; }
            public string Summary { get; set; }
            public Image Icon { get; set; }
            public bool Enabled { get; set; }

            public bool Equals(ActiveIssue other) {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Key, Key) && Equals(other.ServerGuid, ServerGuid);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == typeof (ActiveIssue) && Equals((ActiveIssue) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((Key != null ? Key.GetHashCode() : 0)*397) ^ (ServerGuid != null ? ServerGuid.GetHashCode() : 0);
                }
            }
        }

        public ActiveIssue CurrentActiveIssue { get; private set; }
        private readonly LinkedList<ActiveIssue> pastActiveIssues = new LinkedList<ActiveIssue>();

        private const int ACTIVE_ISSUE_LIST_SIZE = 10;

        private bool paused;
        public int MinutesInProgress { get; private set; }

        public const string NO_ISSUE_ACTIVE = "No Issue Active";
        private const string STOP_WORK = "Stop Work on Active Issue";
        private const string PAUSE_WORK = "Pause Work on Active Issue";
        private const string RESUME_WORK = "Resume Work on Active Issue";
        private const string LOG_WORK = "Log Work on Active Issue";
        private const string COMMENT = "Comment on Active Issue";
        private const string FAILED_TO_START_WORK = "Failed to start work on issue";

        private const int START_PROGRESS_ACTION_ID = 4;

        private readonly Timer minuteTimer;

        public JiraActiveIssueManager(ToolStrip container, StatusLabel jiraStatus) {
            this.container = container;
            this.jiraStatus = jiraStatus;
            activeIssueDropDown = new ToolStripSplitButton();
            labelMinuteTimer = new ToolStripLabel();
            buttonStop = new ToolStripButton(Resources.ico_inactiveissue) {Text = STOP_WORK, DisplayStyle = ToolStripItemDisplayStyle.Image};
            buttonStop.Click += (s, e) => deactivateActiveIssue(true, null, null);
            buttonPause = new ToolStripButton(Resources.ico_pauseissue) { Text = PAUSE_WORK, DisplayStyle = ToolStripItemDisplayStyle.Image };
            buttonPause.Click += (s, e) => {
                                     paused = !paused;
                                     setTimeSpentString();
                                     savePausedState();
                                     buttonPause.Text = paused ? RESUME_WORK : PAUSE_WORK;
                                     buttonPause.Image = paused ? Resources.ico_activateissue : Resources.ico_pauseissue;
                                     if (ToolbarWidthChanged != null) {
                                         ToolbarWidthChanged(this, null);
                                     }
                                 };
            buttonLogWork = new ToolStripButton(Resources.ico_logworkonissue) { Text = LOG_WORK, DisplayStyle = ToolStripItemDisplayStyle.Image };
            buttonLogWork.Click += (s, e) => 
                loadIssueAndRunAction((server, issue) => 
                    container.safeInvoke(new MethodInvoker(() =>
                        new LogWork(container, JiraIssueListModelImpl.Instance, SmartJiraServerFacade.Instance, issue, jiraStatus, this).ShowDialog())), 
                    CurrentActiveIssue);
            buttonComment = new ToolStripButton(Resources.new_comment) { Text = COMMENT, DisplayStyle = ToolStripItemDisplayStyle.Image };
            buttonComment.Click += (s, e) => 
                loadIssueAndRunAction((server, issue) => 
                    container.safeInvoke(new MethodInvoker(() => addComment(issue))), 
                    CurrentActiveIssue);

            separator = new ToolStripSeparator();

            container.Items.Add(activeIssueDropDown);
            container.Items.Add(buttonStop);
            container.Items.Add(buttonPause);
            container.Items.Add(buttonLogWork);
            container.Items.Add(buttonComment);
            container.Items.Add(labelMinuteTimer);
            container.Items.Add(separator);

            activeIssueDropDown.ButtonClick += (s, e) => {
                                                   if (CurrentActiveIssue == null) return;
                                                   JiraServer server = JiraServerModel.Instance.getServer(new Guid(CurrentActiveIssue.ServerGuid));
                                                   if (server == null) return;
                                                   AtlassianPanel.Instance.Jira.findAndOpenIssue(CurrentActiveIssue.Key, server, findFinished);
                                               };

            activeIssueDropDown.ToolTipText = "Active Issue";
            setEnabled(false);

            JiraIssueListModelImpl.Instance.IssueChanged += issueChanged;
            minuteTimer = new Timer {Interval = ONE_MINUTE};
            minuteTimer.Tick += (s, e) => updateMinutes();
            minuteTimer.Start();
        }

        public int ToolbarWidth {
            get {
                if (!separator.Visible) {
                    return 0;
                }
                return activeIssueDropDown.Width + (CurrentActiveIssue != null ? buttonStop.Width + buttonPause.Width + buttonLogWork.Width +
                    buttonComment.Width + labelMinuteTimer.Width : 0) + separator.Width;
            }
        }

        private void addComment(JiraIssue issue) {
            SmartJiraServerFacade facade = SmartJiraServerFacade.Instance;
            NewIssueComment dlg = new NewIssueComment(issue, facade);
            dlg.ShowDialog();
            if (dlg.DialogResult != DialogResult.OK) return;

            Thread addCommentThread =
                PlvsUtils.createThread(delegate {
                                           try {
                                               jiraStatus.setInfo("Adding comment to issue...");
                                               facade.addComment(issue, dlg.CommentBody);
                                               issue = facade.getIssue(issue.Server, issue.Key);
                                               jiraStatus.setInfo("Comment added");
                                               UsageCollector.Instance.bumpJiraIssuesOpen();
                                               container.safeInvoke(new MethodInvoker(() => JiraIssueListModelImpl.Instance.updateIssue(issue)));
                                           } catch (Exception ex) {
                                               jiraStatus.setError("Adding comment failed", ex);
                                           }
                                       });
            addCommentThread.Start();
        }

        private static void findFinished(bool success, string message, Exception e) {
            if (!success) {
                PlvsUtils.showError(message, e);
            }
        }

        private void issueChanged(object sender, IssueChangedEventArgs e) {
            if (CurrentActiveIssue == null) return;
            if (!e.Issue.Key.Equals(CurrentActiveIssue.Key) ||
                !e.Issue.Server.GUID.ToString().Equals(CurrentActiveIssue.ServerGuid)) return;
            ++generation;
            loadActiveIssueDetails();
        }

        private void savePausedState() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            store.storeParameter(ACTIVE_ISSUE_IS_PAUSED, paused ? 1 : 0);
        }

        private void setTimeSpentString() {
            int hours = MinutesInProgress / 60;
            labelMinuteTimer.Text = "Time spent: " + (hours > 0 ? hours + "h " : "") + MinutesInProgress % 60 + "m";
            if (paused) {
                labelMinuteTimer.Text = labelMinuteTimer.Text + " (paused)";
            }
        }

        private void updateMinutes() {
            if (CurrentActiveIssue == null || paused) return;

            ++MinutesInProgress;
            storeTimeSpent();
            setTimeSpentString();
            if (ToolbarWidthChanged != null) {
                ToolbarWidthChanged(this, null);
            }
        }

        private void storeTimeSpent() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            store.storeParameter(ACTIVE_ISSUE_TIMER_VALUE, MinutesInProgress);
        }

        private void setEnabled(bool enabled) {
            foreach (ToolStripItem c in new ToolStripItem[]
                                        {
                                            activeIssueDropDown, 
                                            labelMinuteTimer,
                                            buttonStop, 
                                            buttonPause, 
                                            buttonLogWork, 
                                            buttonComment, 
                                            separator
                                        }) {
                c.Enabled = enabled;
                c.Visible = enabled;
            }
        }

        private static int generation;
        private const string EXPLANATION_TEXT = "Work on issue {0} has to be stopped before you can start work issue {1}";

        private const int ONE_MINUTE = 60000;
        private const int MAX_SUMMARY_LENGTH = 20;

        public void init() {
            ++generation;
            setTimeSpentString();
            CurrentActiveIssue = null;
            setEnabled(false);
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            string activeIssueKey = store.loadParameter(ACTIVE_ISSUE_KEY, null);
            string activeIssueServerGuidStr = store.loadParameter(ACTIVE_ISSUE_SERVER_GUID, null);
            if (activeIssueKey != null && activeIssueServerGuidStr != null) {
                MinutesInProgress = store.loadParameter(ACTIVE_ISSUE_TIMER_VALUE, 0);
                paused = store.loadParameter(ACTIVE_ISSUE_IS_PAUSED, 0) > 0;
                if (paused) {
                    buttonPause.Text = RESUME_WORK;
                    buttonPause.Image = Resources.ico_activateissue;
                }
                ICollection<JiraServer> jiraServers = JiraServerModel.Instance.getAllEnabledServers();
                if (jiraServers.Where(server => server.GUID.ToString().Equals(activeIssueServerGuidStr)).Any()) {
                    CurrentActiveIssue = new ActiveIssue(activeIssueKey, activeIssueServerGuidStr);
                    if (ActiveIssueChanged != null) {
                        ActiveIssueChanged(this, null);
                    }
                }
            }
            loadPastActiveIssues(store);
            setDropDownTextFromCurrentActiveIssue();

            Thread t = PlvsUtils.createThread(() => loadIssueInfosWorker(generation));
            t.Start();
        }

        private void setDropDownTextFromCurrentActiveIssue() {
            if (CurrentActiveIssue != null && CurrentActiveIssue.Enabled) {
                setEnabled(true);
                activeIssueDropDown.Text = CurrentActiveIssue.Key;
            } else {
                setNoIssueActiveInDropDown();
            }
            activeIssueDropDown.Image = null;
        }

        private delegate void OnIssueLoaded(JiraServer server, JiraIssue issue);

        private void loadIssueInfosWorker(int gen) {
            if (CurrentActiveIssue != null) {
                loadActiveIssueDetailsWorker(gen);
            }
        }

        private void loadActiveIssueDetailsWorker(int gen) {
            loadIssueAndRunAction((server, issue) => container.safeInvoke(new MethodInvoker(delegate {
                                                                                                if (gen != generation) return;
                                                                                                if (CurrentActiveIssue != null) {
                                                                                                    CurrentActiveIssue.Summary = issue.Summary;
                                                                                                }
                                                                                                setActiveIssueDropdownTextAndImage(server, issue);
                                                                                            })), CurrentActiveIssue);
        }

        private static void loadIssueAndRunAction(OnIssueLoaded loaded, ActiveIssue issue) {
            if (issue == null) return;
            var server = JiraServerModel.Instance.getServer(new Guid(issue.ServerGuid));
            if (server == null) return;

            try {
                var jiraIssue = SmartJiraServerFacade.Instance.getIssue(server, issue.Key);
                if (jiraIssue != null) {
                    loaded(server, jiraIssue);
                }
            } catch (Exception e) {
                Debug.WriteLine("JiraActiveIssueManager.loadIssuAndRunAction() - exception: " + e);
            }
        }

        private static string getShortIssueSummary(JiraIssue issue) {
            if (issue.Summary == null) {
                return issue.Key;
            }
            if (issue.Summary.Length > MAX_SUMMARY_LENGTH) {
                return issue.Key + ": " + issue.Summary.Substring(0, MAX_SUMMARY_LENGTH) + "...";
            }
            return issue.Key + ": " + issue.Summary;
        }

        private void loadPastActiveIssues(ParameterStore store) {
            pastActiveIssues.Clear();
            int pastIssueCount = Math.Min(store.loadParameter(PAST_ACTIVE_ISSUE_COUNT, 0), ACTIVE_ISSUE_LIST_SIZE);
            for (int i = 0; i < pastIssueCount; ++i) {
                string key = store.loadParameter(PAST_ACTIVE_ISSUE_KEY + i, null);
                string guid = store.loadParameter(PAST_ACTIVE_ISSUE_SERVER_GUID + i, null);
                if (key == null || guid == null) continue;

                ICollection<JiraServer> jiraServers = JiraServerModel.Instance.getAllServers();
                foreach (ActiveIssue issue in from server in jiraServers
                                              where server.GUID.ToString().Equals(guid)
                                              select new ActiveIssue(key, guid) {Enabled = server.Enabled}) {
                    pastActiveIssues.AddLast(issue);
                    break;
                }
            }
            savePastActiveIssuesAndSetupDropDown();
            setNoIssueActiveInDropDown();
        }

        private void setNoIssueActiveInDropDown() {
            setEnabled(false);
            Boolean enableDropDown = pastActiveIssues.Count > 0;
            activeIssueDropDown.Enabled = enableDropDown;
            activeIssueDropDown.Visible = enableDropDown;
            activeIssueDropDown.Text = NO_ISSUE_ACTIVE;
            activeIssueDropDown.Image = null;
            separator.Enabled = enableDropDown;
            separator.Visible = enableDropDown;
            if (ToolbarWidthChanged != null) {
                ToolbarWidthChanged(this, null);
            }
        }

        private void savePastActiveIssuesAndSetupDropDown() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            store.storeParameter(PAST_ACTIVE_ISSUE_COUNT, pastActiveIssues.Count);
            int i = 0;
            foreach (ActiveIssue issue in pastActiveIssues) {
                store.storeParameter(PAST_ACTIVE_ISSUE_KEY + i, issue.Key);
                store.storeParameter(PAST_ACTIVE_ISSUE_SERVER_GUID + i, issue.ServerGuid);
                ++i;
            }
            activeIssueDropDown.DropDown.Items.Clear();
            foreach (var issue in pastActiveIssues.Reverse().Where(issue => issue.Enabled)) {
                activeIssueDropDown.DropDown.Items.Add(new PastActiveIssueMenuItem(this, issue));
            }
            loadPastActiveIssuesDetails();
        }

        private class PastActiveIssueMenuItem : ToolStripMenuItem {
            public ActiveIssue Issue { get; private set; }

            public PastActiveIssueMenuItem(JiraActiveIssueManager mgr, ActiveIssue issue): base(issue.Key) {
                Issue = issue;
                Click += (s, e) => mgr.setActive(issue);
            }
        }

        public bool isActive(JiraIssue issue) {
            if (CurrentActiveIssue == null) {
                return false;
            }
            return issue.Key.Equals(CurrentActiveIssue.Key) &&
                   issue.Server.GUID.ToString().Equals(CurrentActiveIssue.ServerGuid);
        }

        public void toggleActiveState(JiraIssue issue) {
            if (isActive(issue)) {
                deactivateActiveIssue(true, null, null);
            } else {
                setActive(issue);
            }
        }

        private void setActive(JiraIssue issue) {
            setActive(new ActiveIssue(issue.Key, issue.Server.GUID.ToString()));
        }

        private void setActive(ActiveIssue issue) {
            ++generation;
            List<ActiveIssue> toRemove = pastActiveIssues.Where(i => i.Equals(issue)).ToList();
            foreach (var i in toRemove) {
                pastActiveIssues.Remove(i);
            }
            if (CurrentActiveIssue != null) {
                deactivateActiveIssue(false, issue.Key, () => setActivePartDeux(issue, false));
            } else {
                setActivePartDeux(issue, true);
            }
        }

        private void setActivePartDeux(ActiveIssue issue, bool savePastIssues) {
            if (savePastIssues) {
                savePastActiveIssuesAndSetupDropDown();
            }
            CurrentActiveIssue = new ActiveIssue(issue.Key, issue.ServerGuid);

            paused = false;
            savePausedState();

            runActivateIssueActions(() => {
                                        setEnabled(true);
                                        setDropDownTextFromCurrentActiveIssue();
                                        MinutesInProgress = 0;
                                        storeTimeSpent();
                                        setTimeSpentString();
                                        storeActiveIssue();
                                        activeIssueDropDown.Image = null;
                                        loadActiveIssueDetails();
                                        if (ActiveIssueChanged != null) {
                                            ActiveIssueChanged(this, null);
                                        }
                                    });
        }

        private void runActivateIssueActions(Action onFinish) {
            Thread t = PlvsUtils.createThread(() => runActivateIssueActionsWorker(onFinish));
            t.Start();
        }

        private void runActivateIssueActionsWorker(Action onFinish) {
            JiraServer server = JiraServerModel.Instance.getServer(new Guid(CurrentActiveIssue.ServerGuid));
            if (server != null) {
                try {
                    int mods = 0;
                    JiraIssue issue = SmartJiraServerFacade.Instance.getIssue(server, CurrentActiveIssue.Key);
                    if (issue != null) {
                        string me = CredentialUtils.getUserNameWithoutDomain(server.UserName);
                        if (issue.Assignee == null || issue.Assignee.Equals("Unknown") || !issue.Assignee.Equals(me)) {
                            jiraStatus.setInfo("Assigning issue to me...");
                            JiraField assignee = new JiraField("assignee", null) {
                                Values = new List<string> { me },
                                SettablePropertyName = "name"
                            };
                            var rawIssueObject = SmartJiraServerFacade.Instance.getRawIssueObject(issue);
                            assignee.setRawIssueObject(rawIssueObject);
                            SmartJiraServerFacade.Instance.updateIssue(issue, new List<JiraField> { assignee });
                            ++mods;
                        }
                        List<JiraNamedEntity> actions = SmartJiraServerFacade.Instance.getActionsForIssue(issue);
                        JiraNamedEntity action = actions.Find(a => a.Id.Equals(START_PROGRESS_ACTION_ID));
                        if (action == null) {
                            container.safeInvoke(new MethodInvoker(delegate {
                                ActionSelector sel = new ActionSelector(actions);
                                if (sel.ShowDialog() == DialogResult.OK) {
                                    action = sel.SelectedAction;
                                }
                            }));
                        }
//                        foreach (JiraNamedEntity action in actions.Where(action => action.Id.Equals(START_PROGRESS_ACTION_ID))) {
                        if (action != null) {
                            jiraStatus.setInfo("Setting issue in progress...");
                            SmartJiraServerFacade.Instance.runIssueActionWithoutParams(issue, action);
                            ++mods;
                        }
                        if (mods > 0) {
                            issue = SmartJiraServerFacade.Instance.getIssue(server, CurrentActiveIssue.Key);
                        }
                    }
                    jiraStatus.setInfo("Work on issue " + CurrentActiveIssue.Key + " started");
                    container.safeInvoke(new MethodInvoker(() => {
                                                               JiraIssueListModelImpl.Instance.updateIssue(issue);
                                                               onFinish();
                                                           }));
                } catch (Exception e) {
                    CurrentActiveIssue = null;
                    jiraStatus.setError(FAILED_TO_START_WORK, e);
                    container.safeInvoke(new MethodInvoker(() => PlvsUtils.showError(FAILED_TO_START_WORK, e)));
                }
            } else {
                container.safeInvoke(new MethodInvoker(
                    () => PlvsUtils.showError(FAILED_TO_START_WORK, new Exception("Unknown JIRA server"))));
            }
        }

        private void runDeactivateIssueActions(string newActiveIssue, Action onFinish) {
            Thread t = PlvsUtils.createThread(() => runDeactivateIssueActionsWorker(newActiveIssue, onFinish));
            t.Start();
        }

        private void runDeactivateIssueActionsWorker(string newActiveIssue, Action onFinish) {
            JiraServer server = JiraServerModel.Instance.getServer(new Guid(CurrentActiveIssue.ServerGuid));
            if (server != null) {
                try {
                    JiraIssue issue = SmartJiraServerFacade.Instance.getIssue(server, CurrentActiveIssue.Key);
                    if (issue != null) {
                        List<JiraNamedEntity> actions = SmartJiraServerFacade.Instance.getActionsForIssue(issue);
                        container.safeInvoke(
                            new MethodInvoker(
                                () => new DeactivateIssue(newActiveIssue != null ? string.Format(EXPLANATION_TEXT, issue.Key, newActiveIssue) : null,
                                                          container, 
                                                          JiraIssueListModelImpl.Instance,
                                                          SmartJiraServerFacade.Instance,
                                                          ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES),
                                                          issue, 
                                                          jiraStatus, 
                                                          this, 
                                                          actions, 
                                                          () => {
                                                              jiraStatus.setInfo("Work on issue " + CurrentActiveIssue.Key + " stopped");
                                                              onFinish();
                                                          }).ShowDialog()));
                    }
                } catch (Exception e) {
                    jiraStatus.setError(FAILED_TO_START_WORK, e);
                    PlvsUtils.showError(FAILED_TO_START_WORK, e);
                }
            } else {
                PlvsUtils.showError(FAILED_TO_START_WORK, new Exception("Unknown JIRA server"));
            }
        }

        private void loadActiveIssueDetails() {
            JiraServer server = JiraServerModel.Instance.getServer(new Guid(CurrentActiveIssue.ServerGuid));
            if (server == null) return;

            JiraIssue issue = JiraIssueListModelImpl.Instance.getIssue(CurrentActiveIssue.Key, server);
            if (issue == null) {
                Thread t = PlvsUtils.createThread(() => loadActiveIssueDetailsWorker(generation));
                t.Start();
            } else {
                CurrentActiveIssue.Summary = issue.Summary;
                setActiveIssueDropdownTextAndImage(server, issue);
            }
        }

        private void loadPastActiveIssuesDetails() {
            LinkedList<ActiveIssue> past = new LinkedList<ActiveIssue>(pastActiveIssues);
            Thread t = PlvsUtils.createThread(() => loadPastActiveIssuesDetailsWorker(generation, past));
            t.Start();
        }

        private void loadPastActiveIssuesDetailsWorker(int gen, IEnumerable<ActiveIssue> past) {
            List<JiraIssue> issues = (from pastIssue in past
                                      let server = JiraServerModel.Instance.getServer(new Guid(pastIssue.ServerGuid))
                                      where server != null && server.Enabled
                                      select getIssueFromModelOrServer(server, pastIssue.Key)
                                      into issue where issue != null select issue).ToList();
            container.safeInvoke(new MethodInvoker(() => {
                                                       if (gen != generation) return;

                                                       foreach (JiraIssue issue in issues) {
                                                           JiraIssue issueCopy = issue;
                                                           foreach (PastActiveIssueMenuItem it in
                                                               activeIssueDropDown.DropDown.Items.Cast
                                                                   <PastActiveIssueMenuItem>().Where(
                                                                       it => 
                                                                           it.Issue.Key.Equals(issueCopy.Key) 
                                                                           && it.Issue.ServerGuid.Equals(issueCopy.Server.GUID.ToString()))) {
                                                               
                                                               it.Text = getShortIssueSummary(issue);
                                                               ImageCache.ImageInfo imageInfo = ImageCache.Instance.getImage(issue.Server, issue.IssueTypeIconUrl);
                                                               it.Image = imageInfo != null ? imageInfo.Img : null;
                                                           }
                                                       }
                                                   }));
        }

        private static JiraIssue getIssueFromModelOrServer(JiraServer server, string issueKey) {
            try {
                JiraIssue issue = JiraIssueListModelImpl.Instance.getIssue(issueKey, server);
                return issue ?? SmartJiraServerFacade.Instance.getIssue(server, issueKey);
            } catch (Exception e) {
                Debug.WriteLine("JiraActiveIssueManager.getIssueFromModelOrServer() - exception: " + e);
            }
            return null;
        }

        private void setActiveIssueDropdownTextAndImage(JiraServer server, JiraIssue issue) {
            if (CurrentActiveIssue != null && CurrentActiveIssue.Enabled) {
                activeIssueDropDown.Text = getShortIssueSummary(issue);
                ImageCache.ImageInfo imageInfo = ImageCache.Instance.getImage(server, issue.IssueTypeIconUrl);
                activeIssueDropDown.Image = imageInfo != null ? imageInfo.Img : null;
            } else {
                setNoIssueActiveInDropDown();
            }
            if (ToolbarWidthChanged != null) {
                ToolbarWidthChanged(this, null);
            }
        }

        private void storeActiveIssue() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            if (CurrentActiveIssue != null) {
                store.storeParameter(ACTIVE_ISSUE_KEY, CurrentActiveIssue.Key);
                store.storeParameter(ACTIVE_ISSUE_SERVER_GUID, CurrentActiveIssue.ServerGuid);
            } else {
                store.storeParameter(ACTIVE_ISSUE_KEY, null);
                store.storeParameter(ACTIVE_ISSUE_SERVER_GUID, null);
            }
        }

        private void deactivateActiveIssue(bool notifyListeners, string newActiveIssue, Action onFinished) {
            ++generation;

            runDeactivateIssueActions(newActiveIssue, () => {
                                          pastActiveIssues.AddFirst(CurrentActiveIssue);
                                          while (pastActiveIssues.Count > ACTIVE_ISSUE_LIST_SIZE) {
                                              pastActiveIssues.RemoveLast();
                                          }
                                          CurrentActiveIssue = null;
                                          storeActiveIssue();
                                          activeIssueDropDown.Image = null;
                                          savePastActiveIssuesAndSetupDropDown();
                                          setNoIssueActiveInDropDown();
                                          if (notifyListeners && ActiveIssueChanged != null) ActiveIssueChanged(this, null);
                                          if (onFinished != null) onFinished();
                                      });
        }

        public void resetTimeSpent() {
            MinutesInProgress = 0;
            storeTimeSpent();
            setTimeSpentString();
        }
    }
}


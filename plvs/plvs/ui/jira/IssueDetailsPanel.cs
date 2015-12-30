using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using Atlassian.plvs.api;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs.jira;
using Atlassian.plvs.models;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.net;
using Atlassian.plvs.ui.jira.issues;
using Atlassian.plvs.util.jira;
using Atlassian.plvs.windows;
using Atlassian.plvs.util;
using EnvDTE;
using Microsoft.Win32;
using Read64bitRegistryFrom32bitApp;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using Process=System.Diagnostics.Process;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using Thread=System.Threading.Thread;
using Timer = System.Windows.Forms.Timer;

namespace Atlassian.plvs.ui.jira {
    public partial class IssueDetailsPanel : UserControl {
        private JiraIssueListModel model;
        private readonly Solution solution;

        private readonly SmartJiraServerFacade facade = SmartJiraServerFacade.Instance;

        private readonly StatusLabel status;

        private JiraIssue issue;
//        private readonly TabPage myTab;
//        private readonly Action<TabPage> buttonCloseClicked;
        private readonly JiraActiveIssueManager activeIssueManager;

        private bool issueCommentsLoaded;
        private bool issueDescriptionLoaded;
        private bool issueSummaryLoaded;

        private bool issueSubtasksLoaded;
        private bool issueLinksLoaded;
        
        private const int A_LOT = 100000;

        private static readonly string editImagePath = PlvsUtils.getAssemblyBasedLocalFilePath("edit.png");
        private static readonly string nothingImagePath = PlvsUtils.getAssemblyBasedLocalFilePath("nothing.png");

        private WebBrowser issueDescription;
        private WebBrowserWithLabel webAttachmentView;

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern uint ExtractIconEx(string szFileName, int nIconIndex,
           IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

        private IntPtr nextClipboardViewer;

        public IssueDetailsPanel(
            JiraIssueListModel model, Solution solution, JiraIssue issue, //TabPage myTab, 
            ToolWindowStateMonitor toolWindowStateMonitor, //Action<TabPage> buttonCloseClicked, 
            JiraActiveIssueManager activeIssueManager) {

            this.model = model;
            this.solution = solution;

            InitializeComponent();

            status = new StatusLabel(statusStrip, jiraStatus);

            this.issue = issue;

//            this.myTab = myTab;
//            this.buttonCloseClicked = buttonCloseClicked;
            this.activeIssueManager = activeIssueManager;

            dropDownIssueActions.DropDownItems.Add("dummy");

            toolWindowStateMonitor.ToolWindowShown += toolWindowStateMonitor_ToolWindowShown;
            toolWindowStateMonitor.ToolWindowHidden += toolWindowStateMonitor_ToolWindowHidden;

            listViewAttachments.ContextMenuStrip = new ContextMenuStrip();
            listViewAttachments.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Save as...", null,
                                                                                 new EventHandler(saveAttachment)));
            listViewAttachments.ContextMenuStrip.Opening += attachmentsMenuOpening;

            buttonSaveAttachmentAs.Enabled = false;

            toolTipAttachments.SetToolTip(listViewAttachments, "Drop a file here\nto upload it as an attachment");

            createIssueDescriptionPanel();

#if USE_MAZIO
            maybeAddMazioMenu();
#endif

            buttonUploadNew.Image = Resources.icon_addattachment;
            buttonUploadNew.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;

            issueSummary.ScriptErrorsSuppressed = true;

            reinitializeAttachmentView(null);

            nextClipboardViewer = SetClipboardViewer(Handle); 
            onClipboardChanged();

            issueComments.WebBrowserShortcutsEnabled = true;
        }

        private void activeIssueManager_ActiveIssueChanged(object sender, EventArgs e) {
            updateStartStopWorkButton();
        }

        private void updateStartStopWorkButton() {
            bool thisIssueActive = activeIssueManager.CurrentActiveIssue != null
                                   && activeIssueManager.CurrentActiveIssue.Key.Equals(issue.Key)
                                   && activeIssueManager.CurrentActiveIssue.ServerGuid.Equals(issue.Server.GUID.ToString());
            buttonStartStopProgress.Image = thisIssueActive ? Resources.ico_inactiveissue : Resources.ico_activateissue;
            buttonStartStopProgress.Text = thisIssueActive ? "Stop Work" : "Start Work";
        }

#if USE_MAZIO
        private void maybeAddMazioMenu() {
            string mazioDir = null;
#if VS2010
            if (Environment.Is64BitOperatingSystem) {
                mazioDir = RegistryWOW6432.GetRegKey64(RegHive.HKEY_LOCAL_MACHINE, @"SOFTWARE\Kalamon\Mazio", "Install_Dir");
                
            } else {
#else
            {
#endif
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Kalamon\Mazio");
                if (key != null) mazioDir = key.GetValue("Install_Dir") as string;
            }
            if (mazioDir == null) return;

            IntPtr[] hLarge = new[] { IntPtr.Zero };
            IntPtr[] hSmall = new[] { IntPtr.Zero };

            Image mazioImage = null;

            uint iconCount = ExtractIconEx(mazioDir + "\\mazio.exe", 0, hLarge, hSmall, 1);

            // sometimes ExtractIconEx() seems to return some garbage large value. Hence exception block
            try {
                if (iconCount > 0) {
                    Icon extractedIcon = (Icon)Icon.FromHandle(hSmall[0]).Clone();
                    mazioImage = extractedIcon.ToBitmap();
                }

                toolStripAttachmentsMenu.Items.Add("Attach Mazio Screenshot...", mazioImage,
                                                   delegate {
                                                       Thread t = PlvsUtils.createThread(() => {
                                                           string token = facade.getSoapToken(issue.Server);
                                                           if (token == null) return;
                                                           this.safeInvoke(new MethodInvoker(() => runMazio(token)));
                                                       });
                                                       t.Start();
                                                   });
            } catch (Exception e) {
                Debug.WriteLine("IssueDetailsPanel.maybeAddMazioMenu() - exception: " + e.Message);
            }
        }
#endif

        private void runMazio(string token) {
            try {
                Process.Start("mazio:jira:" 
                    + CredentialUtils.getUserNameWithoutDomain(issue.Server.UserName) 
                    + "@" + issue.Server.Url + ":" + issue.Key + "?soaptoken=" + token);
// ReSharper disable EmptyGeneralCatchClause
            } catch (Exception) {
// ReSharper restore EmptyGeneralCatchClause
            }
        }

        /// <summary>
        /// Sigh. VS 2008 dialog designer is too dumb to be able to 
        /// figure out how to lay out this part of the panel properly
        /// </summary>
        private void createIssueDescriptionPanel() {
            issueDescription = new WebBrowser
                               {
                                   Dock = DockStyle.Fill,
                                   IsWebBrowserContextMenuEnabled = false,
                                   ScriptErrorsSuppressed = true,
                                   Location = new Point(0, 0),
                                   MinimumSize = new Size(20, 20),
                                   Name = "issueDescription",
                                   Size = new Size(126, 124),
                                   TabIndex = 1,
                                   WebBrowserShortcutsEnabled = true
                               };

            issueDescription.Navigating += issueDescription_Navigating;
            issueDescription.DocumentCompleted += issueDescription_DocumentCompleted;

            ToolStripContainer tsc = new ToolStripContainer {Dock = DockStyle.Fill};

            ToolStrip ts = new ToolStrip {Dock = DockStyle.None, GripStyle = ToolStripGripStyle.Hidden};

            ToolStripButton buttonEditDescription = new ToolStripButton
                                                    {
                                                        Image = Resources.edit,
                                                        Text = "Edit",
                                                        DisplayStyle = ToolStripItemDisplayStyle.Image
                                                    };
            buttonEditDescription.Click += buttonEditDescription_Click;

            ts.Items.AddRange(new ToolStripItem[] {new ToolStripLabel("Description"), buttonEditDescription});

            tsc.ContentPanel.Controls.Add(issueDescription);
            tsc.TopToolStripPanel.Controls.Add(ts);

            splitContainer.Panel1.Controls.Add(tsc);
        }

        private void init() {
            addModelListeners();
            updateStartStopWorkButton();

            rebuildAllPanels(false);
            status.setInfo("No issue details yet");
            runRefreshThread();

            SolutionUtils.refillAllSolutionProjectItems(solution);
        }

        protected override void OnLoad(EventArgs e) {
            init();
        }

        void toolWindowStateMonitor_ToolWindowShown(object sender, EventArgs e) {
            init();
        }

        void toolWindowStateMonitor_ToolWindowHidden(object sender, EventArgs e) {
            removeModelListeners();
        }

        private void addModelListeners() {
            model.IssueChanged += model_IssueChanged;
            model.ModelChanged += model_ModelChanged;

            activeIssueManager.ActiveIssueChanged += activeIssueManager_ActiveIssueChanged;
        }

        private void removeModelListeners() {
            model.IssueChanged -= model_IssueChanged;
            model.ModelChanged -= model_ModelChanged;
            activeIssueManager.ActiveIssueChanged -= activeIssueManager_ActiveIssueChanged;
        }

        private void model_ModelChanged(object sender, EventArgs e) {
            this.safeInvoke(new MethodInvoker(delegate {
                                         // this is crude. But issueChanged() will do its job of 
                                         // filtering out the proper issue if it still exists in 
                                         // the model and comparing it what we currently have
                                         foreach (var jiraIssue in model.Issues) {
                                             model_IssueChanged(sender, new IssueChangedEventArgs(jiraIssue));
                                         }
                                     }));
        }

        private void model_IssueChanged(object sender, IssueChangedEventArgs e) {
            if (!e.Issue.Server.GUID.Equals(issue.Server.GUID)) return;
            if (e.Issue.Id.Equals(issue.Id)) {
                if (e.Issue.Equals(issue)) return;
                buttonRefresh.Enabled = false;
                runRefreshThread();
            } else if (e.Issue.IsSubtask && e.Issue.ParentKey.Equals(issue.Key)) {
                rebuildSubtasksPanel();
            }
        }

        private void rebuildAllPanels(bool enableRefresh) {
            this.safeInvoke(new MethodInvoker(delegate
                                        {
                                            rebuildSummaryPanel();
                                            rebuildDescriptionPanel();
                                            rebuildCommentsPanel(true);
                                            rebuildSubtasksPanel();
                                            rebuildLinksPanel();
                                            rebuildAttachmentsPanel();
                                            buttonRefresh.Enabled = enableRefresh;
                                        }));
        }

        private void runRefreshThread() {
            Thread worker = PlvsUtils.createThread(delegate {
                                                       try {
                                                           status.setInfo("Retrieving issue details...");
                                                           issue = facade.getIssue(issue.Server, issue.Key);
                                                           status.setInfo("Issue details retrieved");

                                                           // PLVS-133 - this should never happen but does?
                                                           if (model == null) {
                                                               this.safeInvoke(new MethodInvoker(() 
                                                                                        =>
                                                                                        PlvsUtils.showError("Issue List Model was null, please report this as a bug", 
                                                                                                            new Exception("IssueDetailsPanel.runRefreshThread()"))));
                                                               model = JiraIssueListModelImpl.Instance;
                                                           }

                                                           this.safeInvoke(new MethodInvoker(() => model.updateIssue(issue)));
                                                       } catch (Exception e) {
                                                           status.setError("Failed to retrieve issue details", e);
                                                       }
                                                       rebuildAllPanels(true);
                                                   });
            worker.Start();
        }

        private string createCommentsHtml(bool expanded) {
            StringBuilder sb = new StringBuilder();

            sb.Append("<html>\n<head>\n")
                .Append(Resources.comments_css)
                .Append(Resources.toggler_javascript)
                .Append("</head>\n<body>\n");

            for (int i = 0; i < issue.Comments.Count; ++i) {
                sb.Append("<div class=\"comment_header\">")
                    .Append("<div class=\"author\">").Append(
                    JiraServerCache.Instance.getUsers(issue.Server).getUser(issue.Comments[i].Author))
                    .Append(" <span class=\"date\">").Append(
                    JiraIssueUtils.getTimeStringFromIssueDateTime(JiraIssueUtils.getDateTimeFromJiraTimeString(issue.ServerLanguage, issue.Comments[i].Created)))
                    .Append("</span></div>")
                    .Append("<a href=\"javascript:toggle('")
                    .Append(i).Append("', '").Append(i).Append("control');\"><div class=\"toggler\" id=\"")
                    .Append(i).Append("control\">").Append(expanded ? "collapse" : "expand").Append("</div></a></div>\n");

                sb.Append("<div class=\"comment_body\" style=\"display:")
                    .Append(expanded ? "block" : "none").Append(";\" id=\"").Append(i).Append("\">")
                    .Append(createHyperlinedStackTrace(issue.Comments[i].Body)).Append("</div>\n");
            }

            sb.Append("</body></html>");

            return sb.ToString();
        }

        private static readonly Regex STACK_REGEX = new Regex(@"(\s*\w+\S+\(.*\)\s+\w+\s+)(\S+)(:\w+\s+)(\d+)");
        private const int EDITOR_OFFSET = 40;

        private const string SUMMARY_EDIT_TAG = "summary";
        private const string ASSIGNEE_EDIT_TAG = "assignee";
        private const string COMPONENTS_EDIT_TAG = "components";
        private const string FIX_VERSIONS_EDIT_TAG = "fixversions";
        private const string AFFECTS_VERSIONS_EDIT_TAG = "affectsversions";
        private const string PRIORITY_EDIT_TAG = "priority";
        private const string TIMETRACKING_EDIT_TAG = "timetracking";
        private const string ENVIRONMENT_EDIT_TAG = "environment";

        private const string ISSUE_EDIT_URL_TYPE = "issueedit:";
        private const string PARENT_ISSUE_URL_TYPE = "parentissue:";
        private const string SUBTASK_ISSUE_URL_TYPE = "subtaskkey:";
        private const string LINKED_ISSUE_URL_TYPE = "linkedissuekey:";

        private const string STACKLINE_URL_TYPE = "stackline:";
        private const string STACKLINE_LINE_NUMBER_SEPARATOR = "@";

        private static string createHyperlinedStackTrace(string body) {
            var result = new StringBuilder();

            if (body != null) {
                var sr = new StringReader(body);

                var line = sr.ReadLine();
                while (line != null) {
                    if (STACK_REGEX.IsMatch(line)) {
                        line = STACK_REGEX.Replace(line,
                                                   "$1<a href=\"" + STACKLINE_URL_TYPE + "$2" +
                                                   STACKLINE_LINE_NUMBER_SEPARATOR + "$4\">$2$3$4</a>");
                    }
                    result.Append(line).Append('\n');
                    line = sr.ReadLine();
                }
            }

            return result.ToString();
        }

        private string createDescriptionHtml() {
            StringBuilder sb = new StringBuilder();

            sb.Append("<html>\n<head>\n").Append(Resources.summary_and_description_css)
                .Append("\n</head>\n<body class=\"description\">\n")
                .Append(createHyperlinedStackTrace(issue.Description))
                .Append("\n</body>\n</html>\n");

            return sb.ToString();
        }

        private string createSummaryHtml() {
            string timeFields = string.Format((issue.TimeSpent != null
                                               ? Resources.issue_summary_remaining_estimate_editable_html
                                               : Resources.issue_summary_original_estimate_editable_html),
                                          editImagePath,
                                          issue.OriginalEstimate ?? "None",
                                          issue.RemainingEstimate ?? "None",
                                          issue.TimeSpent ?? "None");

            string env = String.IsNullOrEmpty(issue.Environment) ? "None" : issue.Environment;
            // strip <p> tags - with them the pencil makes the whole panel shake and tremble
            if (env.StartsWith("<p>")) {
                env = env.Substring(3);
            }
            if (env.EndsWith("</p>")) {
                env = env.Substring(0, env.LastIndexOf("</p>"));
            }

            string parentKeyOrNothing = issue.IsSubtask
                                   ? string.Format(Resources.issue_summary_parent_issue_html, issue.ParentKey)
                                   : "";
            string tableContents = string.Format(Resources.issue_summary_html,
                                                 editImagePath,
                                                 issue.Summary,
                                                 ImageCache.Instance.getImage(issue.Server, issue.IssueTypeIconUrl).FileUrl ?? nothingImagePath,
                                                 issue.IssueType,
                                                 ImageCache.Instance.getImage(issue.Server, issue.StatusIconUrl).FileUrl ?? nothingImagePath,
                                                 issue.Status,
                                                 ImageCache.Instance.getImage(issue.Server, issue.PriorityIconUrl).FileUrl ?? nothingImagePath,
                                                 issue.Priority ?? "None",
                                                 env,
                                                 JiraServerCache.Instance.getUsers(issue.Server).getUser(issue.Assignee),
                                                 JiraServerCache.Instance.getUsers(issue.Server).getUser(issue.Reporter),
                                                 issue.Resolution,
                                                 JiraIssueUtils.getTimeStringFromIssueDateTime(issue.CreationDate),
                                                 JiraIssueUtils.getTimeStringFromIssueDateTime(issue.UpdateDate),
                                                 issue.Versions.Count > 1 ? "Affects Versions" : "Affects Version",
                                                 createStringList(issue.Versions),
                                                 issue.FixVersions.Count > 1 ? "Fix Versions" : "Fix Version",
                                                 createStringList(issue.FixVersions),
                                                 issue.Components.Count > 1 ? "Components" : "Component",
                                                 createStringList(issue.Components),
                                                 timeFields,
                                                 parentKeyOrNothing);
            return "<html><head>"
                + Resources.summary_and_description_css
                + Resources.toggler_javascript
                + "</head><body><table class=\"summary\">" + tableContents + "</table></body></html>";    
        }

        private static string createStringList(ICollection<string> list) {
            StringBuilder sb = new StringBuilder();
            if (list.Count == 0)
                sb.Append("None");
            else {
                int i = 0;
                foreach (string s in list) {
                    sb.Append(s);
                    if (++i < list.Count)
                        sb.Append(", ");
                }
            }
            return sb.ToString();
        }

        private void rebuildDescriptionPanel() {
            issueDescriptionLoaded = false;
            issueDescription.DocumentText = createDescriptionHtml();
        }

        private void rebuildSummaryPanel() {
            issueSummaryLoaded = false;
            issueSummary.DocumentText = createSummaryHtml();
        }

        private void rebuildCommentsPanel(bool expanded) {
            issueCommentsLoaded = false;
            issueComments.DocumentText = createCommentsHtml(expanded);
        }

        private void rebuildAttachmentsPanel() {
            listViewAttachments.Items.Clear();
            if (!issue.HasAttachments) return;

            foreach (JiraAttachment att in issue.Attachments) {
                listViewAttachments.Items.Add(new JiraAttachmentListViewItem(issue, att));
            }

            webAttachmentView.Browser.Navigate(new Uri("about:blank"));
        }

        private void rebuildLinksPanel() {
            if (issue.HasLinks) {
                if (!issueTabs.TabPages.Contains(tabLinks)) {
                    issueTabs.TabPages.Add(tabLinks);
                }

                issueLinksLoaded = false;

                Thread t = PlvsUtils.createThread(queryLinksAndDisplay);
                t.Start();
            } else {
                if (issueTabs.TabPages.Contains(tabLinks)) {
                    issueTabs.TabPages.Remove(tabLinks);
                }
            }
            
        }

        private void rebuildSubtasksPanel() {
            if (issue.HasSubtasks) {
                if (!issueTabs.TabPages.Contains(tabSubtasks)) {
                    issueTabs.TabPages.Add(tabSubtasks);
                }

                issueSubtasksLoaded = false;

                List<JiraIssue> subsInModel = new List<JiraIssue>();
                List<string> subsToQuery = new List<string>();
                foreach (string key in issue.SubtaskKeys) {
                    JiraIssue sub = model.getIssue(key, issue.Server);
                    if (sub != null) {
                        subsInModel.Add(sub);
                    } else {
                        subsToQuery.Add(key);
                    }
                }
                Thread t = PlvsUtils.createThread(() => querySubtasksAndDisplay(subsInModel, subsToQuery));
                t.Start();
            } else {
                if (issueTabs.TabPages.Contains(tabSubtasks)) {
                    issueTabs.TabPages.Remove(tabSubtasks);
                }
            }
        }

        private static void setWebBrowserWidgetText(WebBrowser widget, string txt) {
            widget.DocumentText =
                "<html>\n<head>\n"
                + Resources.summary_and_description_css
                + "\n</head>\n<body class=\"summary\">\n"
                + txt
                + "\n</body>\n</html>\n";
        }

        private void queryLinksAndDisplay() {
            try {
                StringBuilder sb = new StringBuilder();
                sb.Append("<html>\n<head>\n").Append(Resources.summary_and_description_css)
                    .Append("\n</head>\n<body>\n<div class=\"summary\">\n");

                foreach (IssueLinkType linkType in issue.IssueLinks) {
                    sb.Append("<div class=\"linkcategory\">").Append(linkType.Name).Append("</div>\n");
                    if (linkType.OutwardLinksName != null && linkType.OutwardLinks != null) {
                        sb.Append("<div class=\"linkdirection\">").Append(linkType.OutwardLinksName).Append("</div>");
                        sb.Append("\n<div class=\"linkedissues\"><table class=\"summary\">\n");
                        foreach (JiraIssue linkedIssue in linkType.OutwardLinks.Select(
                            key => model.getIssue(key, issue.Server) ?? facade.getIssue(issue.Server, key))
                            .Where(linkedIssue => linkedIssue != null)) {
                            
                            appendIssueHtml(sb, linkedIssue, LINKED_ISSUE_URL_TYPE);
                        }
                        sb.Append("\n</table></div>");
                    }
                    if (linkType.InwardLinksName != null && linkType.InwardLinks != null) {
                        sb.Append("<div class=\"linkdirection\">").Append(linkType.InwardLinksName).Append("</div>");
                        sb.Append("\n<div class=\"linkedissues\"><table class=\"summary\">\n");
                        foreach (JiraIssue linkedIssue in linkType.InwardLinks.Select(
                            key => model.getIssue(key, issue.Server) ?? facade.getIssue(issue.Server, key))
                            .Where(linkedIssue => linkedIssue != null)) {
                            
                            appendIssueHtml(sb, linkedIssue, LINKED_ISSUE_URL_TYPE);
                        }
                        sb.Append("\n</table></div>");
                    }
                }
                sb.Append("\n</body>\n</html>\n");
                this.safeInvoke(new MethodInvoker(delegate { webLinkedIssues.DocumentText = sb.ToString(); }));

            } catch (Exception e) {
                this.safeInvoke(new MethodInvoker(() => setWebBrowserWidgetText(webLinkedIssues, "Failed to retrieve issue links")));
                status.setError("Failed to retrieve issue links", e);
            }
        }

        private void querySubtasksAndDisplay(ICollection<JiraIssue> subsInModel, IEnumerable<string> subsToQuery) {
            StringBuilder sb = new StringBuilder();

            try {
                foreach (JiraIssue sub in subsToQuery.Select(key => facade.getIssue(issue.Server, key))) {
                    subsInModel.Add(sub);
                }
                this.safeInvoke(new MethodInvoker(delegate {
                    sb.Append("<html>\n<head>\n").Append(Resources.summary_and_description_css)
                        .Append("\n</head>\n<body>\n<table class=\"summary\">\n");
                    foreach (JiraIssue sub in subsInModel) {
                        appendIssueHtml(sb, sub, SUBTASK_ISSUE_URL_TYPE);
                    }
                    sb.Append("\n</table>\n</body>\n</html>\n");
                    webSubtasks.DocumentText = sb.ToString();
                }));
            }
            catch (Exception e) {
                this.safeInvoke(new MethodInvoker(() => setWebBrowserWidgetText(webSubtasks, "Failed to retrieve subtasks")));
                status.setError("Failed to retrieve subtasks", e);
            }                
        }

        private static void appendIssueHtml(StringBuilder sb, JiraIssue jiraIssue, string targetlinktype) {
            sb.Append("<tr>");

            // let's pray all issue icons are 16x16 :)
            sb.Append("<td class=\"issueelement\" width=\"16px\">").Append("<img src=\"" + (ImageCache.Instance.getImage(jiraIssue.Server, jiraIssue.IssueTypeIconUrl).FileUrl ?? nothingImagePath) + "\"/>").Append("</td>");
            sb.Append("<td class=\"issueelement\">").Append("<a href=\"").Append(targetlinktype).Append(jiraIssue.Key).Append("\">").Append(jiraIssue.Key).Append("</a></td>");
            sb.Append("<td class=\"issueelement\" width=\"16px\">").Append("<img src=\"" + (ImageCache.Instance.getImage(jiraIssue.Server, jiraIssue.PriorityIconUrl).FileUrl ?? nothingImagePath) + "\" alt=\"" + jiraIssue.Priority + "\"/>").Append("</td>");
            sb.Append("<td class=\"issueelement\" width=\"16px\">").Append("<img src=\"" + (ImageCache.Instance.getImage(jiraIssue.Server, jiraIssue.StatusIconUrl).FileUrl ?? nothingImagePath) + "\" alt=\"" + jiraIssue.Status + "\"/>").Append("</td>");
            sb.Append("<td class=\"issueelement\">").Append(jiraIssue.Summary).Append("</td></tr>\n");
        }

        private void buttonAddComment_Click(object sender, EventArgs e) {
            NewIssueComment dlg = new NewIssueComment(issue, facade);
            dlg.ShowDialog();
            if (dlg.DialogResult != DialogResult.OK) return;

            Thread addCommentThread = PlvsUtils.createThread(delegate {
                                                                 try {
                                                                     status.setInfo("Adding comment to issue...");
                                                                     facade.addComment(issue, dlg.CommentBody);
                                                                     status.setInfo("Comment added, refreshing view...");
                                                                     UsageCollector.Instance.bumpJiraIssuesOpen();
                                                                     runRefreshThread();
                                                                 }
                                                                 catch (Exception ex) {
                                                                     status.setError("Adding comment failed", ex);
                                                                 }
                                                             });
            addCommentThread.Start();
        }

        private void buttonExpandAll_Click(object sender, EventArgs e) {
            rebuildCommentsPanel(true);
        }

        private void buttonCollapseAll_Click(object sender, EventArgs e) {
            rebuildCommentsPanel(false);
        }

        private void buttonRefresh_Click(object sender, EventArgs e) {
            buttonRefresh.Enabled = false;
            runRefreshThread();
        }

        public void closed() {
            removeModelListeners();
            ChangeClipboardChain(Handle, nextClipboardViewer);
        }

//        private void buttonClose_Click(object sender, EventArgs e) {
//            removeModelListeners();

//            if (buttonCloseClicked != null) {
//                buttonCloseClicked(myTab);
//            }
//        }

        private void issueSummary_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            issueSummaryLoaded = true;
        }

        private void issueDescription_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            issueDescriptionLoaded = true;
        }

        private void issueComments_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            issueCommentsLoaded = true;
// ReSharper disable PossibleNullReferenceException
            issueComments.Document.Body.ScrollTop = A_LOT;
// ReSharper restore PossibleNullReferenceException
        }

        private void webLinkedIssues_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            issueLinksLoaded = true;
        }

        private void webSubtasks_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            issueSubtasksLoaded = true;
        }

        private void issueComments_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (!issueCommentsLoaded) return;
            if (e.Url.ToString().Equals("about:blank")) {
                e.Cancel = true;
                return;
            }

            if (e.Url.ToString().StartsWith("javascript:toggle(")) return;
            if (handleStackUrl(e)) return;
            navigate(e);
        }

        private bool handleStackUrl(WebBrowserNavigatingEventArgs e) {
            string line = e.Url.ToString();

            if (line.StartsWith(STACKLINE_URL_TYPE) && line.LastIndexOf(STACKLINE_LINE_NUMBER_SEPARATOR) != -1) {

                string file = line.Substring(STACKLINE_URL_TYPE.Length,
                                             line.LastIndexOf(STACKLINE_LINE_NUMBER_SEPARATOR) -
                                             STACKLINE_URL_TYPE.Length);

                string lineNoStr = line.Substring(line.LastIndexOf(STACKLINE_LINE_NUMBER_SEPARATOR) + 1);

                SolutionUtils.openSolutionFile(file, lineNoStr, solution);

                e.Cancel = true;
                return true;
            }
            return false;
        }

        private void issueDescription_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (!issueDescriptionLoaded) return;
            if (handleStackUrl(e)) return;
            if (e.Url.Equals("about:blank")) {
                e.Cancel = true;
                return;
            }
            navigate(e);
        }

        private void issueSummary_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            PlvsLogger.log("issueSummary_Navigating() - enter");
            if (!issueSummaryLoaded) {
                PlvsLogger.log("issueSummary_Navigating() - issueSummaryLoaded == false - exiting");
                return;
            }
            if (e.Url.Equals("about:blank")) {
                PlvsLogger.log("issueSummary_Navigating() - URL == about:blank - exiting");
                e.Cancel = true;
                return;
            }
            if (e.Url.ToString().StartsWith(PARENT_ISSUE_URL_TYPE)) {
                PlvsLogger.log("issueSummary_Navigating() - opening issue " + e.Url.ToString().Substring(PARENT_ISSUE_URL_TYPE.Length));
                AtlassianPanel.Instance.Jira.findAndOpenIssue(
                    e.Url.ToString().Substring(PARENT_ISSUE_URL_TYPE.Length), openIssueFinished);
                e.Cancel = true;
                return;
            }
            string title = null;
            if (e.Url.ToString().StartsWith(ISSUE_EDIT_URL_TYPE)) {
                PlvsLogger.log("issueSummary_Navigating() - editing issue field " + e.Url.ToString().Substring(ISSUE_EDIT_URL_TYPE.Length));
                string fieldId = null;
                switch (e.Url.ToString().Substring(ISSUE_EDIT_URL_TYPE.Length)) {
                    case SUMMARY_EDIT_TAG:
                        title = "Edit Summary";
                        fieldId = "summary";
                        break;
                    case PRIORITY_EDIT_TAG:
                        title = "Edit Priority";
                        fieldId = "priority";
                        break;
                    case FIX_VERSIONS_EDIT_TAG:
                        title = "Edit Fix Versions";
                        fieldId = "fixVersions";
                        break;
                    case AFFECTS_VERSIONS_EDIT_TAG:
                        title = "Edit Affects Versions";
                        fieldId = "versions";
                        break;
                    case COMPONENTS_EDIT_TAG:
                        title = "Edit Components";
                        fieldId = "components";
                        break;
                    case ASSIGNEE_EDIT_TAG:
                        title = "Edit Assignee";
                        fieldId = "assignee";
                        break;
                    case ENVIRONMENT_EDIT_TAG:
                        title = "Edit Environment";
                        fieldId = "environment";
                        break;
                    case TIMETRACKING_EDIT_TAG:
                        title = issue.TimeSpent != null ? "Edit Remaining Estimate" : "Edit Original Estimate";
                        fieldId = "timetracking";
                        break;
                }
                if (fieldId != null) {
                    PlvsLogger.log("issueSummary_Navigating() - editing issue field fieldId == " + fieldId);

                    Point pt = Cursor.Position;
                    pt.X = Math.Max(pt.X - EDITOR_OFFSET, 0);
                    pt.Y = Math.Max(pt.Y - EDITOR_OFFSET, 0);
                    
                    FieldEditor editor = new FieldEditor(title, model, facade, issue, fieldId, pt);
                    editor.ShowDialog();
                } else {
                    PlvsLogger.log("issueSummary_Navigating() - editing issue field fieldId == null");
                }
                e.Cancel = true;
                return;
            }
            PlvsLogger.log("issueSummary_Navigating() - navigating to " + e);
            navigate(e);
        }

        private void webLinkedIssues_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            webBrowserNavigating(LINKED_ISSUE_URL_TYPE, e, issueLinksLoaded);
        }

        private void webSubtasks_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            webBrowserNavigating(SUBTASK_ISSUE_URL_TYPE, e, issueSubtasksLoaded);
        }

        private static void webBrowserNavigating(string targetlinktype, WebBrowserNavigatingEventArgs e, bool toTest) {
            if (e.Url.Equals("about:blank")) return;
            if (!toTest) return;
            if (e.Url.ToString().StartsWith(targetlinktype)) {
                AtlassianPanel.Instance.Jira.findAndOpenIssue(
                    e.Url.ToString().Substring(targetlinktype.Length), openIssueFinished);
                e.Cancel = true;
                return;
            }
            navigate(e);
        }

        private static void openIssueFinished(bool success, string message, Exception e) {
            if (!success) {
                PlvsUtils.showError(message, e);
            }
        }

        private static void navigate(WebBrowserNavigatingEventArgs e) {
            string url = e.Url.ToString();
            try {
                PlvsUtils.runBrowser(url);
// ReSharper disable EmptyGeneralCatchClause
            } catch (Exception) {
// ReSharper restore EmptyGeneralCatchClause
            }
            e.Cancel = true;
        }

        private void buttonViewInBrowser_Click(object sender, EventArgs e) {
            try {
                PlvsUtils.runBrowser(issue.Server.Url + "/browse/" + issue.Key);
            } catch (Exception ex) {
                Debug.WriteLine("IssueDetailsPanel.buttonViewInBrowser_Click() - exception: " + ex);
            }
            UsageCollector.Instance.bumpJiraIssuesOpen();
        }

        private void dropDownIssueActions_DropDownOpened(object sender, EventArgs e) {
            dropDownIssueActions.DropDownItems.Clear();

            PlvsUtils.addPhonyMenuItemFixingPlvs109(dropDownIssueActions);
            dropDownIssueActions.DropDownItems.Add(new ToolStripMenuItem
                                                   {Text = "Loading issue actions...", Enabled = false});
            dropDownIssueActions.ToolTipText = "";
            Thread loaderThread = PlvsUtils.createThread(addIssueActionItems);
            loaderThread.Start();
        }

        private void dropDownIssueActionsMouseEnter(object sender, EventArgs e) {
            dropDownIssueActions.ToolTipText = "Issue Actions";
        }

        private void addIssueActionItems() {
            List<JiraNamedEntity> actions = null;
            try {
                status.setInfo("Retrieving issue actions...");
                actions = SmartJiraServerFacade.Instance.getActionsForIssue(issue);
                status.setInfo("Issue actions retrieved");
            } catch (Exception ex) {
                status.setError("Failed to retrieve issue actions", ex);
            }
            if (actions == null || actions.Count == 0) {
                this.safeInvoke(new MethodInvoker(delegate {
                                             dropDownIssueActions.DropDownItems.Clear();
                                             dropDownIssueActions.DropDownItems.Add(new ToolStripMenuItem
                                                                                    {
                                                                                        Text = "No issue actions are available",
                                                                                        Enabled = false
                                                                                    });
                                             PlvsUtils.addPhonyMenuItemFixingPlvs109(dropDownIssueActions);
                }));
                return;
            }

            this.safeInvoke(new MethodInvoker(delegate {
                                         dropDownIssueActions.DropDownItems.Clear();
                                         PlvsUtils.addPhonyMenuItemFixingPlvs109(dropDownIssueActions);
                                         foreach (ToolStripMenuItem item in from action in actions
                                                                            let actionCopy = action
                                                                            select new ToolStripMenuItem(action.Name, null, 
                                                                                new EventHandler(
                                                                                    delegate {
                                                                                        IssueActionRunner.runAction(this, actionCopy, model, issue, status, null);
                                                                                    }))) {
                                             dropDownIssueActions.DropDownItems.Add(item);
                                         }
                                     }));
        }

        private void buttonLogWorkClick(object sender, EventArgs e) {
            new LogWork(this, model, facade, issue, status, activeIssueManager).ShowDialog();
        }

        private void listViewAttachmentsClick(object sender, EventArgs e) {
            if (listViewAttachments.SelectedItems.Count == 0) return;

            JiraAttachmentListViewItem item = listViewAttachments.SelectedItems[0] as JiraAttachmentListViewItem;
            if (item == null) return;

            if (isInlineNavigable(item.Attachment.Name)) {
                try {
                    webAttachmentView.Browser.navigateWithProxy(item.Url + "?" + CredentialUtils.getOsAuthString(issue.Server));
                } catch (COMException ex) {
                    Debug.WriteLine("IssueDetailsPanel.listViewAttachments_Click() - exception caught: " + ex.Message);
                    reinitializeAttachmentView(() => showUnableToViewAttachmentPage(""));
                }
            } else {
                try {
                    showUnableToViewAttachmentPage("due to unsupported attachment type.<br>Double-click to it open using associated external program");
                } catch (COMException ex) {
                    Debug.WriteLine("IssueDetailsPanel.listViewAttachments_Click() - exception caught: " + ex.Message);
                    reinitializeAttachmentView(() => showUnableToViewAttachmentPage(""));
                }
            }
        }

        private void showUnableToViewAttachmentPage(string cause) {
            webAttachmentView.Browser.DocumentText = string.Format(webAttachmentView.ErrorString, Font.FontFamily.Name, cause);
        }

        private void reinitializeAttachmentView(Action onReinserted) {
            splitContainerAttachments.Panel2.SuspendLayout();

            if (webAttachmentView != null) {
                splitContainerAttachments.Panel2.Controls.Remove(webAttachmentView);
            }

            webAttachmentView = new WebBrowserWithLabel
                                {
                                    Dock = DockStyle.Fill,
                                    Location = new Point(0, 24),
                                    MinimumSize = new Size(20, 20),
                                    Name = "webAttachmentView",
                                    Size = new Size(433, 378),
                                    TabIndex = 0,
                                    Title = "Attachment Preview",
                                    ErrorString = Resources.attachment_download_html
                                };
            
            splitContainerAttachments.Panel2.Controls.Add(webAttachmentView);
            splitContainerAttachments.Panel2.ResumeLayout(true);
            
            if (onReinserted == null) return;

            // lame as hell. How can I tell when exactly it is kosher 
            // to set WebBrowser control contents after adding it to parent?
            Timer t = new Timer { Interval = 1000 };
            t.Tick += delegate { t.Stop(); onReinserted(); };
            t.Start();
        }

        private const int WM_DRAWCLIPBOARD = 0x0308;
        private const int WM_CHANGECBCHAIN = 0x030D;

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case WM_DRAWCLIPBOARD:
                    onClipboardChanged();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;
                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer) {
                        nextClipboardViewer = m.LParam;
                    } else {
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void attachmentsMenuOpening(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = !(listViewAttachments.SelectedItems.Count > 0);
        }

        private void saveAttachment(object sender, EventArgs e) {
            if (listViewAttachments.SelectedItems.Count == 0) return;

            JiraAttachmentListViewItem item = listViewAttachments.SelectedItems[0] as JiraAttachmentListViewItem;
            if (item == null) return;

            SaveFileDialog dlg = new SaveFileDialog {FileName = item.Attachment.Name};

            if (dlg.ShowDialog() != DialogResult.OK) return;

            saveAttachmentToStream(item, dlg.OpenFile());
        }

        private void saveAttachmentToStream(JiraAttachmentListViewItem item, Stream stream) {
            saveAttachmentToStream(item, stream, null);
        }

        private void saveAttachmentToStream(JiraAttachmentListViewItem item, Stream stream, Action onComplete) {
            status.setInfo("Saving attachment \"" + item.Attachment.Name + "\"...");
            var client = new WebClient();
            client.DownloadDataCompleted += ((sender, e) => {
                downloadDataCompleted(item.Attachment.Name, e, stream);
                if (onComplete != null) {
                    onComplete();
                }
            });
            client.DownloadDataAsync(new Uri(item.Url + "?" + CredentialUtils.getOsAuthString(issue.Server)));
        }

        private void downloadDataCompleted(string name, DownloadDataCompletedEventArgs e, Stream stream) {
            if (e.Error != null) {
                status.setError("Failed to save attachment \"" + name + "\"", e.Error);
                return;
            }
            stream.Write(e.Result, 0, e.Result.Length);
            stream.Flush();
            stream.Close();

            status.setInfo("Attachment \"" + name + "\" saved");
        }

        private static bool isInlineNavigable(string name) {
            // hmm hmm, will these be typical files that are 
            // (1) openable by IE and 
            // (2) usually interesting for users to view?
            var lname = name.ToLower();
            return lname.EndsWith(".jpg") 
                || lname.EndsWith(".png") 
                || lname.EndsWith(".gif") 
                || lname.EndsWith(".txt") 
                || lname.EndsWith(".xml")
                || lname.EndsWith(".log");
        }

        private void listViewAttachmentsSizeChanged(object sender, EventArgs e) {
            columnName.Width = listViewAttachments.Width / 2;
            columnAuthor.Width = listViewAttachments.Width / 6;
            columnSize.Width = listViewAttachments.Width / 6;
            columnDate.Width = -2;
        }

        private void listViewAttachmentsSelectedIndexChanged(object sender, EventArgs e) {
            buttonSaveAttachmentAs.Enabled = listViewAttachments.SelectedItems.Count > 0;
        }

        private void uploadAttachment(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();

            if (dlg.ShowDialog() != DialogResult.OK) return;

            string fileName = dlg.SafeFileName;

            try {
                readFileFromStreamAndUpload(dlg.OpenFile(), fileName);
            } catch (IOException ex) {
                status.setError("Failed to read attachment \"" + fileName + "\" from file", ex);
                listViewAttachments.AllowDrop = true;
            } catch (Exception ex) {
                status.setError("Failed to upload attachment \"" + fileName + "\"", ex);
                listViewAttachments.AllowDrop = true;
            } 
        }

        private void readFileFromStreamAndUpload(Stream stream, string fileName) {
            listViewAttachments.AllowDrop = false;

            long length = stream.Length;
            byte[] att = new byte[length];
            BinaryReader reader = new BinaryReader(stream);
            for (int i = 0; i < length; ++i) {
                att[i] = reader.ReadByte();
            }

            status.setInfo("Uploading attachment \"" + fileName + "\"...");
            Thread t = PlvsUtils.createThread(() => uploadAttachmentWorker(fileName, att));
            t.Start();
        }

        private void uploadAttachmentWorker(string name, byte[] attachment) {
            try {
                facade.uploadAttachment(issue, name, attachment);
                JiraIssue updatedIssue = facade.getIssue(issue.Server, issue.Key);
                status.setInfo("Uploaded attachment \"" + name + "\"");
                this.safeInvoke(new MethodInvoker(() => model.updateIssue(updatedIssue)));
            } catch (Exception e) {
                status.setError("Failed to upload attachment \"" + name + "\"", e);
            } finally {
                this.safeInvoke(new MethodInvoker(delegate { listViewAttachments.AllowDrop = true; }));
            }
        }

        private void listViewAttachments_DragDrop(object sender, DragEventArgs e) {
            if (!listViewAttachments.AllowDrop) return;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] fileNames = (string[]) e.Data.GetData(DataFormats.FileDrop);

            if (fileNames.Length != 1) return;

            string name = fileNames[0].Contains("\\") 
                ? fileNames[0].Substring(fileNames[0].LastIndexOf("\\") + 1) 
                : fileNames[0];

            FileStream stream = new FileStream(fileNames[0], FileMode.Open);

            try {
                readFileFromStreamAndUpload(stream, name);
            } catch (IOException ex) {
                status.setError("Failed to read attachment \"" + name + "\" from file", ex);
                listViewAttachments.AllowDrop = true;
            } catch (Exception ex) {
                status.setError("Failed to upload attachment \"" + name + "\"", ex);
                listViewAttachments.AllowDrop = true;
            } 
        }

        private void listViewAttachments_DragEnter(object sender, DragEventArgs e) {
            if (!listViewAttachments.AllowDrop) return;

            e.Effect = DragDropEffects.None;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (fileNames.Length != 1) return;

            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void buttonEditDescription_Click(object sender, EventArgs e) {
            FieldEditor editor = new FieldEditor("Edit Description", model, facade, issue, "description", Cursor.Position);
            editor.setCustomSize(new Size(400, 300));
            editor.ShowDialog();
        }

        private void buttonStartStopProgressClick(object sender, EventArgs e) {
            activeIssueManager.toggleActiveState(issue);
        }

        private void onClipboardChanged() {
            buttonPaste.Enabled = Clipboard.ContainsImage();
        }

        private void buttonPasteClick(object sender, EventArgs e) {
            if (Clipboard.ContainsImage()) {
                Image image = Clipboard.GetImage();
                if (image == null) return;
                string ext = null;
                if (ImageFormat.Jpeg.Equals(image.RawFormat)) {
                    ext = ".jpg";
                } else if (ImageFormat.Png.Equals(image.RawFormat)) {
                    ext = ".png";
                } else if (ImageFormat.Gif.Equals(image.RawFormat)) {
                    ext = ".gif";
                }
                string tempFileName = Path.GetTempFileName();
                image.Save(tempFileName, ext != null ? image.RawFormat : ImageFormat.Png);
                if (ext == null) {
                    ext = ".png";
                }
                image.Dispose();
                FileStream stream = new FileStream(tempFileName, FileMode.Open);
                readFileFromStreamAndUpload(stream, "from-clipboard-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ext);
            }
        }

        private void listViewAttachmentsMouseDoubleClick(object sender, MouseEventArgs e) {
            openAttachmentFromTemp();
        }

        private void listViewAttachmentsKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Enter) {
                openAttachmentFromTemp();
            }
        }

        private void openAttachmentFromTemp() {
            if (listViewAttachments.SelectedItems.Count == 0) return;

            var item = listViewAttachments.SelectedItems[0] as JiraAttachmentListViewItem;
            if (item == null) return;
            var path = Path.GetTempPath() + item.Attachment.Name;
            if (File.Exists(path)) {
                File.Delete(path);
            }
            var stream = File.Create(path);
            saveAttachmentToStream(item, stream, () => {
                try {
                    Process.Start(path);
                } catch(Exception e) {
                    PlvsUtils.showError("Unable to open attachment", e);
                }
            });
        }
    }
}
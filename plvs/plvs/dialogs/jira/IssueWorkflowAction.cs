using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.ui.jira.fields;
using Atlassian.plvs.util;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.dialogs.jira {
    public sealed partial class IssueWorkflowAction : Form {
        private readonly JiraIssue issue;
        private readonly JiraNamedEntity action;
        private readonly JiraIssueListModel model;
        private readonly ICollection<JiraField> fields;
        private readonly StatusLabel status;
        private readonly Action onFinish;

        private int verticalPosition;
        private int tabIndex;

        private readonly JiraTextAreaWithWikiPreview textComment = new JiraTextAreaWithWikiPreview();
        private TextBox textUnsupported;
        private readonly List<JiraFieldEditorProvider> editors = new List<JiraFieldEditorProvider>();

        private const int LABEL_X_POS = 0;
        private const int FIELD_X_POS = 120;

        private const int LABEL_HEIGHT = 13;

        private const int MARGIN = 16;

        private const int INITIAL_WIDTH = 700;
        private const int INITIAL_HEIGHT = 500;

        private List<JiraNamedEntity> issueTypes = new List<JiraNamedEntity>();
        private List<JiraNamedEntity> subtaskIssueTypes = new List<JiraNamedEntity>();
        private List<JiraNamedEntity> versions = new List<JiraNamedEntity>();
        private List<JiraNamedEntity> comps = new List<JiraNamedEntity>();

        public IssueWorkflowAction(
            JiraIssue issue, JiraNamedEntity action, JiraIssueListModel model, 
            List<JiraField> fields, StatusLabel status, Action onFinish) {

            this.issue = issue;
            this.action = action;
            this.model = model;
            this.fields = JiraActionFieldType.sortFieldList(fields);

            this.status = status;
            this.onFinish = onFinish;

            InitializeComponent();

            Text = issue.Key + ": " + action.Name;

            ClientSize = new Size(INITIAL_WIDTH, INITIAL_HEIGHT + buttonOk.Height + 3 * MARGIN);

            buttonOk.Enabled = false;

            StartPosition = FormStartPosition.CenterParent;

            panelThrobber.Visible = true;
            panelContent.Visible = false;
        }

        public void initAndShowDialog() {
            initializeFields();
        }

        private void initializeFields() {
            Thread t = PlvsUtils.createThread(initializeThreadWorker);
            t.Start();
            ShowDialog();
        }

        private void initializeThreadWorker() {
            SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(issue.Server);
            if (projects.ContainsKey(issue.ProjectKey)) {
                status.setInfo("Retrieving issue field data...");
                JiraProject project = projects[issue.ProjectKey];
                issueTypes = SmartJiraServerFacade.Instance.getIssueTypes(issue.Server, project);
                subtaskIssueTypes = SmartJiraServerFacade.Instance.getSubtaskIssueTypes(issue.Server, project);
                versions = SmartJiraServerFacade.Instance.getVersions(issue.Server, project);
                comps = SmartJiraServerFacade.Instance.getComponents(issue.Server, project);

                status.setInfo("");

                if (!Visible) return;

                this.safeInvoke(new MethodInvoker(delegate {
                                             panelContent.Visible = true;
                                             panelThrobber.Visible = false;

                                             verticalPosition = 0;

                                             fillFields();
                                             addCommentField();

                                             if (textUnsupported != null) {
                                                 textUnsupported.Location = new Point(LABEL_X_POS, verticalPosition);
                                                 panelContent.Controls.Add(textUnsupported);
                                                 verticalPosition += textUnsupported.Height + MARGIN;
                                             }

                                             ClientSize = new Size(INITIAL_WIDTH,
                                                                   Math.Min(verticalPosition, INITIAL_HEIGHT) + buttonOk.Height + 4*MARGIN);

                                             // resize to perform layout
                                             Size = new Size(Width + 1, Height + 1);

                                             updateOkButton();

                                         }));
            } else {
                status.setInfo("");
                if (Visible) {
                    Invoke(new MethodInvoker(() => MessageBox.Show("Unable to retrieve issue data", Constants.ERROR_CAPTION,
                                                                   MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }
        }


        private void resizeStaticContent() {
            panelContent.Location = new Point(MARGIN, MARGIN);
            panelContent.SuspendLayout();
            panelContent.Size = new Size(ClientSize.Width - 2*MARGIN, ClientSize.Height - 3*MARGIN - buttonOk.Height);
            panelContent.ResumeLayout(true);

            buttonOk.Location = new Point(ClientSize.Width - 2*buttonOk.Width - 3*MARGIN/2,
                                          ClientSize.Height - MARGIN - buttonOk.Height);
            buttonCancel.Location = new Point(ClientSize.Width - buttonOk.Width - MARGIN,
                                              ClientSize.Height - MARGIN - buttonCancel.Height);
        }

        private void fieldValid(JiraFieldEditorProvider sender, bool valid) {
            updateOkButton();    
        }

        private void updateOkButton() {
            if (editors.Any(editor => !editor.FieldValid)) {
                buttonOk.Enabled = false;
                return;
            }
            buttonOk.Enabled = true;
        }

        private void fillFields() {
            List<JiraField> unsupportedFields = new List<JiraField>();

            foreach (JiraField field in fields) {
                JiraFieldEditorProvider editor = null;
                switch (JiraActionFieldType.getFieldTypeForField(field)) {
                    case JiraActionFieldType.WidgetType.SUMMARY:
                        editor = new TextLineFieldEditorProvider(field, field.Values.IsNullOrEmpty() ? "" : field.Values[0], fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.DESCRIPTION:
                        editor = new TextAreaFieldEditorProvider(SmartJiraServerFacade.Instance, issue, field, field.Values.IsNullOrEmpty() ? "" : field.Values[0], fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.ENVIRONMENT:
                        editor = new TextAreaFieldEditorProvider(SmartJiraServerFacade.Instance, issue, field, field.Values.IsNullOrEmpty() ? "" : field.Values[0], fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.ISSUE_TYPE:
                        editor = new NamedEntityComboEditorProvider(issue.Server, field, issue.IssueTypeId, issue.IsSubtask ? subtaskIssueTypes : issueTypes, fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.VERSIONS:
                        editor = new NamedEntityListFieldEditorProvider(field, issue.Versions, versions, fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.FIX_VERSIONS:
                        editor = new NamedEntityListFieldEditorProvider(field, issue.FixVersions, versions, fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.ASSIGNEE:
                        editor = new UserFieldEditorProvider(issue.Server, field, field.Values.IsNullOrEmpty() ? "" : field.Values[0], fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.REPORTER:
                        editor = new UserFieldEditorProvider(issue.Server, field, field.Values.IsNullOrEmpty() ? "" : field.Values[0], fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.DUE_DATE:
                        editor = new DateFieldEditorProvider(issue.ServerLanguage, field, field.Values.IsNullOrEmpty()
                                                                        ? (DateTime?) null 
                                                                        : JiraIssueUtils.getDateTimeFromShortString(issue.ServerLanguage, field.Values[0]), fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.COMPONENTS:
                        editor = new NamedEntityListFieldEditorProvider(field, issue.Components, comps, fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.RESOLUTION:
                        SortedDictionary<int, JiraNamedEntity> resolutions = JiraServerCache.Instance.getResolutions(issue.Server);
                        editor = new ResolutionEditorProvider(issue.Server, field, issue.ResolutionId, resolutions != null ? resolutions.Values : null, fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.PRIORITY:
                        editor = new NamedEntityComboEditorProvider(issue.Server, field, issue.PriorityId, JiraServerCache.Instance.getPriorities(issue.Server), fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.TIMETRACKING:
                        editor = new TimeTrackingEditorProvider(field, field.Values.IsNullOrEmpty() ? "" : field.Values[0], fieldValid);
                        break;
// ReSharper disable RedundantCaseLabel
                    case JiraActionFieldType.WidgetType.SECURITY:
                    case JiraActionFieldType.WidgetType.UNSUPPORTED:
// ReSharper restore RedundantCaseLabel
                    default:
                        if (!JiraActionFieldType.isTimeField(field)) {
                            unsupportedFields.Add(field);
                        }
                        break;
                }

                if (editor == null) continue;

                addLabel(editor.getFieldLabel(issue, field));

                editor.Widget.Location = new Point(FIELD_X_POS, verticalPosition);
                editor.Widget.TabIndex = tabIndex++;

                panelContent.Controls.Add(editor.Widget);
                verticalPosition += editor.VerticalSkip + MARGIN / 2;
                editors.Add(editor);
            }

            if (unsupportedFields.Count == 0) return;

            StringBuilder sb = new StringBuilder();
            sb.Append("Unsupported fields (existing values copied): ");
            foreach (var field in unsupportedFields) {
                sb.Append(field.Name).Append(", ");
            }
            textUnsupported = new TextBox
                              {
                                  Multiline = true,
                                  BorderStyle = BorderStyle.None,
                                  ReadOnly = true,
                                  WordWrap = true,
                                  Text = sb.ToString().Substring(0, sb.Length - 2),
                              };
        }

        private void addCommentField() {
            addLabel("Comment");

            textComment.Location = new Point(FIELD_X_POS, verticalPosition);
            textComment.Size = new Size(calculatedFieldWidth(), JiraFieldEditorProvider.MULTI_LINE_EDITOR_HEIGHT);
            textComment.TabIndex = tabIndex++;
            textComment.Facade = SmartJiraServerFacade.Instance;
            textComment.Issue = issue;

            verticalPosition += JiraFieldEditorProvider.MULTI_LINE_EDITOR_HEIGHT + MARGIN;

            panelContent.Controls.Add(textComment);
        }

        private void addLabel(string text) {
            Label l = new Label
                      {
                          AutoSize = false,
                          Location = new Point(LABEL_X_POS, verticalPosition + 3),
                          Size = new Size(FIELD_X_POS - LABEL_X_POS - MARGIN / 2, LABEL_HEIGHT),
                          TextAlign = ContentAlignment.TopRight,
                          Text = text
                      };

            panelContent.Controls.Add(l);
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            setAllEnabled(false);
            setThrobberVisible(true);

            ICollection<JiraField> updatedFields = mergeFieldsFromEditors();
            Thread t = PlvsUtils.createThread(delegate {
                                                  try {
                                                      SmartJiraServerFacade.Instance.runIssueActionWithParams(
                                                          issue, action, updatedFields, textComment.Text.Length > 0 ? textComment.Text : null);
                                                      var newIssue = SmartJiraServerFacade.Instance.getIssue(issue.Server, issue.Key);
                                                      UsageCollector.Instance.bumpJiraIssuesOpen();
                                                      status.setInfo("Action \"" + action.Name + "\" successfully run on issue " + issue.Key);
                                                      this.safeInvoke(new MethodInvoker(delegate {
                                                                                            Close();
                                                                                            model.updateIssue(newIssue);
                                                                                            if (onFinish != null) onFinish();
                                                                                        }));
                                                  }
                                                  catch (Exception ex) {
                                                      this.safeInvoke(new MethodInvoker(() => showErrorAndResumeEditing(ex)));
                                                  }
                                              });
            t.Start();
        }

        private void showErrorAndResumeEditing(Exception ex) {
            PlvsUtils.showError("Failed to run action " + action.Name + " on issue " + issue.Key, ex);
            setThrobberVisible(false);
            setAllEnabled(true);
        }

        private void setThrobberVisible(bool visible) {
            SuspendLayout();
            panelContent.SuspendLayout();
            panelThrobber.Visible = visible;
            panelContent.Visible = !visible;
            foreach (JiraFieldEditorProvider editor in editors) {
                editor.Widget.Visible = !visible;
            }
            textComment.Visible = !visible;
            if (textUnsupported != null) {
                textUnsupported.Visible = !visible;
            }
            panelContent.ResumeLayout(true);
            ResumeLayout(true);
        }

        private void setAllEnabled(bool enabled) {
            buttonCancel.Enabled = enabled;
            foreach (JiraFieldEditorProvider editor in editors) {
                editor.Widget.Enabled = enabled;
            }
            textComment.Enabled = enabled;
            if (enabled) {
                updateOkButton();
            } else {
                buttonOk.Enabled = false;
            }
        }

        private ICollection<JiraField> mergeFieldsFromEditors() {
            Dictionary<string, JiraField> map = new Dictionary<string, JiraField>();
            foreach (var field in fields) {
                map[field.Id] = field;
            }
            foreach (JiraFieldEditorProvider editor in editors) {
                editor.Field.Values = editor.getValues();
                map[editor.Field.Id] = editor.Field;
            }
            return map.Values;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void issueWorkflowActionResize(object sender, EventArgs e) {
            SuspendLayout();

            resizeStaticContent();

            int width = calculatedFieldWidth();

            textComment.Size = new Size(width, JiraFieldEditorProvider.MULTI_LINE_EDITOR_HEIGHT);
            foreach (JiraFieldEditorProvider editor in editors) {
                editor.resizeToWidth(width);
            }
            if (textUnsupported != null) {
                textUnsupported.Width = ClientSize.Width - 4*MARGIN;
            }

            ResumeLayout(true);
        }

        private int calculatedFieldWidth() {
            return ClientSize.Width - FIELD_X_POS - 4*MARGIN;
        }

        private void issueWorkflowActionKeyPress(object sender, KeyPressEventArgs e) {
            if (buttonCancel.Enabled && e.KeyChar == (char)Keys.Escape) {
                Close();
            }
        }
    }
}
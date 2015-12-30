using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.fields;
using Atlassian.plvs.util;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.dialogs.jira {
    public partial class FieldEditor : Form {
        private readonly JiraIssueListModel model;
        private readonly AbstractJiraServerFacade facade;
        private readonly JiraIssue issue;
        private readonly string fieldId;
        private JiraField field;

        private JiraFieldEditorProvider editorProvider;
        private Control editorControl;

        private const int MARGIN = 8;

        private int customWidth = -1;
        private int customHeight = -1;

        public FieldEditor(string title, JiraIssueListModel model, AbstractJiraServerFacade facade, JiraIssue issue, string fieldId, Point location) {
            PlvsLogger.log("FieldEditor - ctor");

            this.model = model;
            this.facade = facade;
            this.issue = issue;
            this.fieldId = fieldId;

            InitializeComponent();

            buttonOk.Enabled = false;
            buttonCancel.Enabled = false;

            StartPosition = FormStartPosition.Manual;
            Location = location;

            Text = title;

            field = new JiraField(fieldId, null);

            Thread t = PlvsUtils.createThread(fillFieldWrap);
            t.Start();
        }

        public override sealed string Text {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public void fieldValid(JiraFieldEditorProvider sender, bool valid) {
            Invoke(new MethodInvoker(delegate { buttonOk.Enabled = valid; }));
        }

        public void setCustomSize(Size size) {
            customWidth = size.Width;
            customHeight = size.Height;
        }

        private void fillFieldWrap() {
            try {
                getMetadata();
            } catch (Exception e) {
                this.safeInvoke(new MethodInvoker(delegate {
                                             PlvsUtils.showError("Unable to initialize field editor component", e);
                                             Close();
                                         }));
            }
        }

        private void getMetadata() {
            PlvsLogger.log("FieldEditor.getMetadata()");

            object rawIssueObject = facade.getRawIssueObject(issue);
            List<JiraField> filledFields = JiraActionFieldType.fillFieldValues(issue, rawIssueObject, new List<JiraField> { field });
            field = filledFields[0];
            field.setRawIssueObject(rawIssueObject);

            SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(issue.Server);
            if (!projects.ContainsKey(issue.ProjectKey)) return;

            JiraProject project = projects[issue.ProjectKey];

            // todo: these are not always necessary. refactor to make it more efficient and smart
            List<JiraNamedEntity> versions = facade.getVersions(issue.Server, project);
            versions.Reverse();
            List<JiraNamedEntity> comps = facade.getComponents(issue.Server, project);

            this.safeInvoke(new MethodInvoker(() => createEditorWidget(versions, comps, rawIssueObject)));
        }

        private void createEditorWidget(IEnumerable<JiraNamedEntity> versions, IEnumerable<JiraNamedEntity> comps, object rawIssueObject) {
            PlvsLogger.log("FieldEditor.createEditorWidget()");

            switch (JiraActionFieldType.getFieldTypeForFieldId(fieldId)) {
                case JiraActionFieldType.WidgetType.SUMMARY:
                    editorProvider = new TextLineFieldEditorProvider(field, issue.Summary, fieldValid);
                    break;
                case JiraActionFieldType.WidgetType.DESCRIPTION:
                    string descr = JiraIssueUtils.getRawIssueObjectPropertyValue<string>(rawIssueObject, "description") ?? "";
                    editorProvider = new TextAreaFieldEditorProvider(facade, issue, field, descr, fieldValid);
                    break;
                case JiraActionFieldType.WidgetType.ENVIRONMENT:
                    string env = JiraIssueUtils.getRawIssueObjectPropertyValue<string>(rawIssueObject, "environment") ?? "";
                    editorProvider = new TextAreaFieldEditorProvider(facade, issue, field, env, fieldValid);
                    break;
                case JiraActionFieldType.WidgetType.VERSIONS:
                    editorProvider = new NamedEntityListFieldEditorProvider(field, issue.Versions, versions, fieldValid);
                    break;
                case JiraActionFieldType.WidgetType.FIX_VERSIONS:
                    editorProvider = new NamedEntityListFieldEditorProvider(field, issue.FixVersions, versions, fieldValid);
                    break;
                case JiraActionFieldType.WidgetType.ASSIGNEE:
                    editorProvider = new UserFieldEditorProvider(issue.Server, field,
                                                         field.Values.IsNullOrEmpty() ? "" : field.Values[0], fieldValid, true);
                    break;
                case JiraActionFieldType.WidgetType.COMPONENTS:
                    editorProvider = new NamedEntityListFieldEditorProvider(field, issue.Components, comps, fieldValid);
                    break;
                case JiraActionFieldType.WidgetType.PRIORITY:
                    editorProvider = new NamedEntityComboEditorProvider(issue.Server, field, issue.PriorityId,
                                                                JiraServerCache.Instance.getPriorities(issue.Server),
                                                                fieldValid);
                    break;
                case JiraActionFieldType.WidgetType.TIMETRACKING:
                    List<JiraField> fields = JiraActionFieldType.fillFieldValues(issue, rawIssueObject, new List<JiraField> {field});
                    editorProvider = new TimeTrackingEditorProvider(field, fields[0].Values[0], fieldValid);
                    break;
                default:
                    MessageBox.Show("Unsupported field type selected for editing",
                                    Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    break;
            }

            buttonCancel.Enabled = true;
            buttonOk.Enabled = editorProvider != null ? editorProvider.FieldValid : false;
            if (editorProvider != null) {
                addEditorWidget(editorProvider);
            }
        }

        private void addEditorWidget(JiraFieldEditorProvider editor) {
            PlvsLogger.log("FieldEditor.addEditorWidget()");

            Controls.Remove(labelInfo);
            editorControl = editor.Widget;
            editorControl.Location = new Point(MARGIN, MARGIN);
            Controls.Add(editorControl);
            editor.resizeToWidth(Width);

            ClientSize = new Size(editorControl.Width + 2 * MARGIN, editorControl.Height + 2 * MARGIN + (ClientSize.Height - buttonOk.Location.Y));

            Resize += fieldEditorResize;

            if (customWidth != -1 && customHeight != -1) {
                Size = new Size(customWidth, customHeight);
            }
        }

        private void fieldEditorKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Escape && buttonCancel.Enabled) {
                Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            if (!editorProvider.FieldValid) {
                MessageBox.Show("Invalid value", Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            labelInfo.Text = "Applying changes...";
            buttonOk.Enabled = false;
            buttonCancel.Enabled = false;
            
            Controls.Remove(editorControl);
            Controls.Add(labelInfo);

            List<string> values = editorProvider.getValues();
            field.Values = values;

            Thread t = PlvsUtils.createThread(applyChanges);
            t.Start();
        }

        private void applyChanges() {
            try {
                facade.updateIssue(issue, new List<JiraField> {field});
                JiraIssue updatedIssue = facade.getIssue(issue.Server, issue.Key);
                this.safeInvoke(new MethodInvoker(delegate {
                                             Close();
                                             model.updateIssue(updatedIssue);
                                         }));
            } catch (Exception e) {
                this.safeInvoke(new MethodInvoker(delegate {
                                             PlvsUtils.showError("Failed to apply changes", e);
                                             Close();
                                         }));
            }
        }

        private void fieldEditorResize(object sender, EventArgs e) {
            if (editorProvider == null) {
                return;
            }
            editorProvider.resizeToWidth(getWidgetWidth());
            editorProvider.resizeToHeight(getWidgetHeight());
        }

        private int getWidgetHeight() {
            return ClientSize.Height - buttonOk.Height - 3 * MARGIN;
        }

        private int getWidgetWidth() {
            return ClientSize.Width - 2 * MARGIN;
        }
    }
}

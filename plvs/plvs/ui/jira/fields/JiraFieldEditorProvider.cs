using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.fields {
    public abstract class JiraFieldEditorProvider {
        public JiraField Field { get; private set; }

        private readonly FieldValidListener validListener;
        private bool fieldValid = true;

        public static int SINGLE_LINE_EDITOR_HEIGHT = 16;
        public static int MULTI_LINE_EDITOR_HEIGHT = 100;

        public abstract Control Widget { get; }
        public abstract int VerticalSkip { get; }
        public abstract void resizeToWidth(int width);
        public abstract void resizeToHeight(int height);

        public delegate void FieldValidListener(JiraFieldEditorProvider sender, bool valid);

        protected JiraFieldEditorProvider(JiraField field, FieldValidListener validListener) {
            Field = field;
            this.validListener = validListener;
        }

        public bool FieldValid {
            get { return fieldValid; }
            protected set {
                fieldValid = value;
                if (validListener != null) {
                    validListener(this, value);
                }
            }
        }

        public virtual string getFieldLabel(JiraIssue issue, JiraField field) {
            return field.Name;
        }

        public abstract List<string> getValues();
    }
}
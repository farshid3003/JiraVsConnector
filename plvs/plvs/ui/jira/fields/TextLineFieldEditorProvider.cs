using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.fields {
    public class TextLineFieldEditorProvider : FixedHeightFieldEditorProvider {
        private readonly Control editor = new TextBox();

        public TextLineFieldEditorProvider(JiraField field, string value, FieldValidListener validListener)
            : base(field, validListener) {
            if (value != null) {
                editor.Text = value;
            }
        }

        public override Control Widget {
            get { return editor; }
        }

        public override int VerticalSkip {
            get { return editor.Height; }
        }

        public override void resizeToWidth(int width) {
            editor.Width = width;
        }

        public override List<string> getValues() {
            return new List<string> { editor.Text };
        }
    }
}
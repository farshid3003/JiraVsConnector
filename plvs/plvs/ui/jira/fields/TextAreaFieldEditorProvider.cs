using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;

namespace Atlassian.plvs.ui.jira.fields {
    public class TextAreaFieldEditorProvider : JiraFieldEditorProvider {
        private readonly JiraTextAreaWithWikiPreview editor = new JiraTextAreaWithWikiPreview
                                          {
                                              Height = MULTI_LINE_EDITOR_HEIGHT
                                          };

        public TextAreaFieldEditorProvider(
            AbstractJiraServerFacade facade, JiraIssue issue, JiraField field, string value, FieldValidListener validListener)
            : base(field, validListener) {

            editor.Issue = issue;
            editor.Facade = facade;

            if (value == null) return;

            string fixedValue = value.Replace("\r\n", "\n").Replace("\n", "\r\n");
            editor.Text = fixedValue;
        }

        public override Control Widget {
            get { return editor; }
        }

        public override int VerticalSkip {
            get { return MULTI_LINE_EDITOR_HEIGHT; }
        }

        public override void resizeToWidth(int width) {
            editor.Width = width;
        }

        public override void resizeToHeight(int height) {
            editor.Height = height;
        }

        public override List<string> getValues() {
            return new List<string> {editor.Text.Replace("\r\n", "\n")};
        }
    }
}
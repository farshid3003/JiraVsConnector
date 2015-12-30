using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.fields {
    public abstract class FixedHeightFieldEditorProvider : JiraFieldEditorProvider {
        protected FixedHeightFieldEditorProvider(JiraField field, FieldValidListener validListener) : base(field, validListener) {}

        public override void resizeToHeight(int height) {
        }
    }
}

using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.fields {
    public class UserFieldEditorProvider : FixedHeightFieldEditorProvider {

        private JiraUserPicker picker;

        public UserFieldEditorProvider(JiraServer server, JiraField field, string userName, FieldValidListener validListener) 
            : base(field, validListener) {

            init(server, userName, false);
        }

        public UserFieldEditorProvider(JiraServer server, JiraField field, string userName, FieldValidListener validListener, bool showAssignToMe) 
            : base(field, validListener) {

            init(server, userName, showAssignToMe);

        }

        private void init(JiraServer server, string userName, bool showAssignToMe) {
            picker = new JiraUserPicker();
            picker.init(server, userName, showAssignToMe);
        }

        public override Control Widget {
            get { return picker; }
        }

        public override int VerticalSkip {
            get { return picker.Height; }
        }

        public override void resizeToWidth(int width) {}

        public override List<string> getValues() {
            return new List<string> { picker.Value };
        }
    }
}
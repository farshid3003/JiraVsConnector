using Atlassian.plvs.models.jira;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.jira {
    class UserTypeComboBoxItem {
        public JiraCustomFilter.UserType Type { get; private set; }

        public UserTypeComboBoxItem(JiraCustomFilter.UserType type) {
            Type = type;
        }

        public override string ToString() {
            return Type.GetStringValue();
        }
    }
}
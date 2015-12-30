using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.dialogs.jira {
    public partial class ActionSelector : Form {
        public ActionSelector(IEnumerable<JiraNamedEntity> actions) {
            InitializeComponent();

            foreach (var action in actions) {
                comboActions.Items.Add(action);
            }
            if (comboActions.Items.Count > 0) {
                comboActions.SelectedIndex = 0;
            }
        }

        public JiraNamedEntity SelectedAction {
            get { return comboActions.SelectedItem as JiraNamedEntity; }
        }

        private void buttonCancelClick(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOkClick(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

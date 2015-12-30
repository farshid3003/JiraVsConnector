using System;
using System.Windows.Forms;

namespace Atlassian.plvs.dialogs.jira {
    public partial class SetEndTime : Form {

        public SetEndTime(DateTime time) {

            InitializeComponent();

            DateTime = time;

            StartPosition = FormStartPosition.CenterParent;
        }

        public DateTime DateTime { 
            get {
                DateTime dt = dateEnd.Value.Date.AddHours((double) numericHours.Value).AddMinutes((double) numericMinutes.Value);
                return dt;
            }

            private set {
                dateEnd.Value = value;
                numericHours.Value = value.Hour;
                numericMinutes.Value = value.Minute;
            } 
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void setEndTimeKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Escape) {
                Close();
            }
        }
    }
}
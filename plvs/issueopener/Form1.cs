using System.Collections.Generic;
using System.Windows.Forms;

namespace issueopener {
    public partial class Form1 : Form {
        public Form1(IEnumerable<string> args) {
            InitializeComponent();

            foreach (string s in args) {
                textxxx.Text = textxxx.Text + "\r\n" + s;
            }
        }
    }
}

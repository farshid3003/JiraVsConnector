using System;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Forms.UserControl;


namespace Atlassian.plvs.ui {
    public class TextBoxWithSpellChecker : UserControl {

        public TextBox TextBox { get; private set; }
        private readonly ElementHost host;

        public override string Text {
            get { return TextBox.Text; }
            set { TextBox.Text = value; }
        }

        protected override void OnEnabledChanged(EventArgs e) {
            host.Enabled = Enabled;
            TextBox.IsEnabled = Enabled;
        }

        private bool multiline;

        public bool IsMultiline {
            get { return multiline; }
            set {
                multiline = value;
                TextBox.AcceptsReturn = value;
                TextBox.VerticalScrollBarVisibility = value ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
                host.Dock = value ? DockStyle.Fill : DockStyle.Top;
            }
        }

        public TextBoxWithSpellChecker() {
            TextBox = new TextBox();
            TextBox.SpellCheck.IsEnabled = true;
            TextBox.Language = XmlLanguage.GetLanguage("en-us");

            host = new ElementHost {Child = TextBox, Height = 20};

            IsMultiline = false;

            Controls.Add(host);
        }
    }
}

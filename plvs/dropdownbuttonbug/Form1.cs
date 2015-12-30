using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace dropdownbuttonbug {
    public class Form1 : Form {
        private IContainer components;
        private ToolStrip toolStrip1;
        private ToolStripContainer toolStripContainer1;
        private ToolStripDropDownButton toolStripDropDown;

        public Form1() {
            InitializeComponent();
            toolStripDropDown.DropDownItems.Add("dummy");
        }

        private void toolStripDropDown_DropDownOpened(object sender, EventArgs e) {
            toolStripDropDown.DropDownItems.Clear();
            var toolStripMenuItem = new ToolStripMenuItem("test");
            toolStripDropDown.DropDownItems.Add(toolStripMenuItem);

#if WORKAROUND

            var phoney = new ToolStripSeparator();
            toolStripDropDown.DropDownItems.Add(phoney);

            var t = new Timer {Interval = 1};
            t.Tick += (a, b) => {
                          t.Stop();
                          toolStripDropDown.DropDownItems.Remove(phoney);
                      };
            t.Start();
#endif
        }

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            toolStripContainer1 = new ToolStripContainer();
            toolStrip1 = new ToolStrip();
            toolStripDropDown = new ToolStripDropDownButton();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();

            toolStripContainer1.ContentPanel.Size = new Size(813, 237);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new Point(0, 0);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(813, 262);
            toolStripContainer1.TabIndex = 0;
            toolStripContainer1.Text = "toolStripContainer1";
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);
            toolStrip1.Dock = DockStyle.None;
            toolStrip1.Items.AddRange(new ToolStripItem[]
                                      {
                                          toolStripDropDown
                                      });
            toolStrip1.Location = new Point(3, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(207, 25);
            toolStrip1.TabIndex = 0;
            toolStripDropDown.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDown.ImageTransparentColor = Color.Magenta;
            toolStripDropDown.Name = "toolStripDropDown";
            toolStripDropDown.Size = new Size(164, 22);
            toolStripDropDown.Text = "toolStripDropDownButton1";
            toolStripDropDown.DropDownOpened += toolStripDropDown_DropDownOpened;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(813, 262);
            Controls.Add(toolStripContainer1);
            Name = "Form1";
            Text = "Form1";
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
        }
    }
}
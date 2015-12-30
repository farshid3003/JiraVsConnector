using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui {
    public class StatusLabel {
        private readonly StatusStrip statusBar;
        private readonly ToolStripStatusLabel targetLabel;

        public StatusLabel(StatusStrip statusBar, ToolStripStatusLabel targetLabel) {
            this.statusBar = statusBar;
            this.targetLabel = targetLabel;
        }

        private ICollection<Exception> lastExceptions;

        public void setError(string txt, Exception e) {
            setError(txt, new List<Exception> {e});
        }

        public bool HaveErrors {
            get { return lastExceptions != null; }
        }

        public void setError(string txt, ICollection<Exception> exceptions) {
            statusBar.safeInvoke(new MethodInvoker(delegate {
                                                       targetLabel.BackColor = Color.LightPink;
                                                       statusBar.BackColor = Color.LightPink;
                                                       targetLabel.Text = txt;
                                                       lastExceptions = exceptions;
                                                       targetLabel.Visible = true;
                                                       targetLabel.IsLink = true;
                                                       targetLabel.Click += targetLabel_Click;
                                                       targetLabel.ImageAlign = ContentAlignment.MiddleRight;
                                                       targetLabel.Image = SystemIcons.Error.ToBitmap();
                                                       targetLabel.TextAlign = ContentAlignment.MiddleRight;
                                                   }));
        }

        private void targetLabel_Click(object sender, EventArgs e) {
            if (lastExceptions == null || lastExceptions.Count == 0) {
                return;
            }
            PlvsUtils.showErrors(null, lastExceptions);

            lastExceptions = null;
            targetLabel.BackColor = SystemColors.Control;
            statusBar.BackColor = SystemColors.Control;
            targetLabel.Text = "";
            targetLabel.IsLink = false;
            targetLabel.Image = null;
            targetLabel.TextAlign = ContentAlignment.MiddleRight;
        }

        public void setInfo(string txt) {
            statusBar.safeInvoke(new MethodInvoker(delegate {
                                                       lastExceptions = null;
                                                       targetLabel.BackColor = SystemColors.Control;
                                                       statusBar.BackColor = SystemColors.Control;
                                                       targetLabel.Text = txt;
                                                       targetLabel.Visible = true;
                                                       targetLabel.IsLink = false;
                                                       targetLabel.Image = null;
                                                       targetLabel.TextAlign = ContentAlignment.MiddleRight;
                                                   }));
        }
    }
}
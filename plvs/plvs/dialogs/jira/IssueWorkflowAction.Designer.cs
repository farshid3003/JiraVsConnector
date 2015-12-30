namespace Atlassian.plvs.dialogs.jira {
    partial class IssueWorkflowAction {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IssueWorkflowAction));
            this.buttonOk = new System.Windows.Forms.Button();
            this.panelContent = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.progressWait = new System.Windows.Forms.ProgressBar();
            this.panelThrobber = new System.Windows.Forms.Panel();
            this.labelWait = new System.Windows.Forms.Label();
            this.panelThrobber.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(516, 627);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // panelContent
            // 
            this.panelContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContent.AutoScroll = true;
            this.panelContent.Location = new System.Drawing.Point(12, 12);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(660, 609);
            this.panelContent.TabIndex = 1;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(597, 627);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // progressWait
            // 
            this.progressWait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressWait.Location = new System.Drawing.Point(190, 125);
            this.progressWait.Name = "progressWait";
            this.progressWait.Size = new System.Drawing.Size(304, 23);
            this.progressWait.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressWait.TabIndex = 0;
            // 
            // panelThrobber
            // 
            this.panelThrobber.BackColor = System.Drawing.SystemColors.Control;
            this.panelThrobber.Controls.Add(this.labelWait);
            this.panelThrobber.Controls.Add(this.progressWait);
            this.panelThrobber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelThrobber.Location = new System.Drawing.Point(0, 0);
            this.panelThrobber.Name = "panelThrobber";
            this.panelThrobber.Size = new System.Drawing.Size(684, 662);
            this.panelThrobber.TabIndex = 1;
            // 
            // labelWait
            // 
            this.labelWait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelWait.Location = new System.Drawing.Point(12, 194);
            this.labelWait.Name = "labelWait";
            this.labelWait.Size = new System.Drawing.Size(660, 13);
            this.labelWait.TabIndex = 1;
            this.labelWait.Text = "Please Wait...";
            this.labelWait.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // IssueWorkflowAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 662);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.panelThrobber);
            this.Controls.Add(this.panelContent);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "IssueWorkflowAction";
            this.Text = "IssueWorkflowAction";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.issueWorkflowActionKeyPress);
            this.Resize += new System.EventHandler(this.issueWorkflowActionResize);
            this.panelThrobber.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panelThrobber;
        private System.Windows.Forms.Label labelWait;
        private System.Windows.Forms.ProgressBar progressWait;
    }
}
namespace Atlassian.plvs.dialogs {
    partial class AbstractTestConnection
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AbstractTestConnection));
            this.progress = new System.Windows.Forms.ProgressBar();
            this.buttonClose = new System.Windows.Forms.Button();
            this.linkErrorDetails = new System.Windows.Forms.LinkLabel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(59, 12);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(290, 23);
            this.progress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progress.TabIndex = 0;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonClose.Location = new System.Drawing.Point(176, 60);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(57, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // linkErrorDetails
            // 
            this.linkErrorDetails.BackColor = System.Drawing.Color.Transparent;
            this.linkErrorDetails.Location = new System.Drawing.Point(6, 9);
            this.linkErrorDetails.Name = "linkErrorDetails";
            this.linkErrorDetails.Size = new System.Drawing.Size(391, 26);
            this.linkErrorDetails.TabIndex = 3;
            this.linkErrorDetails.TabStop = true;
            this.linkErrorDetails.Text = "Click here for details";
            this.linkErrorDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkErrorDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkErrorDetails_LinkClicked);
            // 
            // labelStatus
            // 
            this.labelStatus.BackColor = System.Drawing.SystemColors.Control;
            this.labelStatus.Location = new System.Drawing.Point(6, 40);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(391, 17);
            this.labelStatus.TabIndex = 4;
            this.labelStatus.Text = "Status Message";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AbstractTestConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 95);
            this.ControlBox = false;
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.linkErrorDetails);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.progress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AbstractTestConnection";
            this.Text = "Test Connection to Server";
            this.Load += new System.EventHandler(this.abstractTestConnectionLoad);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.testJiraConnectionKeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.LinkLabel linkErrorDetails;
        private System.Windows.Forms.Label labelStatus;
    }
}
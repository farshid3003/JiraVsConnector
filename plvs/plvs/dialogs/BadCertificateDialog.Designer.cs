namespace Atlassian.plvs.dialogs {
    partial class BadCertificateDialog {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BadCertificateDialog));
            this.buttonYes = new System.Windows.Forms.Button();
            this.buttonNo = new System.Windows.Forms.Button();
            this.labelWarning1 = new System.Windows.Forms.Label();
            this.linkCertificateDetails = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelSubject = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelIssuer = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelValidFrom = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labelValidUntil = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelWarning3 = new System.Windows.Forms.Label();
            this.labelWarning2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonYes
            // 
            this.buttonYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.buttonYes.Location = new System.Drawing.Point(316, 207);
            this.buttonYes.Name = "buttonYes";
            this.buttonYes.Size = new System.Drawing.Size(75, 23);
            this.buttonYes.TabIndex = 0;
            this.buttonYes.Text = "Yes";
            this.buttonYes.UseVisualStyleBackColor = true;
            // 
            // buttonNo
            // 
            this.buttonNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.buttonNo.Location = new System.Drawing.Point(397, 207);
            this.buttonNo.Name = "buttonNo";
            this.buttonNo.Size = new System.Drawing.Size(75, 23);
            this.buttonNo.TabIndex = 1;
            this.buttonNo.Text = "No";
            this.buttonNo.UseVisualStyleBackColor = true;
            // 
            // labelWarning1
            // 
            this.labelWarning1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelWarning1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelWarning1.ForeColor = System.Drawing.Color.Red;
            this.labelWarning1.Location = new System.Drawing.Point(13, 9);
            this.labelWarning1.Name = "labelWarning1";
            this.labelWarning1.Size = new System.Drawing.Size(460, 15);
            this.labelWarning1.TabIndex = 2;
            this.labelWarning1.Text = "Warning";
            this.labelWarning1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkCertificateDetails
            // 
            this.linkCertificateDetails.AutoSize = true;
            this.linkCertificateDetails.Location = new System.Drawing.Point(12, 180);
            this.linkCertificateDetails.Name = "linkCertificateDetails";
            this.linkCertificateDetails.Size = new System.Drawing.Size(89, 13);
            this.linkCertificateDetails.TabIndex = 3;
            this.linkCertificateDetails.TabStop = true;
            this.linkCertificateDetails.Text = "Certificate Details";
            this.linkCertificateDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkCertificateDetails_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Subject";
            // 
            // labelSubject
            // 
            this.labelSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSubject.Location = new System.Drawing.Point(129, 61);
            this.labelSubject.Name = "labelSubject";
            this.labelSubject.Size = new System.Drawing.Size(343, 13);
            this.labelSubject.TabIndex = 5;
            this.labelSubject.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Issuer";
            // 
            // labelIssuer
            // 
            this.labelIssuer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelIssuer.Location = new System.Drawing.Point(129, 78);
            this.labelIssuer.Name = "labelIssuer";
            this.labelIssuer.Size = new System.Drawing.Size(343, 13);
            this.labelIssuer.TabIndex = 7;
            this.labelIssuer.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Valid Before";
            // 
            // labelValidFrom
            // 
            this.labelValidFrom.AutoSize = true;
            this.labelValidFrom.Location = new System.Drawing.Point(129, 95);
            this.labelValidFrom.Name = "labelValidFrom";
            this.labelValidFrom.Size = new System.Drawing.Size(35, 13);
            this.labelValidFrom.TabIndex = 9;
            this.labelValidFrom.Text = "label7";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 112);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Valid Until";
            // 
            // labelValidUntil
            // 
            this.labelValidUntil.AutoSize = true;
            this.labelValidUntil.Location = new System.Drawing.Point(129, 112);
            this.labelValidUntil.Name = "labelValidUntil";
            this.labelValidUntil.Size = new System.Drawing.Size(35, 13);
            this.labelValidUntil.TabIndex = 11;
            this.labelValidUntil.Text = "label9";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(13, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(459, 42);
            this.label3.TabIndex = 12;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // labelWarning3
            // 
            this.labelWarning3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelWarning3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelWarning3.ForeColor = System.Drawing.Color.Red;
            this.labelWarning3.Location = new System.Drawing.Point(13, 40);
            this.labelWarning3.Name = "labelWarning3";
            this.labelWarning3.Size = new System.Drawing.Size(460, 17);
            this.labelWarning3.TabIndex = 13;
            this.labelWarning3.Text = "Warning";
            this.labelWarning3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelWarning2
            // 
            this.labelWarning2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelWarning2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelWarning2.ForeColor = System.Drawing.Color.Red;
            this.labelWarning2.Location = new System.Drawing.Point(13, 24);
            this.labelWarning2.Name = "labelWarning2";
            this.labelWarning2.Size = new System.Drawing.Size(460, 16);
            this.labelWarning2.TabIndex = 14;
            this.labelWarning2.Text = "Warning";
            this.labelWarning2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BadCertificateDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonNo;
            this.ClientSize = new System.Drawing.Size(484, 242);
            this.Controls.Add(this.labelWarning2);
            this.Controls.Add(this.labelWarning3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelValidUntil);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.labelValidFrom);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelIssuer);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelSubject);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkCertificateDetails);
            this.Controls.Add(this.labelWarning1);
            this.Controls.Add(this.buttonNo);
            this.Controls.Add(this.buttonYes);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(10000, 280);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 280);
            this.Name = "BadCertificateDialog";
            this.Text = "Unknown SSL Certificate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonYes;
        private System.Windows.Forms.Button buttonNo;
        private System.Windows.Forms.Label labelWarning1;
        private System.Windows.Forms.LinkLabel linkCertificateDetails;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelSubject;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelIssuer;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelValidFrom;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelValidUntil;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelWarning3;
        private System.Windows.Forms.Label labelWarning2;
    }
}
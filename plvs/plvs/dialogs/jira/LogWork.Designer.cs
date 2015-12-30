namespace Atlassian.plvs.dialogs.jira {
    partial class LogWork {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogWork));
            this.buttonOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textTimeSpent = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.radioAutoUpdate = new System.Windows.Forms.RadioButton();
            this.radioLeaveUnchanged = new System.Windows.Forms.RadioButton();
            this.radioUpdateManually = new System.Windows.Forms.RadioButton();
            this.textRemainingEstimate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelEndTime = new System.Windows.Forms.Label();
            this.buttonChange = new System.Windows.Forms.Button();
            this.textExplanation = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.logWorkPanel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.textComment = new System.Windows.Forms.TextBox();
            this.logWorkPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(370, 277);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(89, 23);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "Add Worklog";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Time Spent";
            // 
            // textTimeSpent
            // 
            this.textTimeSpent.Location = new System.Drawing.Point(82, 14);
            this.textTimeSpent.Name = "textTimeSpent";
            this.textTimeSpent.Size = new System.Drawing.Size(148, 20);
            this.textTimeSpent.TabIndex = 0;
            this.textTimeSpent.TextChanged += new System.EventHandler(this.textTimeSpent_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(278, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Remaining Estimate";
            // 
            // radioAutoUpdate
            // 
            this.radioAutoUpdate.AutoSize = true;
            this.radioAutoUpdate.Location = new System.Drawing.Point(293, 40);
            this.radioAutoUpdate.Name = "radioAutoUpdate";
            this.radioAutoUpdate.Size = new System.Drawing.Size(85, 17);
            this.radioAutoUpdate.TabIndex = 1;
            this.radioAutoUpdate.TabStop = true;
            this.radioAutoUpdate.Text = "Auto Update";
            this.radioAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // radioLeaveUnchanged
            // 
            this.radioLeaveUnchanged.AutoSize = true;
            this.radioLeaveUnchanged.Location = new System.Drawing.Point(293, 63);
            this.radioLeaveUnchanged.Name = "radioLeaveUnchanged";
            this.radioLeaveUnchanged.Size = new System.Drawing.Size(114, 17);
            this.radioLeaveUnchanged.TabIndex = 2;
            this.radioLeaveUnchanged.TabStop = true;
            this.radioLeaveUnchanged.Text = "Leave Unchanged";
            this.radioLeaveUnchanged.UseVisualStyleBackColor = true;
            // 
            // radioUpdateManually
            // 
            this.radioUpdateManually.AutoSize = true;
            this.radioUpdateManually.Location = new System.Drawing.Point(293, 86);
            this.radioUpdateManually.Name = "radioUpdateManually";
            this.radioUpdateManually.Size = new System.Drawing.Size(105, 17);
            this.radioUpdateManually.TabIndex = 3;
            this.radioUpdateManually.TabStop = true;
            this.radioUpdateManually.Text = "Update Manually";
            this.radioUpdateManually.UseVisualStyleBackColor = true;
            this.radioUpdateManually.CheckedChanged += new System.EventHandler(this.radioUpdateManually_CheckedChanged);
            // 
            // textRemainingEstimate
            // 
            this.textRemainingEstimate.Location = new System.Drawing.Point(404, 85);
            this.textRemainingEstimate.Name = "textRemainingEstimate";
            this.textRemainingEstimate.Size = new System.Drawing.Size(148, 20);
            this.textRemainingEstimate.TabIndex = 4;
            this.textRemainingEstimate.TextChanged += new System.EventHandler(this.textRemainingEstimate_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "When";
            // 
            // labelEndTime
            // 
            this.labelEndTime.Location = new System.Drawing.Point(79, 142);
            this.labelEndTime.Name = "labelEndTime";
            this.labelEndTime.Size = new System.Drawing.Size(139, 13);
            this.labelEndTime.TabIndex = 10;
            this.labelEndTime.Text = "16/06/2010 11:33 PM";
            // 
            // buttonChange
            // 
            this.buttonChange.Location = new System.Drawing.Point(216, 137);
            this.buttonChange.Name = "buttonChange";
            this.buttonChange.Size = new System.Drawing.Size(75, 23);
            this.buttonChange.TabIndex = 5;
            this.buttonChange.Text = "Change";
            this.buttonChange.UseVisualStyleBackColor = true;
            this.buttonChange.Click += new System.EventHandler(this.buttonChange_Click);
            // 
            // textExplanation
            // 
            this.textExplanation.BackColor = System.Drawing.SystemColors.Control;
            this.textExplanation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textExplanation.Location = new System.Drawing.Point(42, 40);
            this.textExplanation.Multiline = true;
            this.textExplanation.Name = "textExplanation";
            this.textExplanation.ReadOnly = true;
            this.textExplanation.Size = new System.Drawing.Size(227, 70);
            this.textExplanation.TabIndex = 12;
            this.textExplanation.TabStop = false;
            this.textExplanation.Text = resources.GetString("textExplanation.Text");
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(465, 277);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(89, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // logWorkPanel
            // 
            this.logWorkPanel.BackColor = System.Drawing.SystemColors.Control;
            this.logWorkPanel.Controls.Add(this.label4);
            this.logWorkPanel.Controls.Add(this.textComment);
            this.logWorkPanel.Controls.Add(this.textExplanation);
            this.logWorkPanel.Controls.Add(this.label1);
            this.logWorkPanel.Controls.Add(this.textTimeSpent);
            this.logWorkPanel.Controls.Add(this.buttonChange);
            this.logWorkPanel.Controls.Add(this.label2);
            this.logWorkPanel.Controls.Add(this.labelEndTime);
            this.logWorkPanel.Controls.Add(this.radioAutoUpdate);
            this.logWorkPanel.Controls.Add(this.label3);
            this.logWorkPanel.Controls.Add(this.radioLeaveUnchanged);
            this.logWorkPanel.Controls.Add(this.textRemainingEstimate);
            this.logWorkPanel.Controls.Add(this.radioUpdateManually);
            this.logWorkPanel.Location = new System.Drawing.Point(0, 0);
            this.logWorkPanel.Name = "logWorkPanel";
            this.logWorkPanel.Size = new System.Drawing.Size(566, 276);
            this.logWorkPanel.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Comment";
            // 
            // textComment
            // 
            this.textComment.AcceptsReturn = true;
            this.textComment.Location = new System.Drawing.Point(82, 171);
            this.textComment.Multiline = true;
            this.textComment.Name = "textComment";
            this.textComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textComment.Size = new System.Drawing.Size(470, 91);
            this.textComment.TabIndex = 13;
            // 
            // LogWork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 312);
            this.Controls.Add(this.logWorkPanel);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogWork";
            this.Text = "LogWork";
            this.Load += new System.EventHandler(this.logWorkLoad);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.logWorkKeyPress);
            this.logWorkPanel.ResumeLayout(false);
            this.logWorkPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textTimeSpent;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioAutoUpdate;
        private System.Windows.Forms.RadioButton radioLeaveUnchanged;
        private System.Windows.Forms.RadioButton radioUpdateManually;
        private System.Windows.Forms.TextBox textRemainingEstimate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelEndTime;
        private System.Windows.Forms.Button buttonChange;
        private System.Windows.Forms.TextBox textExplanation;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel logWorkPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textComment;
    }
}
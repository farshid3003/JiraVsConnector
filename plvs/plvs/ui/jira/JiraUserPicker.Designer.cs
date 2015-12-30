namespace Atlassian.plvs.ui.jira {
    partial class JiraUserPicker {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboUsers = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkAssignToMe = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // comboUsers
            // 
            this.comboUsers.FormattingEnabled = true;
            this.comboUsers.Location = new System.Drawing.Point(4, 4);
            this.comboUsers.Name = "comboUsers";
            this.comboUsers.Size = new System.Drawing.Size(229, 21);
            this.comboUsers.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(239, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(210, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select user from list or type user login name";
            // 
            // checkAssignToMe
            // 
            this.checkAssignToMe.AutoSize = true;
            this.checkAssignToMe.Location = new System.Drawing.Point(4, 31);
            this.checkAssignToMe.Name = "checkAssignToMe";
            this.checkAssignToMe.Size = new System.Drawing.Size(91, 17);
            this.checkAssignToMe.TabIndex = 2;
            this.checkAssignToMe.Text = "Assign To Me";
            this.checkAssignToMe.UseVisualStyleBackColor = true;
            this.checkAssignToMe.CheckedChanged += new System.EventHandler(this.checkAssignToMe_CheckedChanged);
            // 
            // JiraUserPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkAssignToMe);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboUsers);
            this.Name = "JiraUserPicker";
            this.Size = new System.Drawing.Size(458, 54);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboUsers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkAssignToMe;
    }
}

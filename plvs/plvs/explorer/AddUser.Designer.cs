namespace Atlassian.plvs.explorer {
    partial class AddUser {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddUser));
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textUserId = new System.Windows.Forms.TextBox();
            this.checkOpenDropZone = new System.Windows.Forms.CheckBox();
            this.labelUserExists = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(214, 75);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "Add";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(295, 75);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Provide User Login Name";
            // 
            // textUserId
            // 
            this.textUserId.Location = new System.Drawing.Point(147, 9);
            this.textUserId.Name = "textUserId";
            this.textUserId.Size = new System.Drawing.Size(223, 20);
            this.textUserId.TabIndex = 0;
            this.textUserId.TextChanged += new System.EventHandler(this.textUserId_TextChanged);
            // 
            // checkOpenDropZone
            // 
            this.checkOpenDropZone.AutoSize = true;
            this.checkOpenDropZone.Location = new System.Drawing.Point(147, 52);
            this.checkOpenDropZone.Name = "checkOpenDropZone";
            this.checkOpenDropZone.Size = new System.Drawing.Size(223, 17);
            this.checkOpenDropZone.TabIndex = 1;
            this.checkOpenDropZone.Text = "Open \"Assign to\" Drop Zone After Adding";
            this.checkOpenDropZone.UseVisualStyleBackColor = true;
            // 
            // labelUserExists
            // 
            this.labelUserExists.AutoSize = true;
            this.labelUserExists.ForeColor = System.Drawing.Color.Red;
            this.labelUserExists.Location = new System.Drawing.Point(144, 32);
            this.labelUserExists.Name = "labelUserExists";
            this.labelUserExists.Size = new System.Drawing.Size(87, 13);
            this.labelUserExists.TabIndex = 4;
            this.labelUserExists.Text = "Error: User Exists";
            // 
            // AddUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 110);
            this.ControlBox = false;
            this.Controls.Add(this.labelUserExists);
            this.Controls.Add(this.checkOpenDropZone);
            this.Controls.Add(this.textUserId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "AddUser";
            this.Text = "Add User";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AddUser_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textUserId;
        private System.Windows.Forms.CheckBox checkOpenDropZone;
        private System.Windows.Forms.Label labelUserExists;
    }
}
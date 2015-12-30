namespace Atlassian.plvs.explorer {
    sealed partial class DropZone {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DropZone));
            this.labelInfo = new System.Windows.Forms.Label();
            this.textHistory = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelInfo
            // 
            this.labelInfo.Location = new System.Drawing.Point(12, 9);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(267, 190);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "Drop zone is idle";
            this.labelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textHistory
            // 
            this.textHistory.BackColor = System.Drawing.Color.White;
            this.textHistory.Location = new System.Drawing.Point(12, 226);
            this.textHistory.Multiline = true;
            this.textHistory.Name = "textHistory";
            this.textHistory.ReadOnly = true;
            this.textHistory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textHistory.Size = new System.Drawing.Size(267, 68);
            this.textHistory.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 210);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "History";
            // 
            // DropZone
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(291, 306);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textHistory);
            this.Controls.Add(this.labelInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DropZone";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DropZone";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DropZone_Load);
            this.DragLeave += new System.EventHandler(this.DropZone_DragLeave);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DropZone_DragDrop);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DropZone_FormClosed);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DropZone_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.TextBox textHistory;
        private System.Windows.Forms.Label label1;
    }
}
namespace Atlassian.plvs.ui.crucible {
    partial class TabCrucible {
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
            this.buttonShowChanges = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonShowChanges
            // 
            this.buttonShowChanges.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonShowChanges.Location = new System.Drawing.Point(257, 144);
            this.buttonShowChanges.Name = "buttonShowChanges";
            this.buttonShowChanges.Size = new System.Drawing.Size(222, 23);
            this.buttonShowChanges.TabIndex = 0;
            this.buttonShowChanges.Text = "Show Changed Files";
            this.buttonShowChanges.UseVisualStyleBackColor = true;
            this.buttonShowChanges.Click += new System.EventHandler(this.buttonShowChanges_Click);
            // 
            // TabCrucible
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonShowChanges);
            this.Name = "TabCrucible";
            this.Size = new System.Drawing.Size(737, 310);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonShowChanges;
    }
}

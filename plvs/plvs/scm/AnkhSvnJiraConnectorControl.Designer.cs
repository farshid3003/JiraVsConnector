namespace Atlassian.plvs.scm {
    partial class AnkhSvnJiraConnectorControl {
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
            this.checkIntegrate = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkIntegrate
            // 
            this.checkIntegrate.AutoSize = true;
            this.checkIntegrate.Location = new System.Drawing.Point(3, 3);
            this.checkIntegrate.Name = "checkIntegrate";
            this.checkIntegrate.Size = new System.Drawing.Size(187, 17);
            this.checkIntegrate.TabIndex = 1;
            this.checkIntegrate.Text = "Integrate with Atlassian Connector";
            this.checkIntegrate.UseVisualStyleBackColor = true;
            this.checkIntegrate.CheckedChanged += new System.EventHandler(this.checkIntegrate_CheckedChanged);
            // 
            // AnkhSvnJiraConnectorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.checkIntegrate);
            this.Name = "AnkhSvnJiraConnectorControl";
            this.Size = new System.Drawing.Size(287, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkIntegrate;

    }
}

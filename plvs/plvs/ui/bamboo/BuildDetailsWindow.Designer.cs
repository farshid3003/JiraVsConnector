namespace Atlassian.plvs.ui.bamboo {
    partial class BuildDetailsWindow {
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
            this.tabBuilds = new TabControlWithCloseButton();
            this.SuspendLayout();
            // 
            // tabBuilds
            // 
            this.tabBuilds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabBuilds.Location = new System.Drawing.Point(0, 0);
            this.tabBuilds.Name = "tabBuilds";
            this.tabBuilds.SelectedIndex = 0;
            this.tabBuilds.ShowToolTips = true;
            this.tabBuilds.Size = new System.Drawing.Size(803, 294);
            this.tabBuilds.TabIndex = 0;
            // 
            // BuildDetailsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabBuilds);
            this.Name = "BuildDetailsWindow";
            this.Size = new System.Drawing.Size(803, 294);
            this.ResumeLayout(false);

        }

        #endregion

        private TabControlWithCloseButton tabBuilds;
    }
}
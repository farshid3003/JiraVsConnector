namespace Atlassian.plvs.ui.jira {
    partial class JiraTextAreaWithWikiPreview {
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
            this.tabContents = new System.Windows.Forms.TabControl();
            this.tabMarkup = new System.Windows.Forms.TabPage();
            this.textMarkup = new System.Windows.Forms.TextBox();
            this.tabPreview = new System.Windows.Forms.TabPage();
            this.webPreview = new System.Windows.Forms.WebBrowser();
            this.tabContents.SuspendLayout();
            this.tabMarkup.SuspendLayout();
            this.tabPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabContents
            // 
            this.tabContents.Controls.Add(this.tabMarkup);
            this.tabContents.Controls.Add(this.tabPreview);
            this.tabContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabContents.Location = new System.Drawing.Point(0, 0);
            this.tabContents.Name = "tabContents";
            this.tabContents.SelectedIndex = 0;
            this.tabContents.Size = new System.Drawing.Size(504, 283);
            this.tabContents.TabIndex = 0;
            this.tabContents.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabContents_Selected);
            // 
            // tabMarkup
            // 
            this.tabMarkup.Controls.Add(this.textMarkup);
            this.tabMarkup.Location = new System.Drawing.Point(4, 22);
            this.tabMarkup.Name = "tabMarkup";
            this.tabMarkup.Padding = new System.Windows.Forms.Padding(3);
            this.tabMarkup.Size = new System.Drawing.Size(496, 257);
            this.tabMarkup.TabIndex = 0;
            this.tabMarkup.Text = "Wiki Markup";
            this.tabMarkup.UseVisualStyleBackColor = true;
            // 
            // textMarkup
            // 
            this.textMarkup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textMarkup.Location = new System.Drawing.Point(3, 3);
            this.textMarkup.Multiline = true;
            this.textMarkup.Name = "textMarkup";
            this.textMarkup.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textMarkup.Size = new System.Drawing.Size(490, 251);
            this.textMarkup.TabIndex = 0;
            this.textMarkup.TextChanged += new System.EventHandler(this.textMarkup_TextChanged);
            // 
            // tabPreview
            // 
            this.tabPreview.Controls.Add(this.webPreview);
            this.tabPreview.Location = new System.Drawing.Point(4, 22);
            this.tabPreview.Name = "tabPreview";
            this.tabPreview.Padding = new System.Windows.Forms.Padding(3);
            this.tabPreview.Size = new System.Drawing.Size(496, 257);
            this.tabPreview.TabIndex = 1;
            this.tabPreview.Text = "Preview";
            this.tabPreview.UseVisualStyleBackColor = true;
            // 
            // webPreview
            // 
            this.webPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webPreview.Location = new System.Drawing.Point(3, 3);
            this.webPreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.webPreview.Name = "webPreview";
            this.webPreview.Size = new System.Drawing.Size(490, 251);
            this.webPreview.TabIndex = 0;
            this.webPreview.WebBrowserShortcutsEnabled = false;
            this.webPreview.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webPreview_Navigating);
            // 
            // JiraTextAreaWithWikiPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabContents);
            this.Name = "JiraTextAreaWithWikiPreview";
            this.Size = new System.Drawing.Size(504, 283);
            this.tabContents.ResumeLayout(false);
            this.tabMarkup.ResumeLayout(false);
            this.tabMarkup.PerformLayout();
            this.tabPreview.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabContents;
        private System.Windows.Forms.TabPage tabMarkup;
        private System.Windows.Forms.TabPage tabPreview;
        private System.Windows.Forms.TextBox textMarkup;
        private System.Windows.Forms.WebBrowser webPreview;
    }
}

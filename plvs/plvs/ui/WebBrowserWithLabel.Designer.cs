namespace Atlassian.plvs.ui {
    partial class WebBrowserWithLabel {
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.webContent = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.SystemColors.Window;
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(490, 23);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "No Title";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // webContent
            // 
            this.webContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webContent.IsWebBrowserContextMenuEnabled = false;
            this.webContent.Location = new System.Drawing.Point(0, 23);
            this.webContent.MinimumSize = new System.Drawing.Size(20, 20);
            this.webContent.Name = "webContent";
            this.webContent.ScriptErrorsSuppressed = true;
            this.webContent.Size = new System.Drawing.Size(490, 363);
            this.webContent.TabIndex = 1;
            this.webContent.WebBrowserShortcutsEnabled = false;
            this.webContent.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webContent_Navigating);
            this.webContent.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webContent_Navigated);
            // 
            // WebBrowserWithLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.webContent);
            this.Controls.Add(this.labelTitle);
            this.Name = "WebBrowserWithLabel";
            this.Size = new System.Drawing.Size(490, 386);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.WebBrowser webContent;
    }
}

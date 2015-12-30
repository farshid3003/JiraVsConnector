namespace Atlassian.plvs.dialogs {
    partial class ProjectConfiguration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectConfiguration));
            this.serverTree = new System.Windows.Forms.TreeView();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonTest = new System.Windows.Forms.Button();
            this.webServerDetails = new System.Windows.Forms.WebBrowser();
            this.menuJira = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBamboo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.addNewServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jIRAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bambooToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAsDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverTree
            // 
            this.serverTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.serverTree.HideSelection = false;
            this.serverTree.Location = new System.Drawing.Point(12, 12);
            this.serverTree.Name = "serverTree";
            this.serverTree.Size = new System.Drawing.Size(235, 219);
            this.serverTree.TabIndex = 0;
            this.serverTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.serverTree_AfterSelect);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(643, 277);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEdit.Enabled = false;
            this.buttonEdit.Location = new System.Drawing.Point(492, 208);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(110, 23);
            this.buttonEdit.TabIndex = 3;
            this.buttonEdit.Text = "Edit...";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDelete.Enabled = false;
            this.buttonDelete.Location = new System.Drawing.Point(608, 208);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(110, 23);
            this.buttonDelete.TabIndex = 5;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonTest
            // 
            this.buttonTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTest.Enabled = false;
            this.buttonTest.Location = new System.Drawing.Point(376, 208);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(110, 23);
            this.buttonTest.TabIndex = 6;
            this.buttonTest.Text = "Test Connection";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // webServerDetails
            // 
            this.webServerDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webServerDetails.IsWebBrowserContextMenuEnabled = false;
            this.webServerDetails.Location = new System.Drawing.Point(255, 12);
            this.webServerDetails.MinimumSize = new System.Drawing.Size(20, 20);
            this.webServerDetails.Name = "webServerDetails";
            this.webServerDetails.ScriptErrorsSuppressed = true;
            this.webServerDetails.Size = new System.Drawing.Size(463, 190);
            this.webServerDetails.TabIndex = 7;
            this.webServerDetails.WebBrowserShortcutsEnabled = false;
            this.webServerDetails.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webServerDetails_Navigating);
            // 
            // menuJira
            // 
            this.menuJira.Image = global::Atlassian.plvs.Resources.tab_jira;
            this.menuJira.Name = "menuJira";
            this.menuJira.Size = new System.Drawing.Size(119, 22);
            this.menuJira.Text = "JIRA";
            // 
            // menuBamboo
            // 
            this.menuBamboo.Image = global::Atlassian.plvs.Resources.tab_bamboo;
            this.menuBamboo.Name = "menuBamboo";
            this.menuBamboo.Size = new System.Drawing.Size(119, 22);
            this.menuBamboo.Text = "Bamboo";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::Atlassian.plvs.Resources.tab_jira;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(119, 22);
            this.toolStripMenuItem1.Text = "JIRA";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = global::Atlassian.plvs.Resources.tab_bamboo;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(119, 22);
            this.toolStripMenuItem2.Text = "Bamboo";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewServerToolStripMenuItem,
            this.setAsDefaultToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(12, 238);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(235, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // addNewServerToolStripMenuItem
            // 
            this.addNewServerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jIRAToolStripMenuItem,
            this.bambooToolStripMenuItem});
            this.addNewServerToolStripMenuItem.Image = global::Atlassian.plvs.Resources.plus;
            this.addNewServerToolStripMenuItem.Name = "addNewServerToolStripMenuItem";
            this.addNewServerToolStripMenuItem.Size = new System.Drawing.Size(119, 20);
            this.addNewServerToolStripMenuItem.Text = "Add New Server";
            // 
            // jIRAToolStripMenuItem
            // 
            this.jIRAToolStripMenuItem.Image = global::Atlassian.plvs.Resources.tab_jira;
            this.jIRAToolStripMenuItem.Name = "jIRAToolStripMenuItem";
            this.jIRAToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.jIRAToolStripMenuItem.Text = "JIRA";
            this.jIRAToolStripMenuItem.Click += new System.EventHandler(this.menuJira_Click);
            // 
            // bambooToolStripMenuItem
            // 
            this.bambooToolStripMenuItem.Image = global::Atlassian.plvs.Resources.tab_bamboo;
            this.bambooToolStripMenuItem.Name = "bambooToolStripMenuItem";
            this.bambooToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.bambooToolStripMenuItem.Text = "Bamboo";
            this.bambooToolStripMenuItem.Click += new System.EventHandler(this.menuBamboo_Click);
            // 
            // setAsDefaultToolStripMenuItem
            // 
            this.setAsDefaultToolStripMenuItem.Image = global::Atlassian.plvs.Resources.bluetick;
            this.setAsDefaultToolStripMenuItem.Name = "setAsDefaultToolStripMenuItem";
            this.setAsDefaultToolStripMenuItem.Size = new System.Drawing.Size(108, 20);
            this.setAsDefaultToolStripMenuItem.Text = "Set As Default";
            this.setAsDefaultToolStripMenuItem.Click += new System.EventHandler(this.setAsDefaultToolStripMenuItem_Click);
            // 
            // ProjectConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 312);
            this.Controls.Add(this.webServerDetails);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.serverTree);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 350);
            this.Name = "ProjectConfiguration";
            this.Text = "Project Configuration";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.projectConfigurationKeyPress);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView serverTree;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.WebBrowser webServerDetails;
        private System.Windows.Forms.ToolStripMenuItem menuJira;
        private System.Windows.Forms.ToolStripMenuItem menuBamboo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addNewServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jIRAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bambooToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setAsDefaultToolStripMenuItem;
    }
}
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.ui.bamboo;

namespace Atlassian.plvs.windows {
    partial class AtlassianPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainContainer = new System.Windows.Forms.ToolStripContainer();
            this.productTabs = new System.Windows.Forms.TabControl();
            this.tabIssues = new System.Windows.Forms.TabPage();
            this.tabJira = new ui.jira.TabJira();
            this.tabBuilds = new System.Windows.Forms.TabPage();
            this.tabBamboo = new ui.bamboo.TabBamboo();
            this.globalToolBar = new System.Windows.Forms.ToolStrip();
            this.buttonProjectProperties = new System.Windows.Forms.ToolStripButton();
            this.buttonGlobalProperties = new System.Windows.Forms.ToolStripButton();
            this.buttonAbout = new System.Windows.Forms.ToolStripButton();
            this.buttonReportBug = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.notifyUpdate = new System.Windows.Forms.NotifyIcon(this.components);
            this.tabReviews = new System.Windows.Forms.TabPage();
            this.tabCrucible = new ui.crucible.TabCrucible();
            this.mainContainer.ContentPanel.SuspendLayout();
            this.mainContainer.LeftToolStripPanel.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.productTabs.SuspendLayout();
            this.tabIssues.SuspendLayout();
            this.tabBuilds.SuspendLayout();
            this.globalToolBar.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tabReviews.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainContainer
            // 
            // 
            // mainContainer.ContentPanel
            // 
            this.mainContainer.ContentPanel.Controls.Add(this.productTabs);
            this.mainContainer.ContentPanel.Size = new System.Drawing.Size(828, 311);
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // mainContainer.LeftToolStripPanel
            // 
            this.mainContainer.LeftToolStripPanel.Controls.Add(this.globalToolBar);
            this.mainContainer.Location = new System.Drawing.Point(0, 0);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Size = new System.Drawing.Size(852, 336);
            this.mainContainer.TabIndex = 2;
            this.mainContainer.Text = "toolStripContainer1";
            // 
            // productTabs
            // 
            this.productTabs.Controls.Add(this.tabIssues);
            this.productTabs.Controls.Add(this.tabBuilds);
            this.productTabs.Controls.Add(this.tabReviews);
            this.productTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productTabs.Location = new System.Drawing.Point(0, 0);
            this.productTabs.Name = "productTabs";
            this.productTabs.SelectedIndex = 0;
            this.productTabs.Size = new System.Drawing.Size(828, 311);
            this.productTabs.TabIndex = 0;
            // 
            // tabIssues
            // 
            this.tabIssues.Controls.Add(this.tabJira);
            this.tabIssues.ImageIndex = 0;
            this.tabIssues.Location = new System.Drawing.Point(4, 22);
            this.tabIssues.Name = "tabIssues";
            this.tabIssues.Padding = new System.Windows.Forms.Padding(3);
            this.tabIssues.Size = new System.Drawing.Size(820, 285);
            this.tabIssues.TabIndex = 0;
            this.tabIssues.Text = "Issues - JIRA";
            this.tabIssues.UseVisualStyleBackColor = true;
            // 
            // tabJira
            // 
            this.tabJira.BackColor = System.Drawing.SystemColors.Control;
            this.tabJira.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabJira.Location = new System.Drawing.Point(3, 3);
            this.tabJira.Name = "tabJira";
            this.tabJira.Size = new System.Drawing.Size(814, 279);
            this.tabJira.TabIndex = 0;
            // 
            // tabBuilds
            // 
            this.tabBuilds.Controls.Add(this.tabBamboo);
            this.tabBuilds.ImageIndex = 1;
            this.tabBuilds.Location = new System.Drawing.Point(4, 22);
            this.tabBuilds.Name = "tabBuilds";
            this.tabBuilds.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuilds.Size = new System.Drawing.Size(820, 285);
            this.tabBuilds.TabIndex = 1;
            this.tabBuilds.Text = "Builds - Bamboo";
            this.tabBuilds.UseVisualStyleBackColor = true;
            // 
            // tabBamboo
            // 
            this.tabBamboo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabBamboo.Location = new System.Drawing.Point(3, 3);
            this.tabBamboo.Name = "tabBamboo";
            this.tabBamboo.Size = new System.Drawing.Size(814, 279);
            this.tabBamboo.TabIndex = 0;
            // 
            // globalToolBar
            // 
            this.globalToolBar.Dock = System.Windows.Forms.DockStyle.None;
            this.globalToolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.globalToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonProjectProperties,
            this.buttonGlobalProperties,
            this.buttonAbout,
            this.buttonReportBug});
            this.globalToolBar.Location = new System.Drawing.Point(0, 3);
            this.globalToolBar.Name = "globalToolBar";
            this.globalToolBar.Size = new System.Drawing.Size(24, 94);
            this.globalToolBar.TabIndex = 0;
            // 
            // buttonProjectProperties
            // 
            this.buttonProjectProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonProjectProperties.Image = global::Atlassian.plvs.Resources.projectsettings;
            this.buttonProjectProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonProjectProperties.Name = "buttonProjectProperties";
            this.buttonProjectProperties.Size = new System.Drawing.Size(22, 20);
            this.buttonProjectProperties.Text = "Project Configuration";
            this.buttonProjectProperties.Click += new System.EventHandler(this.buttonProjectProperties_Click);
            // 
            // buttonGlobalProperties
            // 
            this.buttonGlobalProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonGlobalProperties.Image = global::Atlassian.plvs.Resources.global_properties;
            this.buttonGlobalProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonGlobalProperties.Name = "buttonGlobalProperties";
            this.buttonGlobalProperties.Size = new System.Drawing.Size(22, 20);
            this.buttonGlobalProperties.Text = "Global Configuration";
            this.buttonGlobalProperties.Click += new System.EventHandler(this.buttonGlobalProperties_Click);
            // 
            // buttonAbout
            // 
            this.buttonAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAbout.Image = global::Atlassian.plvs.Resources.about;
            this.buttonAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(22, 20);
            this.buttonAbout.Text = "About";
            this.buttonAbout.Click += new System.EventHandler(this.buttonAbout_Click);
            // 
            // buttonReportBug
            // 
            this.buttonReportBug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonReportBug.Image = global::Atlassian.plvs.Resources.bug;
            this.buttonReportBug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonReportBug.Name = "buttonReportBug";
            this.buttonReportBug.Size = new System.Drawing.Size(22, 20);
            this.buttonReportBug.Text = "Report a Bug or Suggest a Feature";
            this.buttonReportBug.Click += new System.EventHandler(this.buttonReportBug_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(852, 311);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(852, 336);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // notifyUpdate
            // 
            this.notifyUpdate.Visible = true;
            this.notifyUpdate.BalloonTipClicked += new System.EventHandler(this.notifyUpdate_BalloonTipClicked);
            this.notifyUpdate.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyUpdate_MouseDoubleClick);
            // 
            // tabReviews
            // 
            this.tabReviews.Controls.Add(this.tabCrucible);
            this.tabReviews.Location = new System.Drawing.Point(4, 22);
            this.tabReviews.Name = "tabReviews";
            this.tabReviews.Size = new System.Drawing.Size(820, 285);
            this.tabReviews.TabIndex = 2;
            this.tabReviews.Text = "Reviews - Crucible";
            this.tabReviews.UseVisualStyleBackColor = true;
            // 
            // tabCrucible
            // 
            this.tabCrucible.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCrucible.Location = new System.Drawing.Point(0, 0);
            this.tabCrucible.Name = "tabCrucible";
            this.tabCrucible.Size = new System.Drawing.Size(820, 285);
            this.tabCrucible.TabIndex = 0;
            // 
            // AtlassianPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "AtlassianPanel";
            this.Size = new System.Drawing.Size(852, 336);
            this.mainContainer.ContentPanel.ResumeLayout(false);
            this.mainContainer.LeftToolStripPanel.ResumeLayout(false);
            this.mainContainer.LeftToolStripPanel.PerformLayout();
            this.mainContainer.ResumeLayout(false);
            this.mainContainer.PerformLayout();
            this.productTabs.ResumeLayout(false);
            this.tabIssues.ResumeLayout(false);
            this.tabBuilds.ResumeLayout(false);
            this.globalToolBar.ResumeLayout(false);
            this.globalToolBar.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tabReviews.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer mainContainer;
        private System.Windows.Forms.TabControl productTabs;
        private System.Windows.Forms.TabPage tabIssues;
        private System.Windows.Forms.ToolStrip globalToolBar;
        private System.Windows.Forms.ToolStripButton buttonProjectProperties;
        private System.Windows.Forms.ToolStripButton buttonAbout;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripButton buttonGlobalProperties;
        private System.Windows.Forms.TabPage tabBuilds;
        private TabBamboo tabBamboo;
        private TabJira tabJira;
        private System.Windows.Forms.NotifyIcon notifyUpdate;
        private System.Windows.Forms.ToolStripButton buttonReportBug;
        private System.Windows.Forms.TabPage tabReviews;
        private global::Atlassian.plvs.ui.crucible.TabCrucible tabCrucible;
    }
}
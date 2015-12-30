namespace Atlassian.plvs.ui.bamboo {
    partial class BuildDetailsPanel {
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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.webSummary = new System.Windows.Forms.WebBrowser();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.tabLogPanels = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.webLog = new System.Windows.Forms.WebBrowser();
            this.tabTests = new System.Windows.Forms.TabPage();
            this.labelNoTestsFound = new System.Windows.Forms.Label();
            this.toolStripContainerTests = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonViewInBrowser = new System.Windows.Forms.ToolStripButton();
            this.buttonRun = new System.Windows.Forms.ToolStripButton();
            this.testResultsToolStrip = new System.Windows.Forms.ToolStrip();
            this.buttonFailedOnly = new System.Windows.Forms.ToolStripButton();
            this.buttonOpenTest = new System.Windows.Forms.ToolStripButton();
            this.buttonRunTestInVs = new System.Windows.Forms.ToolStripButton();
            this.buttonDebugTestInVs = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabSummary.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.tabLogPanels.SuspendLayout();
            this.tabTests.SuspendLayout();
            this.toolStripContainerTests.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tabControl);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(666, 205);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(666, 252);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(666, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 0;
            // 
            // labelStatus
            // 
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabSummary);
            this.tabControl.Controls.Add(this.tabLog);
            this.tabControl.Controls.Add(this.tabTests);
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(663, 199);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabSummary
            // 
            this.tabSummary.Controls.Add(this.webSummary);
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Padding = new System.Windows.Forms.Padding(3);
            this.tabSummary.Size = new System.Drawing.Size(655, 173);
            this.tabSummary.TabIndex = 0;
            this.tabSummary.Text = "Summary";
            this.tabSummary.UseVisualStyleBackColor = true;
            // 
            // webSummary
            // 
            this.webSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webSummary.IsWebBrowserContextMenuEnabled = false;
            this.webSummary.Location = new System.Drawing.Point(3, 3);
            this.webSummary.MinimumSize = new System.Drawing.Size(20, 20);
            this.webSummary.Name = "webSummary";
            this.webSummary.ScriptErrorsSuppressed = true;
            this.webSummary.Size = new System.Drawing.Size(649, 167);
            this.webSummary.TabIndex = 0;
            this.webSummary.WebBrowserShortcutsEnabled = false;
            this.webSummary.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webSummary_DocumentCompleted);
            this.webSummary.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webSummary_Navigating);
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.tabLogPanels);
            this.tabLog.Controls.Add(this.webLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(655, 173);
            this.tabLog.TabIndex = 1;
            this.tabLog.Text = "Build Logs";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // tabLogPanels
            // 
            this.tabLogPanels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabLogPanels.Controls.Add(this.tabPage1);
            this.tabLogPanels.Controls.Add(this.tabPage2);
            this.tabLogPanels.Location = new System.Drawing.Point(21, 17);
            this.tabLogPanels.Name = "tabLogPanels";
            this.tabLogPanels.SelectedIndex = 0;
            this.tabLogPanels.Size = new System.Drawing.Size(543, 137);
            this.tabLogPanels.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(535, 111);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(535, 111);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // webLog
            // 
            this.webLog.IsWebBrowserContextMenuEnabled = false;
            this.webLog.Location = new System.Drawing.Point(526, 23);
            this.webLog.MinimumSize = new System.Drawing.Size(20, 20);
            this.webLog.Name = "webLog";
            this.webLog.ScriptErrorsSuppressed = true;
            this.webLog.Size = new System.Drawing.Size(107, 94);
            this.webLog.TabIndex = 0;
            this.webLog.WebBrowserShortcutsEnabled = false;
            // 
            // tabTests
            // 
            this.tabTests.Controls.Add(this.labelNoTestsFound);
            this.tabTests.Controls.Add(this.toolStripContainerTests);
            this.tabTests.Location = new System.Drawing.Point(4, 22);
            this.tabTests.Name = "tabTests";
            this.tabTests.Size = new System.Drawing.Size(655, 173);
            this.tabTests.TabIndex = 2;
            this.tabTests.Text = "Test Results";
            this.tabTests.UseVisualStyleBackColor = true;
            // 
            // labelNoTestsFound
            // 
            this.labelNoTestsFound.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNoTestsFound.Location = new System.Drawing.Point(0, 0);
            this.labelNoTestsFound.Name = "labelNoTestsFound";
            this.labelNoTestsFound.Size = new System.Drawing.Size(655, 173);
            this.labelNoTestsFound.TabIndex = 17;
            this.labelNoTestsFound.Text = "No Tests Found";
            this.labelNoTestsFound.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolStripContainerTests
            // 
            this.toolStripContainerTests.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainerTests.ContentPanel
            // 
            this.toolStripContainerTests.ContentPanel.Size = new System.Drawing.Size(145, 42);
            this.toolStripContainerTests.LeftToolStripPanelVisible = false;
            this.toolStripContainerTests.Location = new System.Drawing.Point(12, 7);
            this.toolStripContainerTests.Name = "toolStripContainerTests";
            this.toolStripContainerTests.RightToolStripPanelVisible = false;
            this.toolStripContainerTests.Size = new System.Drawing.Size(145, 67);
            this.toolStripContainerTests.TabIndex = 16;
            this.toolStripContainerTests.Text = "toolStripContainer2";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonViewInBrowser,
            this.buttonRun});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(49, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // buttonViewInBrowser
            // 
            this.buttonViewInBrowser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonViewInBrowser.Image = global::Atlassian.plvs.Resources.view_in_browser;
            this.buttonViewInBrowser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonViewInBrowser.Name = "buttonViewInBrowser";
            this.buttonViewInBrowser.Size = new System.Drawing.Size(23, 22);
            this.buttonViewInBrowser.Text = "View In Browser";
            this.buttonViewInBrowser.Click += new System.EventHandler(this.buttonViewInBrowser_Click);
            // 
            // buttonRun
            // 
            this.buttonRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRun.Image = global::Atlassian.plvs.Resources.run_build;
            this.buttonRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(23, 22);
            this.buttonRun.Text = "Run Build";
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // testResultsToolStrip
            // 
            this.testResultsToolStrip.Location = new System.Drawing.Point(0, 0);
            this.testResultsToolStrip.Name = "testResultsToolStrip";
            this.testResultsToolStrip.Size = new System.Drawing.Size(100, 25);
            this.testResultsToolStrip.TabIndex = 0;
            // 
            // buttonFailedOnly
            // 
            this.buttonFailedOnly.CheckOnClick = true;
            this.buttonFailedOnly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonFailedOnly.Image = global::Atlassian.plvs.Resources.icn_plan_failed;
            this.buttonFailedOnly.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonFailedOnly.Name = "buttonFailedOnly";
            this.buttonFailedOnly.Size = new System.Drawing.Size(23, 22);
            this.buttonFailedOnly.Text = "Show Failed Tests Only";
            // 
            // buttonOpenTest
            // 
            this.buttonOpenTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonOpenTest.Image = global::Atlassian.plvs.Resources.open_in_ide;
            this.buttonOpenTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOpenTest.Name = "buttonOpenTest";
            this.buttonOpenTest.Size = new System.Drawing.Size(23, 22);
            this.buttonOpenTest.Text = "Open Test";
            // 
            // buttonRunTestInVs
            // 
            this.buttonRunTestInVs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRunTestInVs.Image = global::Atlassian.plvs.Resources.run_build;
            this.buttonRunTestInVs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRunTestInVs.Name = "buttonRunTestInVs";
            this.buttonRunTestInVs.Size = new System.Drawing.Size(23, 22);
            this.buttonRunTestInVs.Text = "Run Test";
            // 
            // buttonDebugTestInVs
            // 
            this.buttonDebugTestInVs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDebugTestInVs.Image = global::Atlassian.plvs.Resources.bug;
            this.buttonDebugTestInVs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDebugTestInVs.Name = "buttonDebugTestInVs";
            this.buttonDebugTestInVs.Size = new System.Drawing.Size(23, 22);
            this.buttonDebugTestInVs.Text = "Debug Test";
            // 
            // BuildDetailsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "BuildDetailsPanel";
            this.Size = new System.Drawing.Size(666, 252);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabSummary.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tabLogPanels.ResumeLayout(false);
            this.tabTests.ResumeLayout(false);
            this.toolStripContainerTests.ResumeLayout(false);
            this.toolStripContainerTests.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabSummary;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripStatusLabel labelStatus;
        private System.Windows.Forms.WebBrowser webSummary;
        private System.Windows.Forms.WebBrowser webLog;
        private System.Windows.Forms.ToolStripButton buttonViewInBrowser;
        private System.Windows.Forms.ToolStripButton buttonRun;
        private System.Windows.Forms.TabPage tabTests;
        private System.Windows.Forms.ToolStripContainer toolStripContainerTests;
        private System.Windows.Forms.ToolStrip testResultsToolStrip;
        private System.Windows.Forms.ToolStripButton buttonFailedOnly;
        private System.Windows.Forms.ToolStripButton buttonOpenTest;
        private System.Windows.Forms.ToolStripButton buttonRunTestInVs;
        private System.Windows.Forms.ToolStripButton buttonDebugTestInVs;
        private System.Windows.Forms.Label labelNoTestsFound;
        private System.Windows.Forms.TabControl tabLogPanels;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}

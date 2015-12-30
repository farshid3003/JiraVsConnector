namespace Atlassian.plvs.dialogs {
    partial class GlobalSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlobalSettings));
            this.checkAutoupdate = new System.Windows.Forms.CheckBox();
            this.checkUnstable = new System.Windows.Forms.CheckBox();
            this.checkStats = new System.Windows.Forms.CheckBox();
            this.buttonCheckNow = new System.Windows.Forms.Button();
            this.radioStable = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkUsageStatsDetails = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioUnstable = new System.Windows.Forms.RadioButton();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.numericMaxIssueLinksFileLength = new System.Windows.Forms.NumericUpDown();
            this.radioDisableIssueLinksForLargeFiles = new System.Windows.Forms.RadioButton();
            this.radioDisableIssueLinksForAllFiles = new System.Windows.Forms.RadioButton();
            this.checkDisableIssueLinks = new System.Windows.Forms.CheckBox();
            this.checkAnkhSvn = new System.Windows.Forms.CheckBox();
            this.checkJiraExplorer = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericJiraBatchSize = new System.Windows.Forms.NumericUpDown();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.numericBambooPollingInterval = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericProxyPort = new System.Windows.Forms.NumericUpDown();
            this.textProxyPassword = new System.Windows.Forms.TextBox();
            this.textProxyUserName = new System.Windows.Forms.TextBox();
            this.textProxyHost = new System.Windows.Forms.TextBox();
            this.checkUseProxyAuth = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.radioUseNoProxy = new System.Windows.Forms.RadioButton();
            this.radioUseCustomProxy = new System.Windows.Forms.RadioButton();
            this.radioUseSystemProxy = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.numericNetworkTimeout = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxIssueLinksFileLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericJiraBatchSize)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericBambooPollingInterval)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericProxyPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericNetworkTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // checkAutoupdate
            // 
            this.checkAutoupdate.AutoSize = true;
            this.checkAutoupdate.Location = new System.Drawing.Point(6, 19);
            this.checkAutoupdate.Name = "checkAutoupdate";
            this.checkAutoupdate.Size = new System.Drawing.Size(264, 17);
            this.checkAutoupdate.TabIndex = 0;
            this.checkAutoupdate.Text = "Automatically check for availability of new versions";
            this.checkAutoupdate.UseVisualStyleBackColor = true;
            this.checkAutoupdate.CheckedChanged += new System.EventHandler(this.checkAutoupdate_CheckedChanged);
            // 
            // checkUnstable
            // 
            this.checkUnstable.AutoSize = true;
            this.checkUnstable.Location = new System.Drawing.Point(22, 43);
            this.checkUnstable.Name = "checkUnstable";
            this.checkUnstable.Size = new System.Drawing.Size(239, 17);
            this.checkUnstable.TabIndex = 1;
            this.checkUnstable.Text = "Check stable versions and snaphost versions";
            this.checkUnstable.UseVisualStyleBackColor = true;
            this.checkUnstable.CheckedChanged += new System.EventHandler(this.checkUnstable_CheckedChanged);
            // 
            // checkStats
            // 
            this.checkStats.AutoSize = true;
            this.checkStats.Location = new System.Drawing.Point(22, 67);
            this.checkStats.Name = "checkStats";
            this.checkStats.Size = new System.Drawing.Size(133, 17);
            this.checkStats.TabIndex = 2;
            this.checkStats.Text = "Report usage statistics";
            this.checkStats.UseVisualStyleBackColor = true;
            this.checkStats.CheckedChanged += new System.EventHandler(this.checkStats_CheckedChanged);
            // 
            // buttonCheckNow
            // 
            this.buttonCheckNow.Location = new System.Drawing.Point(358, 19);
            this.buttonCheckNow.Name = "buttonCheckNow";
            this.buttonCheckNow.Size = new System.Drawing.Size(162, 23);
            this.buttonCheckNow.TabIndex = 3;
            this.buttonCheckNow.Text = "Check Now";
            this.buttonCheckNow.UseVisualStyleBackColor = true;
            this.buttonCheckNow.Click += new System.EventHandler(this.buttonCheckNow_Click);
            // 
            // radioStable
            // 
            this.radioStable.AutoSize = true;
            this.radioStable.Location = new System.Drawing.Point(358, 48);
            this.radioStable.Name = "radioStable";
            this.radioStable.Size = new System.Drawing.Size(93, 17);
            this.radioStable.TabIndex = 4;
            this.radioStable.TabStop = true;
            this.radioStable.Text = "Stable Version";
            this.radioStable.UseVisualStyleBackColor = true;
            this.radioStable.CheckedChanged += new System.EventHandler(this.radioStable_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.linkUsageStatsDetails);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.radioUnstable);
            this.groupBox1.Controls.Add(this.radioStable);
            this.groupBox1.Controls.Add(this.checkAutoupdate);
            this.groupBox1.Controls.Add(this.buttonCheckNow);
            this.groupBox1.Controls.Add(this.checkUnstable);
            this.groupBox1.Controls.Add(this.checkStats);
            this.groupBox1.Location = new System.Drawing.Point(6, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(526, 96);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Automatic Updates";
            // 
            // linkUsageStatsDetails
            // 
            this.linkUsageStatsDetails.AutoSize = true;
            this.linkUsageStatsDetails.Location = new System.Drawing.Point(167, 68);
            this.linkUsageStatsDetails.Name = "linkUsageStatsDetails";
            this.linkUsageStatsDetails.Size = new System.Drawing.Size(39, 13);
            this.linkUsageStatsDetails.TabIndex = 8;
            this.linkUsageStatsDetails.TabStop = true;
            this.linkUsageStatsDetails.Text = "Details";
            this.linkUsageStatsDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkUsageStatsDetails_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(204, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = ")";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(161, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "(";
            // 
            // radioUnstable
            // 
            this.radioUnstable.AutoSize = true;
            this.radioUnstable.Location = new System.Drawing.Point(358, 71);
            this.radioUnstable.Name = "radioUnstable";
            this.radioUnstable.Size = new System.Drawing.Size(162, 17);
            this.radioUnstable.TabIndex = 5;
            this.radioUnstable.TabStop = true;
            this.radioUnstable.Text = "Stable and Snapshot Version";
            this.radioUnstable.UseVisualStyleBackColor = true;
            this.radioUnstable.CheckedChanged += new System.EventHandler(this.radioUnstable_CheckedChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(403, 292);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(484, 292);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(13, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(546, 274);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(538, 248);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.checkAnkhSvn);
            this.tabPage2.Controls.Add(this.checkJiraExplorer);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.numericJiraBatchSize);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(538, 248);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "JIRA";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.numericMaxIssueLinksFileLength);
            this.groupBox3.Controls.Add(this.radioDisableIssueLinksForLargeFiles);
            this.groupBox3.Controls.Add(this.radioDisableIssueLinksForAllFiles);
            this.groupBox3.Controls.Add(this.checkDisableIssueLinks);
            this.groupBox3.Location = new System.Drawing.Point(9, 44);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(465, 105);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "JIRA Issue Links In Editor";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(276, 69);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(28, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "lines";
            // 
            // numericMaxIssueLinksFileLength
            // 
            this.numericMaxIssueLinksFileLength.Location = new System.Drawing.Point(150, 67);
            this.numericMaxIssueLinksFileLength.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericMaxIssueLinksFileLength.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericMaxIssueLinksFileLength.Name = "numericMaxIssueLinksFileLength";
            this.numericMaxIssueLinksFileLength.Size = new System.Drawing.Size(120, 20);
            this.numericMaxIssueLinksFileLength.TabIndex = 3;
            this.numericMaxIssueLinksFileLength.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericMaxIssueLinksFileLength.ValueChanged += new System.EventHandler(this.numericMaxIssueLinksFileLength_ValueChanged);
            // 
            // radioDisableIssueLInksForLargeFiles
            // 
            this.radioDisableIssueLinksForLargeFiles.AutoSize = true;
            this.radioDisableIssueLinksForLargeFiles.Location = new System.Drawing.Point(19, 67);
            this.radioDisableIssueLinksForLargeFiles.Name = "radioDisableIssueLinksForLargeFiles";
            this.radioDisableIssueLinksForLargeFiles.Size = new System.Drawing.Size(125, 17);
            this.radioDisableIssueLinksForLargeFiles.TabIndex = 2;
            this.radioDisableIssueLinksForLargeFiles.TabStop = true;
            this.radioDisableIssueLinksForLargeFiles.Text = "For Files Larger Than";
            this.radioDisableIssueLinksForLargeFiles.UseVisualStyleBackColor = true;
            this.radioDisableIssueLinksForLargeFiles.CheckedChanged += new System.EventHandler(this.radioDisableIssueLInksForLargeFiles_CheckedChanged);
            // 
            // radioDisableIssueLinksForAllFiles
            // 
            this.radioDisableIssueLinksForAllFiles.AutoSize = true;
            this.radioDisableIssueLinksForAllFiles.Location = new System.Drawing.Point(19, 43);
            this.radioDisableIssueLinksForAllFiles.Name = "radioDisableIssueLinksForAllFiles";
            this.radioDisableIssueLinksForAllFiles.Size = new System.Drawing.Size(78, 17);
            this.radioDisableIssueLinksForAllFiles.TabIndex = 1;
            this.radioDisableIssueLinksForAllFiles.TabStop = true;
            this.radioDisableIssueLinksForAllFiles.Text = "For All Files";
            this.radioDisableIssueLinksForAllFiles.UseVisualStyleBackColor = true;
            this.radioDisableIssueLinksForAllFiles.CheckedChanged += new System.EventHandler(this.radioDisableIssueLinksForAllFiles_CheckedChanged);
            // 
            // checkDisableIssueLinks
            // 
            this.checkDisableIssueLinks.AutoSize = true;
            this.checkDisableIssueLinks.Location = new System.Drawing.Point(7, 20);
            this.checkDisableIssueLinks.Name = "checkDisableIssueLinks";
            this.checkDisableIssueLinks.Size = new System.Drawing.Size(268, 17);
            this.checkDisableIssueLinks.TabIndex = 0;
            this.checkDisableIssueLinks.Text = "Disable Scanning Text Editors For JIRA Issue Links";
            this.checkDisableIssueLinks.UseVisualStyleBackColor = true;
            this.checkDisableIssueLinks.CheckedChanged += new System.EventHandler(this.checkDisableIssueLinks_CheckedChanged);
            // 
            // checkAnkhSvn
            // 
            this.checkAnkhSvn.AutoSize = true;
            this.checkAnkhSvn.Location = new System.Drawing.Point(9, 189);
            this.checkAnkhSvn.Name = "checkAnkhSvn";
            this.checkAnkhSvn.Size = new System.Drawing.Size(230, 17);
            this.checkAnkhSvn.TabIndex = 3;
            this.checkAnkhSvn.Text = "Enable AnkhSVN Integration (experimental)";
            this.checkAnkhSvn.UseVisualStyleBackColor = true;
            this.checkAnkhSvn.CheckedChanged += new System.EventHandler(this.checkAnkhSvn_CheckedChanged);
            // 
            // checkJiraExplorer
            // 
            this.checkJiraExplorer.AutoSize = true;
            this.checkJiraExplorer.Location = new System.Drawing.Point(9, 166);
            this.checkJiraExplorer.Name = "checkJiraExplorer";
            this.checkJiraExplorer.Size = new System.Drawing.Size(228, 17);
            this.checkJiraExplorer.TabIndex = 2;
            this.checkJiraExplorer.Text = "Enable JIRA Server Explorer (experimental)";
            this.checkJiraExplorer.UseVisualStyleBackColor = true;
            this.checkJiraExplorer.CheckedChanged += new System.EventHandler(this.checkJiraExplorer_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Issues Batch Size";
            // 
            // numericJiraBatchSize
            // 
            this.numericJiraBatchSize.Location = new System.Drawing.Point(149, 14);
            this.numericJiraBatchSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericJiraBatchSize.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericJiraBatchSize.Name = "numericJiraBatchSize";
            this.numericJiraBatchSize.Size = new System.Drawing.Size(60, 20);
            this.numericJiraBatchSize.TabIndex = 0;
            this.numericJiraBatchSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericJiraBatchSize.ValueChanged += new System.EventHandler(this.numericJiraBatchSize_ValueChanged);
            this.numericJiraBatchSize.KeyUp += new System.Windows.Forms.KeyEventHandler(this.numericJiraBatchSize_KeyUp);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.numericBambooPollingInterval);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(538, 248);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Bamboo";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // numericBambooPollingInterval
            // 
            this.numericBambooPollingInterval.Location = new System.Drawing.Point(149, 14);
            this.numericBambooPollingInterval.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numericBambooPollingInterval.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericBambooPollingInterval.Name = "numericBambooPollingInterval";
            this.numericBambooPollingInterval.Size = new System.Drawing.Size(60, 20);
            this.numericBambooPollingInterval.TabIndex = 1;
            this.numericBambooPollingInterval.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericBambooPollingInterval.ValueChanged += new System.EventHandler(this.numericBambooPollingInterval_ValueChanged);
            this.numericBambooPollingInterval.KeyUp += new System.Windows.Forms.KeyEventHandler(this.numericBambooPollingInterval_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Polling Interval [seconds]";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Controls.Add(this.label6);
            this.tabPage4.Controls.Add(this.numericNetworkTimeout);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(538, 248);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Network And Proxy Settings";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericProxyPort);
            this.groupBox2.Controls.Add(this.textProxyPassword);
            this.groupBox2.Controls.Add(this.textProxyUserName);
            this.groupBox2.Controls.Add(this.textProxyHost);
            this.groupBox2.Controls.Add(this.checkUseProxyAuth);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.radioUseNoProxy);
            this.groupBox2.Controls.Add(this.radioUseCustomProxy);
            this.groupBox2.Controls.Add(this.radioUseSystemProxy);
            this.groupBox2.Location = new System.Drawing.Point(6, 39);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(526, 203);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Proxy Settings";
            // 
            // numericProxyPort
            // 
            this.numericProxyPort.Location = new System.Drawing.Point(435, 68);
            this.numericProxyPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericProxyPort.Name = "numericProxyPort";
            this.numericProxyPort.Size = new System.Drawing.Size(74, 20);
            this.numericProxyPort.TabIndex = 12;
            this.numericProxyPort.ValueChanged += new System.EventHandler(this.numericProxyPort_ValueChanged);
            // 
            // textProxyPassword
            // 
            this.textProxyPassword.Location = new System.Drawing.Point(192, 143);
            this.textProxyPassword.Name = "textProxyPassword";
            this.textProxyPassword.PasswordChar = '*';
            this.textProxyPassword.Size = new System.Drawing.Size(173, 20);
            this.textProxyPassword.TabIndex = 11;
            this.textProxyPassword.TextChanged += new System.EventHandler(this.textProxyPassword_TextChanged);
            // 
            // textProxyUserName
            // 
            this.textProxyUserName.Location = new System.Drawing.Point(192, 117);
            this.textProxyUserName.Name = "textProxyUserName";
            this.textProxyUserName.Size = new System.Drawing.Size(173, 20);
            this.textProxyUserName.TabIndex = 10;
            this.textProxyUserName.TextChanged += new System.EventHandler(this.textProxyUserName_TextChanged);
            // 
            // textProxyHost
            // 
            this.textProxyHost.Location = new System.Drawing.Point(138, 68);
            this.textProxyHost.Name = "textProxyHost";
            this.textProxyHost.Size = new System.Drawing.Size(227, 20);
            this.textProxyHost.TabIndex = 8;
            this.textProxyHost.TextChanged += new System.EventHandler(this.textProxyHost_TextChanged);
            // 
            // checkUseProxyAuth
            // 
            this.checkUseProxyAuth.AutoSize = true;
            this.checkUseProxyAuth.Location = new System.Drawing.Point(77, 94);
            this.checkUseProxyAuth.Name = "checkUseProxyAuth";
            this.checkUseProxyAuth.Size = new System.Drawing.Size(145, 17);
            this.checkUseProxyAuth.TabIndex = 7;
            this.checkUseProxyAuth.Text = "Use Proxy Authentication";
            this.checkUseProxyAuth.UseVisualStyleBackColor = true;
            this.checkUseProxyAuth.CheckedChanged += new System.EventHandler(this.checkUseProxyAuth_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(126, 146);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Password";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(126, 120);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "User Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(374, 71);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Proxy Port";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(74, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Proxy Host";
            // 
            // radioUseNoProxy
            // 
            this.radioUseNoProxy.AutoSize = true;
            this.radioUseNoProxy.Location = new System.Drawing.Point(15, 180);
            this.radioUseNoProxy.Name = "radioUseNoProxy";
            this.radioUseNoProxy.Size = new System.Drawing.Size(110, 17);
            this.radioUseNoProxy.TabIndex = 2;
            this.radioUseNoProxy.TabStop = true;
            this.radioUseNoProxy.Text = "Do Not Use Proxy";
            this.radioUseNoProxy.UseVisualStyleBackColor = true;
            this.radioUseNoProxy.CheckedChanged += new System.EventHandler(this.radioUseNoProxy_CheckedChanged);
            // 
            // radioUseCustomProxy
            // 
            this.radioUseCustomProxy.AutoSize = true;
            this.radioUseCustomProxy.Location = new System.Drawing.Point(15, 44);
            this.radioUseCustomProxy.Name = "radioUseCustomProxy";
            this.radioUseCustomProxy.Size = new System.Drawing.Size(152, 17);
            this.radioUseCustomProxy.TabIndex = 1;
            this.radioUseCustomProxy.TabStop = true;
            this.radioUseCustomProxy.Text = "Use Custom Proxy Settings";
            this.radioUseCustomProxy.UseVisualStyleBackColor = true;
            this.radioUseCustomProxy.CheckedChanged += new System.EventHandler(this.radioUseCustomProxy_CheckedChanged);
            // 
            // radioUseSystemProxy
            // 
            this.radioUseSystemProxy.AutoSize = true;
            this.radioUseSystemProxy.Location = new System.Drawing.Point(15, 20);
            this.radioUseSystemProxy.Name = "radioUseSystemProxy";
            this.radioUseSystemProxy.Size = new System.Drawing.Size(151, 17);
            this.radioUseSystemProxy.TabIndex = 0;
            this.radioUseSystemProxy.TabStop = true;
            this.radioUseSystemProxy.Text = "Use System Proxy Settings";
            this.radioUseSystemProxy.UseVisualStyleBackColor = true;
            this.radioUseSystemProxy.CheckedChanged += new System.EventHandler(this.radioUseSystemProxy_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(137, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Network Timeout [seconds]";
            // 
            // numericNetworkTimeout
            // 
            this.numericNetworkTimeout.Location = new System.Drawing.Point(149, 13);
            this.numericNetworkTimeout.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericNetworkTimeout.Name = "numericNetworkTimeout";
            this.numericNetworkTimeout.Size = new System.Drawing.Size(60, 20);
            this.numericNetworkTimeout.TabIndex = 5;
            this.numericNetworkTimeout.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericNetworkTimeout.ValueChanged += new System.EventHandler(this.numericNetworkTimeout_ValueChanged);
            // 
            // GlobalSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 327);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GlobalSettings";
            this.Text = "Global Settings";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.globalSettingsKeyPress);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxIssueLinksFileLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericJiraBatchSize)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericBambooPollingInterval)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericProxyPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericNetworkTimeout)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkAutoupdate;
        private System.Windows.Forms.CheckBox checkUnstable;
        private System.Windows.Forms.CheckBox checkStats;
        private System.Windows.Forms.Button buttonCheckNow;
        private System.Windows.Forms.RadioButton radioStable;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioUnstable;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericJiraBatchSize;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.NumericUpDown numericBambooPollingInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkUsageStatsDetails;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkJiraExplorer;
        private System.Windows.Forms.CheckBox checkAnkhSvn;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkUseProxyAuth;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioUseNoProxy;
        private System.Windows.Forms.RadioButton radioUseCustomProxy;
        private System.Windows.Forms.RadioButton radioUseSystemProxy;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericNetworkTimeout;
        private System.Windows.Forms.TextBox textProxyPassword;
        private System.Windows.Forms.TextBox textProxyUserName;
        private System.Windows.Forms.TextBox textProxyHost;
        private System.Windows.Forms.NumericUpDown numericProxyPort;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numericMaxIssueLinksFileLength;
        private System.Windows.Forms.RadioButton radioDisableIssueLinksForLargeFiles;
        private System.Windows.Forms.RadioButton radioDisableIssueLinksForAllFiles;
        private System.Windows.Forms.CheckBox checkDisableIssueLinks;

    }
}
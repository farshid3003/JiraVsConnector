using Atlassian.plvs.ui;

namespace Atlassian.plvs.dialogs.jira {
    sealed partial class EditCustomFilter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditCustomFilter));
            this.buttonClose = new System.Windows.Forms.Button();
            this.listViewProjects = new System.Windows.Forms.ListView();
            this.listViewIssueTypes = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewFixForVersions = new System.Windows.Forms.ListView();
            this.listViewAffectsVersions = new System.Windows.Forms.ListView();
            this.listViewComponents = new System.Windows.Forms.ListView();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.listViewPriorities = new System.Windows.Forms.ListView();
            this.listViewStatuses = new System.Windows.Forms.ListView();
            this.listViewResolutions = new System.Windows.Forms.ListView();
            this.comboBoxAssignee = new System.Windows.Forms.ComboBox();
            this.comboBoxReporter = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.textBoxFilterName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(571, 418);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // listViewProjects
            // 
            this.listViewProjects.FullRowSelect = true;
            this.listViewProjects.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewProjects.HideSelection = false;
            this.listViewProjects.Location = new System.Drawing.Point(6, 27);
            this.listViewProjects.Name = "listViewProjects";
            this.listViewProjects.Size = new System.Drawing.Size(407, 290);
            this.listViewProjects.TabIndex = 0;
            this.listViewProjects.UseCompatibleStateImageBehavior = false;
            this.listViewProjects.View = System.Windows.Forms.View.Details;
            // 
            // listViewIssueTypes
            // 
            this.listViewIssueTypes.FullRowSelect = true;
            this.listViewIssueTypes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewIssueTypes.HideSelection = false;
            this.listViewIssueTypes.Location = new System.Drawing.Point(419, 27);
            this.listViewIssueTypes.Name = "listViewIssueTypes";
            this.listViewIssueTypes.Size = new System.Drawing.Size(201, 290);
            this.listViewIssueTypes.TabIndex = 1;
            this.listViewIssueTypes.UseCompatibleStateImageBehavior = false;
            this.listViewIssueTypes.View = System.Windows.Forms.View.Details;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(416, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Issue Type";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Project";
            // 
            // listViewFixForVersions
            // 
            this.listViewFixForVersions.FullRowSelect = true;
            this.listViewFixForVersions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewFixForVersions.HideSelection = false;
            this.listViewFixForVersions.Location = new System.Drawing.Point(6, 27);
            this.listViewFixForVersions.Name = "listViewFixForVersions";
            this.listViewFixForVersions.Size = new System.Drawing.Size(201, 290);
            this.listViewFixForVersions.TabIndex = 0;
            this.listViewFixForVersions.UseCompatibleStateImageBehavior = false;
            this.listViewFixForVersions.View = System.Windows.Forms.View.Details;
            // 
            // listViewAffectsVersions
            // 
            this.listViewAffectsVersions.FullRowSelect = true;
            this.listViewAffectsVersions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewAffectsVersions.HideSelection = false;
            this.listViewAffectsVersions.Location = new System.Drawing.Point(212, 27);
            this.listViewAffectsVersions.Name = "listViewAffectsVersions";
            this.listViewAffectsVersions.Size = new System.Drawing.Size(201, 290);
            this.listViewAffectsVersions.TabIndex = 1;
            this.listViewAffectsVersions.UseCompatibleStateImageBehavior = false;
            this.listViewAffectsVersions.View = System.Windows.Forms.View.Details;
            // 
            // listViewComponents
            // 
            this.listViewComponents.FullRowSelect = true;
            this.listViewComponents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewComponents.HideSelection = false;
            this.listViewComponents.Location = new System.Drawing.Point(419, 27);
            this.listViewComponents.Name = "listViewComponents";
            this.listViewComponents.Size = new System.Drawing.Size(201, 290);
            this.listViewComponents.TabIndex = 2;
            this.listViewComponents.UseCompatibleStateImageBehavior = false;
            this.listViewComponents.View = System.Windows.Forms.View.Details;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(210, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Affects Versions";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(416, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Components";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Fix For";
            // 
            // listViewPriorities
            // 
            this.listViewPriorities.FullRowSelect = true;
            this.listViewPriorities.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewPriorities.HideSelection = false;
            this.listViewPriorities.Location = new System.Drawing.Point(419, 27);
            this.listViewPriorities.Name = "listViewPriorities";
            this.listViewPriorities.Size = new System.Drawing.Size(201, 251);
            this.listViewPriorities.TabIndex = 2;
            this.listViewPriorities.UseCompatibleStateImageBehavior = false;
            this.listViewPriorities.View = System.Windows.Forms.View.Details;
            // 
            // listViewStatuses
            // 
            this.listViewStatuses.FullRowSelect = true;
            this.listViewStatuses.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewStatuses.HideSelection = false;
            this.listViewStatuses.Location = new System.Drawing.Point(6, 27);
            this.listViewStatuses.Name = "listViewStatuses";
            this.listViewStatuses.Size = new System.Drawing.Size(201, 251);
            this.listViewStatuses.TabIndex = 0;
            this.listViewStatuses.UseCompatibleStateImageBehavior = false;
            this.listViewStatuses.View = System.Windows.Forms.View.Details;
            // 
            // listViewResolutions
            // 
            this.listViewResolutions.Enabled = false;
            this.listViewResolutions.FullRowSelect = true;
            this.listViewResolutions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewResolutions.HideSelection = false;
            this.listViewResolutions.Location = new System.Drawing.Point(212, 27);
            this.listViewResolutions.Name = "listViewResolutions";
            this.listViewResolutions.Size = new System.Drawing.Size(201, 251);
            this.listViewResolutions.TabIndex = 1;
            this.listViewResolutions.UseCompatibleStateImageBehavior = false;
            this.listViewResolutions.View = System.Windows.Forms.View.Details;
            // 
            // comboBoxAssignee
            // 
            this.comboBoxAssignee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAssignee.Enabled = false;
            this.comboBoxAssignee.FormattingEnabled = true;
            this.comboBoxAssignee.Location = new System.Drawing.Point(419, 303);
            this.comboBoxAssignee.Name = "comboBoxAssignee";
            this.comboBoxAssignee.Size = new System.Drawing.Size(201, 21);
            this.comboBoxAssignee.TabIndex = 4;
            // 
            // comboBoxReporter
            // 
            this.comboBoxReporter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxReporter.Enabled = false;
            this.comboBoxReporter.FormattingEnabled = true;
            this.comboBoxReporter.Location = new System.Drawing.Point(57, 303);
            this.comboBoxReporter.Name = "comboBoxReporter";
            this.comboBoxReporter.Size = new System.Drawing.Size(201, 21);
            this.comboBoxReporter.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(416, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Priorities";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(209, 13);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Resolutions";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Status";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(363, 306);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Assignee";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 306);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Reporter";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(409, 418);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClear.Location = new System.Drawing.Point(490, 418);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 1;
            this.buttonClear.Text = "Clear Filter";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // textBoxFilterName
            // 
            this.textBoxFilterName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilterName.Location = new System.Drawing.Point(70, 387);
            this.textBoxFilterName.Name = "textBoxFilterName";
            this.textBoxFilterName.Size = new System.Drawing.Size(575, 20);
            this.textBoxFilterName.TabIndex = 0;
            this.textBoxFilterName.TextChanged += new System.EventHandler(this.textBoxFilterName_TextChanged);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 390);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "Filter Name";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(634, 360);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.listViewProjects);
            this.tabPage1.Controls.Add(this.listViewIssueTypes);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(626, 334);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Project / Issue";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.listViewFixForVersions);
            this.tabPage2.Controls.Add(this.listViewAffectsVersions);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.listViewComponents);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(626, 334);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Components / Versions";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.comboBoxReporter);
            this.tabPage3.Controls.Add(this.comboBoxAssignee);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.listViewStatuses);
            this.tabPage3.Controls.Add(this.listViewPriorities);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.listViewResolutions);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(626, 334);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Issue Attributes";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // EditCustomFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 459);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBoxFilterName);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditCustomFilter";
            this.Text = "Edit Local Filter";
            this.Shown += new System.EventHandler(this.editCustomFilterShown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.editCustomFilterKeyPress);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxAssignee;
        private System.Windows.Forms.ComboBox comboBoxReporter;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.ListView listViewIssueTypes;
        private System.Windows.Forms.ListView listViewPriorities;
        private System.Windows.Forms.ListView listViewStatuses;
        private System.Windows.Forms.TextBox textBoxFilterName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListView listViewProjects;
        private System.Windows.Forms.ListView listViewAffectsVersions;
        private System.Windows.Forms.ListView listViewComponents;
        private System.Windows.Forms.ListView listViewResolutions;
        private System.Windows.Forms.ListView listViewFixForVersions;
    }
}
using Atlassian.plvs.ui.jira;

namespace Atlassian.plvs.dialogs.jira {
    partial class CreateIssue {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateIssue));
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.comboProjects = new System.Windows.Forms.ComboBox();
            this.comboTypes = new JiraNamedEntityComboBox();
            this.listComponents = new System.Windows.Forms.ListBox();
            this.listAffectsVersions = new System.Windows.Forms.ListBox();
            this.listFixVersions = new System.Windows.Forms.ListBox();
            this.comboPriorities = new JiraNamedEntityComboBox();
            this.textSummary = new System.Windows.Forms.TextBox();
            this.labelProject = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.jiraAssigneePicker = new JiraUserPicker();
            this.textDescription = new JiraTextAreaWithWikiPreview();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCreateAndClose = new System.Windows.Forms.Button();
            this.labelParentIssueKey = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonCreate
            // 
            this.buttonCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreate.Location = new System.Drawing.Point(288, 632);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(75, 23);
            this.buttonCreate.TabIndex = 9;
            this.buttonCreate.Text = "Create";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(486, 632);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // comboProjects
            // 
            this.comboProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboProjects.FormattingEnabled = true;
            this.comboProjects.Location = new System.Drawing.Point(102, 12);
            this.comboProjects.Name = "comboProjects";
            this.comboProjects.Size = new System.Drawing.Size(459, 21);
            this.comboProjects.TabIndex = 0;
            this.comboProjects.SelectedIndexChanged += new System.EventHandler(this.comboProjects_SelectedIndexChanged);
            // 
            // comboTypes
            // 
            this.comboTypes.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTypes.FormattingEnabled = true;
            this.comboTypes.ImageList = null;
            this.comboTypes.ItemHeight = 16;
            this.comboTypes.Location = new System.Drawing.Point(102, 40);
            this.comboTypes.Name = "comboTypes";
            this.comboTypes.Size = new System.Drawing.Size(459, 22);
            this.comboTypes.TabIndex = 1;
            this.comboTypes.SelectedIndexChanged += new System.EventHandler(this.comboTypes_SelectedIndexChanged);
            // 
            // listComponents
            // 
            this.listComponents.FormattingEnabled = true;
            this.listComponents.Location = new System.Drawing.Point(102, 255);
            this.listComponents.Name = "listComponents";
            this.listComponents.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listComponents.Size = new System.Drawing.Size(459, 95);
            this.listComponents.TabIndex = 2;
            // 
            // listAffectsVersions
            // 
            this.listAffectsVersions.FormattingEnabled = true;
            this.listAffectsVersions.Location = new System.Drawing.Point(102, 356);
            this.listAffectsVersions.Name = "listAffectsVersions";
            this.listAffectsVersions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listAffectsVersions.Size = new System.Drawing.Size(459, 95);
            this.listAffectsVersions.TabIndex = 3;
            // 
            // listFixVersions
            // 
            this.listFixVersions.FormattingEnabled = true;
            this.listFixVersions.Location = new System.Drawing.Point(102, 457);
            this.listFixVersions.Name = "listFixVersions";
            this.listFixVersions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listFixVersions.Size = new System.Drawing.Size(459, 95);
            this.listFixVersions.TabIndex = 4;
            // 
            // comboPriorities
            // 
            this.comboPriorities.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboPriorities.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPriorities.FormattingEnabled = true;
            this.comboPriorities.ImageList = null;
            this.comboPriorities.ItemHeight = 16;
            this.comboPriorities.Location = new System.Drawing.Point(102, 93);
            this.comboPriorities.Name = "comboPriorities";
            this.comboPriorities.Size = new System.Drawing.Size(459, 22);
            this.comboPriorities.TabIndex = 5;
            // 
            // textSummary
            // 
            this.textSummary.Location = new System.Drawing.Point(102, 67);
            this.textSummary.Name = "textSummary";
            this.textSummary.Size = new System.Drawing.Size(459, 20);
            this.textSummary.TabIndex = 6;
            this.textSummary.TextChanged += new System.EventHandler(this.textSummary_TextChanged);
            // 
            // labelProject
            // 
            this.labelProject.AutoSize = true;
            this.labelProject.Location = new System.Drawing.Point(56, 15);
            this.labelProject.Name = "labelProject";
            this.labelProject.Size = new System.Drawing.Size(44, 13);
            this.labelProject.TabIndex = 12;
            this.labelProject.Text = "Project*";
            this.labelProject.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(65, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Type*";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 255);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Component/s";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 356);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Affects Version/s";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 457);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Fix Version/s";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(56, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Priority*";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(46, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Summary*";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(40, 143);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Description";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(46, 564);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Assignee";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // jiraAssigneePicker
            // 
            this.jiraAssigneePicker.Location = new System.Drawing.Point(97, 558);
            this.jiraAssigneePicker.Name = "jiraAssigneePicker";
            this.jiraAssigneePicker.Size = new System.Drawing.Size(458, 51);
            this.jiraAssigneePicker.TabIndex = 21;
            // 
            // textDescription
            // 
            this.textDescription.Facade = null;
            this.textDescription.Issue = null;
            this.textDescription.IssueType = -1;
            this.textDescription.Location = new System.Drawing.Point(102, 121);
            this.textDescription.Name = "textDescription";
            this.textDescription.Project = null;
            this.textDescription.Server = null;
            this.textDescription.Size = new System.Drawing.Size(458, 128);
            this.textDescription.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 637);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "(*) - Asterisk denotes required fields";
            // 
            // buttonCreateAndClose
            // 
            this.buttonCreateAndClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateAndClose.Location = new System.Drawing.Point(369, 632);
            this.buttonCreateAndClose.Name = "buttonCreateAndClose";
            this.buttonCreateAndClose.Size = new System.Drawing.Size(111, 23);
            this.buttonCreateAndClose.TabIndex = 24;
            this.buttonCreateAndClose.Text = "Create And Close";
            this.buttonCreateAndClose.UseVisualStyleBackColor = true;
            this.buttonCreateAndClose.Click += new System.EventHandler(this.buttonCreateAndClose_Click);
            // 
            // labelParentIssueKey
            // 
            this.labelParentIssueKey.AutoSize = true;
            this.labelParentIssueKey.Location = new System.Drawing.Point(102, 15);
            this.labelParentIssueKey.Name = "labelParentIssueKey";
            this.labelParentIssueKey.Size = new System.Drawing.Size(35, 13);
            this.labelParentIssueKey.TabIndex = 25;
            this.labelParentIssueKey.Text = "label2";
            // 
            // CreateIssue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 667);
            this.Controls.Add(this.labelParentIssueKey);
            this.Controls.Add(this.buttonCreateAndClose);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textDescription);
            this.Controls.Add(this.jiraAssigneePicker);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelProject);
            this.Controls.Add(this.textSummary);
            this.Controls.Add(this.comboPriorities);
            this.Controls.Add(this.listFixVersions);
            this.Controls.Add(this.listAffectsVersions);
            this.Controls.Add(this.listComponents);
            this.Controls.Add(this.comboTypes);
            this.Controls.Add(this.comboProjects);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonCreate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateIssue";
            this.Text = "Create JIRA Issue";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.createIssueKeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboProjects;
        private ui.jira.JiraNamedEntityComboBox comboTypes;
        private System.Windows.Forms.ListBox listComponents;
        private System.Windows.Forms.ListBox listAffectsVersions;
        private System.Windows.Forms.ListBox listFixVersions;
        private ui.jira.JiraNamedEntityComboBox comboPriorities;
        private System.Windows.Forms.TextBox textSummary;
        private System.Windows.Forms.Label labelProject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private ui.jira.JiraUserPicker jiraAssigneePicker;
        private global::Atlassian.plvs.ui.jira.JiraTextAreaWithWikiPreview textDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCreateAndClose;
        private System.Windows.Forms.Label labelParentIssueKey;
    }
}
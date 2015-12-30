namespace Atlassian.plvs.dialogs.bamboo {
    partial class AddOrEditBambooServer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddOrEditBambooServer));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAddOrEdit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.TextBox();
            this.url = new System.Windows.Forms.TextBox();
            this.user = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.radioUseFavourites = new System.Windows.Forms.RadioButton();
            this.radioSelectManually = new System.Windows.Forms.RadioButton();
            this.checkedListBuilds = new System.Windows.Forms.CheckedListBox();
            this.buttonGetBuilds = new System.Windows.Forms.Button();
            this.checkEnabled = new System.Windows.Forms.CheckBox();
            this.buttonTestConnection = new System.Windows.Forms.Button();
            this.checkDontUseProxy = new System.Windows.Forms.CheckBox();
            this.checkShared = new System.Windows.Forms.CheckBox();
            this.checkShowBranches = new System.Windows.Forms.CheckBox();
            this.checkMyBranches = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(377, 547);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonAddOrEdit
            // 
            this.buttonAddOrEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddOrEdit.Location = new System.Drawing.Point(296, 547);
            this.buttonAddOrEdit.Name = "buttonAddOrEdit";
            this.buttonAddOrEdit.Size = new System.Drawing.Size(75, 23);
            this.buttonAddOrEdit.TabIndex = 1;
            this.buttonAddOrEdit.Text = "AddOrEdit";
            this.buttonAddOrEdit.UseVisualStyleBackColor = true;
            this.buttonAddOrEdit.Click += new System.EventHandler(this.buttonAddOrEditClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Server Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "User Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Server URL";
            // 
            // name
            // 
            this.name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.name.Location = new System.Drawing.Point(97, 20);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(355, 20);
            this.name.TabIndex = 6;
            this.name.TextChanged += new System.EventHandler(this.nameTextChanged);
            // 
            // url
            // 
            this.url.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.url.Location = new System.Drawing.Point(97, 92);
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(355, 20);
            this.url.TabIndex = 7;
            this.url.TextChanged += new System.EventHandler(this.urlTextChanged);
            // 
            // user
            // 
            this.user.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.user.Location = new System.Drawing.Point(97, 141);
            this.user.Name = "user";
            this.user.Size = new System.Drawing.Size(355, 20);
            this.user.TabIndex = 8;
            this.user.TextChanged += new System.EventHandler(this.userTextChanged);
            // 
            // password
            // 
            this.password.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.password.Location = new System.Drawing.Point(97, 167);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(355, 20);
            this.password.TabIndex = 9;
            // 
            // radioUseFavourites
            // 
            this.radioUseFavourites.AutoSize = true;
            this.radioUseFavourites.Location = new System.Drawing.Point(97, 193);
            this.radioUseFavourites.Name = "radioUseFavourites";
            this.radioUseFavourites.Size = new System.Drawing.Size(136, 17);
            this.radioUseFavourites.TabIndex = 10;
            this.radioUseFavourites.TabStop = true;
            this.radioUseFavourites.Text = "Monitor Favourite Plans";
            this.radioUseFavourites.UseVisualStyleBackColor = true;
            this.radioUseFavourites.CheckedChanged += new System.EventHandler(this.radioUseFavouritesCheckedChanged);
            // 
            // radioSelectManually
            // 
            this.radioSelectManually.AutoSize = true;
            this.radioSelectManually.Location = new System.Drawing.Point(97, 216);
            this.radioSelectManually.Name = "radioSelectManually";
            this.radioSelectManually.Size = new System.Drawing.Size(134, 17);
            this.radioSelectManually.TabIndex = 11;
            this.radioSelectManually.TabStop = true;
            this.radioSelectManually.Text = "Select Plans to Monitor";
            this.radioSelectManually.UseVisualStyleBackColor = true;
            this.radioSelectManually.CheckedChanged += new System.EventHandler(this.radioSelectManuallyCheckedChanged);
            // 
            // checkedListBuilds
            // 
            this.checkedListBuilds.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBuilds.FormattingEnabled = true;
            this.checkedListBuilds.Location = new System.Drawing.Point(97, 249);
            this.checkedListBuilds.Name = "checkedListBuilds";
            this.checkedListBuilds.Size = new System.Drawing.Size(355, 259);
            this.checkedListBuilds.TabIndex = 12;
            // 
            // buttonGetBuilds
            // 
            this.buttonGetBuilds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGetBuilds.Location = new System.Drawing.Point(335, 213);
            this.buttonGetBuilds.Name = "buttonGetBuilds";
            this.buttonGetBuilds.Size = new System.Drawing.Size(117, 23);
            this.buttonGetBuilds.TabIndex = 13;
            this.buttonGetBuilds.Text = "Get Plan List";
            this.buttonGetBuilds.UseVisualStyleBackColor = true;
            this.buttonGetBuilds.Click += new System.EventHandler(this.buttonGetBuildsClick);
            // 
            // checkEnabled
            // 
            this.checkEnabled.AutoSize = true;
            this.checkEnabled.Location = new System.Drawing.Point(97, 46);
            this.checkEnabled.Name = "checkEnabled";
            this.checkEnabled.Size = new System.Drawing.Size(99, 17);
            this.checkEnabled.TabIndex = 14;
            this.checkEnabled.Text = "Server Enabled";
            this.checkEnabled.UseVisualStyleBackColor = true;
            this.checkEnabled.CheckedChanged += new System.EventHandler(this.checkEnabledCheckedChanged);
            // 
            // buttonTestConnection
            // 
            this.buttonTestConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTestConnection.Location = new System.Drawing.Point(97, 547);
            this.buttonTestConnection.Name = "buttonTestConnection";
            this.buttonTestConnection.Size = new System.Drawing.Size(112, 23);
            this.buttonTestConnection.TabIndex = 15;
            this.buttonTestConnection.Text = "Test Connection";
            this.buttonTestConnection.UseVisualStyleBackColor = true;
            this.buttonTestConnection.Click += new System.EventHandler(this.buttonTestConnectionClick);
            // 
            // checkDontUseProxy
            // 
            this.checkDontUseProxy.AutoSize = true;
            this.checkDontUseProxy.Location = new System.Drawing.Point(97, 118);
            this.checkDontUseProxy.Name = "checkDontUseProxy";
            this.checkDontUseProxy.Size = new System.Drawing.Size(111, 17);
            this.checkDontUseProxy.TabIndex = 16;
            this.checkDontUseProxy.Text = "Do Not Use Proxy";
            this.checkDontUseProxy.UseVisualStyleBackColor = true;
            this.checkDontUseProxy.CheckedChanged += new System.EventHandler(this.checkUseProxyCheckedChanged);
            // 
            // checkShared
            // 
            this.checkShared.AutoSize = true;
            this.checkShared.Location = new System.Drawing.Point(97, 69);
            this.checkShared.Name = "checkShared";
            this.checkShared.Size = new System.Drawing.Size(185, 17);
            this.checkShared.TabIndex = 17;
            this.checkShared.Text = "Server Shared Between Solutions";
            this.checkShared.UseVisualStyleBackColor = true;
            this.checkShared.CheckedChanged += new System.EventHandler(this.checkSharedCheckedChanged);
            // 
            // checkShowBranches
            // 
            this.checkShowBranches.AutoSize = true;
            this.checkShowBranches.Location = new System.Drawing.Point(97, 515);
            this.checkShowBranches.Name = "checkShowBranches";
            this.checkShowBranches.Size = new System.Drawing.Size(125, 17);
            this.checkShowBranches.TabIndex = 18;
            this.checkShowBranches.Text = "Show Plan Branches";
            this.checkShowBranches.UseVisualStyleBackColor = true;
            this.checkShowBranches.CheckedChanged += new System.EventHandler(this.checkShowBranchesCheckedChanged);
            // 
            // checkMyBranches
            // 
            this.checkMyBranches.AutoSize = true;
            this.checkMyBranches.Location = new System.Drawing.Point(279, 515);
            this.checkMyBranches.Name = "checkMyBranches";
            this.checkMyBranches.Size = new System.Drawing.Size(112, 17);
            this.checkMyBranches.TabIndex = 19;
            this.checkMyBranches.Text = "Only My Branches";
            this.checkMyBranches.UseVisualStyleBackColor = true;
            this.checkMyBranches.CheckedChanged += new System.EventHandler(this.checkMyBranchesCheckedChanged);
            // 
            // AddOrEditBambooServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 582);
            this.Controls.Add(this.checkMyBranches);
            this.Controls.Add(this.checkShowBranches);
            this.Controls.Add(this.checkShared);
            this.Controls.Add(this.checkDontUseProxy);
            this.Controls.Add(this.buttonTestConnection);
            this.Controls.Add(this.checkEnabled);
            this.Controls.Add(this.buttonGetBuilds);
            this.Controls.Add(this.checkedListBuilds);
            this.Controls.Add(this.radioSelectManually);
            this.Controls.Add(this.radioUseFavourites);
            this.Controls.Add(this.password);
            this.Controls.Add(this.user);
            this.Controls.Add(this.url);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonAddOrEdit);
            this.Controls.Add(this.buttonCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 380);
            this.Name = "AddOrEditBambooServer";
            this.Text = "AddOrEditBambooServer";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.addOrEditServerKeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonAddOrEdit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.TextBox url;
        private System.Windows.Forms.TextBox user;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.RadioButton radioUseFavourites;
        private System.Windows.Forms.RadioButton radioSelectManually;
        private System.Windows.Forms.CheckedListBox checkedListBuilds;
        private System.Windows.Forms.Button buttonGetBuilds;
        private System.Windows.Forms.CheckBox checkEnabled;
        private System.Windows.Forms.Button buttonTestConnection;
        private System.Windows.Forms.CheckBox checkDontUseProxy;
        private System.Windows.Forms.CheckBox checkShared;
        private System.Windows.Forms.CheckBox checkShowBranches;
        private System.Windows.Forms.CheckBox checkMyBranches;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
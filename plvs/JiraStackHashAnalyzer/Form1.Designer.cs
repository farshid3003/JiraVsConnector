namespace JiraStackHashAnalyzer {
    partial class Form1 {
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
            this.components = new System.ComponentModel.Container();
            this.textLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textUrl = new System.Windows.Forms.TextBox();
            this.textUser = new System.Windows.Forms.TextBox();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.buttonGo = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.textServerName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textProject = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonAnalyze = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.textAnalyzeLog = new System.Windows.Forms.TextBox();
            this.textAnalyzeProject = new System.Windows.Forms.TextBox();
            this.textAnalyzeServerName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textAnalyzeDbLog = new System.Windows.Forms.TextBox();
            this.buttonAnalyzeFromDBGo = new System.Windows.Forms.Button();
            this.numericProjectId = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.checkJustDescription = new System.Windows.Forms.CheckBox();
            this.buttonClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericProjectId)).BeginInit();
            this.SuspendLayout();
            // 
            // textLog
            // 
            this.textLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textLog.Location = new System.Drawing.Point(6, 235);
            this.textLog.Multiline = true;
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textLog.Size = new System.Drawing.Size(546, 321);
            this.textLog.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 219);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Log";
            // 
            // textUrl
            // 
            this.textUrl.Location = new System.Drawing.Point(86, 42);
            this.textUrl.Name = "textUrl";
            this.textUrl.Size = new System.Drawing.Size(239, 20);
            this.textUrl.TabIndex = 1;
            // 
            // textUser
            // 
            this.textUser.Location = new System.Drawing.Point(86, 97);
            this.textUser.Name = "textUser";
            this.textUser.Size = new System.Drawing.Size(239, 20);
            this.textUser.TabIndex = 3;
            // 
            // textPassword
            // 
            this.textPassword.Location = new System.Drawing.Point(86, 124);
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '*';
            this.textPassword.Size = new System.Drawing.Size(239, 20);
            this.textPassword.TabIndex = 4;
            // 
            // buttonGo
            // 
            this.buttonGo.Location = new System.Drawing.Point(86, 150);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(75, 23);
            this.buttonGo.TabIndex = 5;
            this.buttonGo.Text = "Go!";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Server Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Password";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Login";
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(86, 179);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 6;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "URL";
            // 
            // textServerName
            // 
            this.textServerName.Location = new System.Drawing.Point(86, 16);
            this.textServerName.Name = "textServerName";
            this.textServerName.Size = new System.Drawing.Size(239, 20);
            this.textServerName.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Project";
            // 
            // textProject
            // 
            this.textProject.Location = new System.Drawing.Point(86, 68);
            this.textProject.Name = "textProject";
            this.textProject.Size = new System.Drawing.Size(239, 20);
            this.textProject.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(566, 588);
            this.tabControl1.TabIndex = 14;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textLog);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.textProject);
            this.tabPage1.Controls.Add(this.textUrl);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.textUser);
            this.tabPage1.Controls.Add(this.textServerName);
            this.tabPage1.Controls.Add(this.textPassword);
            this.tabPage1.Controls.Add(this.buttonStop);
            this.tabPage1.Controls.Add(this.buttonGo);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(558, 562);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Collect";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.buttonAnalyze);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.textAnalyzeLog);
            this.tabPage2.Controls.Add(this.textAnalyzeProject);
            this.tabPage2.Controls.Add(this.textAnalyzeServerName);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(558, 562);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Analyze";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonAnalyze
            // 
            this.buttonAnalyze.Location = new System.Drawing.Point(84, 75);
            this.buttonAnalyze.Name = "buttonAnalyze";
            this.buttonAnalyze.Size = new System.Drawing.Size(75, 23);
            this.buttonAnalyze.TabIndex = 6;
            this.buttonAnalyze.Text = "Analyze!";
            this.buttonAnalyze.UseVisualStyleBackColor = true;
            this.buttonAnalyze.Click += new System.EventHandler(this.buttonAnalyze_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 110);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Results";
            // 
            // textAnalyzeLog
            // 
            this.textAnalyzeLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textAnalyzeLog.Location = new System.Drawing.Point(12, 126);
            this.textAnalyzeLog.Multiline = true;
            this.textAnalyzeLog.Name = "textAnalyzeLog";
            this.textAnalyzeLog.ReadOnly = true;
            this.textAnalyzeLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textAnalyzeLog.Size = new System.Drawing.Size(538, 428);
            this.textAnalyzeLog.TabIndex = 4;
            // 
            // textAnalyzeProject
            // 
            this.textAnalyzeProject.Location = new System.Drawing.Point(84, 38);
            this.textAnalyzeProject.Name = "textAnalyzeProject";
            this.textAnalyzeProject.Size = new System.Drawing.Size(100, 20);
            this.textAnalyzeProject.TabIndex = 3;
            // 
            // textAnalyzeServerName
            // 
            this.textAnalyzeServerName.Location = new System.Drawing.Point(84, 14);
            this.textAnalyzeServerName.Name = "textAnalyzeServerName";
            this.textAnalyzeServerName.Size = new System.Drawing.Size(100, 20);
            this.textAnalyzeServerName.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Project";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Server Name";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.buttonClear);
            this.tabPage3.Controls.Add(this.checkJustDescription);
            this.tabPage3.Controls.Add(this.textAnalyzeDbLog);
            this.tabPage3.Controls.Add(this.buttonAnalyzeFromDBGo);
            this.tabPage3.Controls.Add(this.numericProjectId);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(558, 562);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Analyze by DB";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // textAnalyzeDbLog
            // 
            this.textAnalyzeDbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textAnalyzeDbLog.Location = new System.Drawing.Point(8, 112);
            this.textAnalyzeDbLog.Multiline = true;
            this.textAnalyzeDbLog.Name = "textAnalyzeDbLog";
            this.textAnalyzeDbLog.ReadOnly = true;
            this.textAnalyzeDbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textAnalyzeDbLog.Size = new System.Drawing.Size(542, 442);
            this.textAnalyzeDbLog.TabIndex = 3;
            // 
            // buttonAnalyzeFromDBGo
            // 
            this.buttonAnalyzeFromDBGo.Location = new System.Drawing.Point(68, 53);
            this.buttonAnalyzeFromDBGo.Name = "buttonAnalyzeFromDBGo";
            this.buttonAnalyzeFromDBGo.Size = new System.Drawing.Size(75, 23);
            this.buttonAnalyzeFromDBGo.TabIndex = 2;
            this.buttonAnalyzeFromDBGo.Text = "Analyze!";
            this.buttonAnalyzeFromDBGo.UseVisualStyleBackColor = true;
            this.buttonAnalyzeFromDBGo.Click += new System.EventHandler(this.buttonAnalyzeFromDbGoClick);
            // 
            // numericProjectId
            // 
            this.numericProjectId.Location = new System.Drawing.Point(68, 27);
            this.numericProjectId.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericProjectId.Name = "numericProjectId";
            this.numericProjectId.Size = new System.Drawing.Size(120, 20);
            this.numericProjectId.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Project ID";
            // 
            // checkJustDescription
            // 
            this.checkJustDescription.AutoSize = true;
            this.checkJustDescription.Location = new System.Drawing.Point(209, 57);
            this.checkJustDescription.Name = "checkJustDescription";
            this.checkJustDescription.Size = new System.Drawing.Size(101, 17);
            this.checkJustDescription.TabIndex = 4;
            this.checkJustDescription.Text = "Just Description";
            this.checkJustDescription.UseVisualStyleBackColor = true;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(458, 81);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 5;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 588);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "JIRA Duplicate Analyzer";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericProjectId)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textUrl;
        private System.Windows.Forms.TextBox textUser;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.BindingSource bindingSource;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textServerName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textProject;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button buttonAnalyze;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textAnalyzeLog;
        private System.Windows.Forms.TextBox textAnalyzeProject;
        private System.Windows.Forms.TextBox textAnalyzeServerName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox textAnalyzeDbLog;
        private System.Windows.Forms.Button buttonAnalyzeFromDBGo;
        private System.Windows.Forms.NumericUpDown numericProjectId;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox checkJustDescription;
        private System.Windows.Forms.Button buttonClear;
    }
}


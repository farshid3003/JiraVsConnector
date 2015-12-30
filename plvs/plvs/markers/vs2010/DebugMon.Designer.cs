namespace Atlassian.plvs.markers.vs2010 {
    partial class DebugMon {
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
            this.textDebug = new System.Windows.Forms.TextBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textDebug
            // 
            this.textDebug.Location = new System.Drawing.Point(13, 13);
            this.textDebug.Multiline = true;
            this.textDebug.Name = "textDebug";
            this.textDebug.ReadOnly = true;
            this.textDebug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDebug.Size = new System.Drawing.Size(691, 377);
            this.textDebug.TabIndex = 0;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(314, 408);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 1;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // DebugMon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 448);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.textDebug);
            this.Name = "DebugMon";
            this.Text = "DebugMon";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textDebug;
        private System.Windows.Forms.Button buttonClear;
    }
}

namespace ATMProject
{
    partial class Management
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.stripBtnCreateATM = new System.Windows.Forms.ToolStripMenuItem();
            this.stripOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.stripExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripBtnCreateATM,
            this.stripOptions,
            this.stripExit});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(460, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // stripBtnCreateATM
            // 
            this.stripBtnCreateATM.Name = "stripBtnCreateATM";
            this.stripBtnCreateATM.Size = new System.Drawing.Size(80, 20);
            this.stripBtnCreateATM.Text = "Create ATM";
            this.stripBtnCreateATM.Click += new System.EventHandler(this.stripBtnCreateATM_Click);
            // 
            // stripOptions
            // 
            this.stripOptions.Name = "stripOptions";
            this.stripOptions.Size = new System.Drawing.Size(61, 20);
            this.stripOptions.Text = "Options";
            this.stripOptions.Click += new System.EventHandler(this.stripOptions_Click);
            // 
            // stripExit
            // 
            this.stripExit.Name = "stripExit";
            this.stripExit.Size = new System.Drawing.Size(38, 20);
            this.stripExit.Text = "Exit";
            this.stripExit.Click += new System.EventHandler(this.stripExit_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 450);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Account Management System";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem stripOptions;
        private System.Windows.Forms.ToolStripMenuItem stripBtnCreateATM;
        private System.Windows.Forms.ToolStripMenuItem stripExit;
    }
}


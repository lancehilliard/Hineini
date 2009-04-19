using System.Drawing;
using System.Windows.Forms;

namespace Hineini {
    partial class MessagesForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.backMenuItem = new System.Windows.Forms.MenuItem();
            this.messagesLabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.backMenuItem);
            // 
            // backMenuItem
            // 
            this.backMenuItem.Text = "Back";
            this.backMenuItem.Click += new System.EventHandler(this.backMenuItem_Click);
            // 
            // messagesLabel
            // 
            this.messagesLabel.Dock = DockStyle.Fill;
            this.messagesLabel.Font = new System.Drawing.Font("Segoe Condensed", 8F, FontStyle.Regular);
            this.messagesLabel.Location = new System.Drawing.Point(0, 0);
            this.messagesLabel.Name = "messagesLabel";
            this.messagesLabel.Size = new System.Drawing.Size(240, 266);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MessagesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 266);
            this.Controls.Add(this.messagesLabel);
            this.Menu = this.mainMenu1;
            this.Name = "MessagesForm";
            this.Text = "Hineini";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem backMenuItem;
        private System.Windows.Forms.Label messagesLabel;
        private System.Windows.Forms.Timer timer1;
    }
}
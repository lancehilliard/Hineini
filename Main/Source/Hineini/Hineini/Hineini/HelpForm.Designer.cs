namespace Hineini {
    partial class HelpForm {
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
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
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(240, 266);
            // 
            // HelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 266);
            this.Controls.Add(this.webBrowser1);
            this.Font = new System.Drawing.Font("Segoe Condensed", 8F, System.Drawing.FontStyle.Regular);
            this.Menu = this.mainMenu1;
            this.Name = "HelpForm";
            this.Text = "Hineini";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem backMenuItem;
        private System.Windows.Forms.WebBrowser webBrowser1;
    }
}
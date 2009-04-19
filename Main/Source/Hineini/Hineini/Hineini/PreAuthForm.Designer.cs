namespace Hineini {
    partial class PreAuthForm {
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
            this.FormLabel = new System.Windows.Forms.Label();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.exitMenuItem = new System.Windows.Forms.MenuItem();
            this.timer1 = new System.Windows.Forms.Timer();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.exitMenuItem);
            // 
            // FormLabel
            // 
            this.FormLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FormLabel.Font = new System.Drawing.Font("Segoe Condensed", 10F, System.Drawing.FontStyle.Regular);
            this.FormLabel.Location = new System.Drawing.Point(0, 0);
            this.FormLabel.Name = "FormLabel";
            this.FormLabel.Size = new System.Drawing.Size(240, 266);
            // 
            // menuItem1
            // 
            this.menuItem1.Enabled = false;
            this.menuItem1.Text = "";
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // PreAuthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 266);
            this.Controls.Add(this.FormLabel);
            this.Menu = this.mainMenu1;
            this.Name = "PreAuthForm";
            this.Text = "Hineini";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label FormLabel;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem exitMenuItem;
        private System.Windows.Forms.Timer timer1;
    }
}
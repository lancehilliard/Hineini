namespace Hineini {
    partial class TagForm {
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
            this.tagPictureBox = new System.Windows.Forms.PictureBox();
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
            // tagPictureBox
            // 
            this.tagPictureBox.Location = new System.Drawing.Point(0, 0);
            this.tagPictureBox.Name = "tagPictureBox";
            this.tagPictureBox.Size = new System.Drawing.Size(136, 136);
            // 
            // TagForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 266);
            this.Controls.Add(this.tagPictureBox);
            this.Menu = this.mainMenu1;
            this.Name = "TagForm";
            this.Text = "Hineini";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox tagPictureBox;
        private System.Windows.Forms.MenuItem backMenuItem;
    }
}
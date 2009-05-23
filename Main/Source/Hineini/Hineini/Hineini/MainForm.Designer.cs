using System;
using System.Drawing;
using System.Windows.Forms;
using Hineini.Utility;

namespace Hineini {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem mainMenu;
        private System.Windows.Forms.MenuItem locateViaMenuItem;
        private System.Windows.Forms.MenuItem exitMenuItem;
        private System.Windows.Forms.MenuItem updateIntervalMenuItem;
        private System.Windows.Forms.MenuItem oneMinuteMenuItem;
        private System.Windows.Forms.MenuItem fiveMinutesMenuItem;
        private System.Windows.Forms.MenuItem fifteenMinutesMenuItem;
        private System.Windows.Forms.MenuItem thirtyMinutesMenuItem;
        private System.Windows.Forms.MenuItem sixtyMinutesMenuItem;
        private System.Windows.Forms.MenuItem towerLocationsMenuItem;
        private System.Windows.Forms.MenuItem yahooAlwaysMenuItem;
        private System.Windows.Forms.MenuItem googleSometimesMenuItem;
        private System.Windows.Forms.MenuItem googleAlwaysMenuItem;
        private System.Windows.Forms.MenuItem gpsOnlyMenuItem;
        private System.Windows.Forms.MenuItem towersSometimesMenuItem;
        private System.Windows.Forms.MenuItem towersOnlyMenuItem;
        private System.Windows.Forms.MenuItem manuallyMenuItem;
        private System.Windows.Forms.MenuItem backlightMenuItem;
        private System.Windows.Forms.MenuItem systemManagedMenuItem;
        private System.Windows.Forms.MenuItem alwaysOnMenuItem;
        private System.Windows.Forms.MenuItem accountSetupMenuItem;
        private System.Windows.Forms.MenuItem undoHineiniAuthorizationMenuItem;
        private System.Windows.Forms.MenuItem UndoHineiniAuthorizationConfirmMenuItem;

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
            this.mainMenu = new System.Windows.Forms.MenuItem();
            this.locateViaMenuItem = new System.Windows.Forms.MenuItem();
            this.gpsOnlyMenuItem = new System.Windows.Forms.MenuItem();
            this.towersSometimesMenuItem = new System.Windows.Forms.MenuItem();
            this.towersOnlyMenuItem = new System.Windows.Forms.MenuItem();
            this.gpsStationaryThresholdMenuItem = new System.Windows.Forms.MenuItem();
            this.noneMenuItem = new System.Windows.Forms.MenuItem();
            this.quarterMileMenuItem = new System.Windows.Forms.MenuItem();
            this.halfMileMenuItem = new System.Windows.Forms.MenuItem();
            this.oneMileMenuItem = new System.Windows.Forms.MenuItem();
            this.updateIntervalMenuItem = new System.Windows.Forms.MenuItem();
            this.oneMinuteMenuItem = new System.Windows.Forms.MenuItem();
            this.fiveMinutesMenuItem = new System.Windows.Forms.MenuItem();
            this.fifteenMinutesMenuItem = new System.Windows.Forms.MenuItem();
            this.thirtyMinutesMenuItem = new System.Windows.Forms.MenuItem();
            this.sixtyMinutesMenuItem = new System.Windows.Forms.MenuItem();
            this.manuallyMenuItem = new System.Windows.Forms.MenuItem();
            this.towerLocationsMenuItem = new System.Windows.Forms.MenuItem();
            this.yahooAlwaysMenuItem = new System.Windows.Forms.MenuItem();
            this.googleSometimesMenuItem = new System.Windows.Forms.MenuItem();
            this.googleAlwaysMenuItem = new System.Windows.Forms.MenuItem();
            this.backlightMenuItem = new System.Windows.Forms.MenuItem();
            this.systemManagedMenuItem = new System.Windows.Forms.MenuItem();
            this.alwaysOnMenuItem = new System.Windows.Forms.MenuItem();
            this.accountSetupMenuItem = new System.Windows.Forms.MenuItem();
            this.undoHineiniAuthorizationMenuItem = new System.Windows.Forms.MenuItem();
            this.UndoHineiniAuthorizationConfirmMenuItem = new System.Windows.Forms.MenuItem();
            this.mobileWebsiteMenuItem = new System.Windows.Forms.MenuItem();
            this.logsMenuItem = new System.Windows.Forms.MenuItem();
            this.infoMenuItem = new System.Windows.Forms.MenuItem();
            this.errorMenuItem = new System.Windows.Forms.MenuItem();
            this.helpMenuItem = new System.Windows.Forms.MenuItem();
            this.userManualMenuItem = new System.Windows.Forms.MenuItem();
            this.aboutMenuItem = new System.Windows.Forms.MenuItem();
            this.exitMenuItem = new System.Windows.Forms.MenuItem();
            this.timer1 = new System.Windows.Forms.Timer();
            this.updatePanel = new System.Windows.Forms.Panel();
            this.updateTextBox = new System.Windows.Forms.TextBox();
            this.leftMostUpdateBorderPanel = new System.Windows.Forms.Panel();
            this.justLeftOfTextBoxUpdateBorderPanel = new System.Windows.Forms.Panel();
            this.betweenTextBoxAndLinkLabelBorderPanel = new System.Windows.Forms.Panel();
            this.updateLinkLabel = new System.Windows.Forms.LinkLabel();
            this.rightMostUpdateBorderPanel = new System.Windows.Forms.Panel();
            this.locationPicturePanel = new System.Windows.Forms.Panel();
            this.mostRecentInfoMessageLabel = new System.Windows.Forms.Label();
            this.locationPictureBox = new System.Windows.Forms.PictureBox();
            this.showTagMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.whatIsTagMenuItem = new System.Windows.Forms.MenuItem();
            this.updatePanel.SuspendLayout();
            this.leftMostUpdateBorderPanel.SuspendLayout();
            this.locationPicturePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.mainMenu);
            this.mainMenu1.MenuItems.Add(this.exitMenuItem);
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.Add(this.locateViaMenuItem);
            this.mainMenu.MenuItems.Add(this.updateIntervalMenuItem);
            this.mainMenu.MenuItems.Add(this.towerLocationsMenuItem);
            this.mainMenu.MenuItems.Add(this.backlightMenuItem);
            this.mainMenu.MenuItems.Add(this.accountSetupMenuItem);
            this.mainMenu.MenuItems.Add(this.logsMenuItem);
            this.mainMenu.MenuItems.Add(this.helpMenuItem);
            this.mainMenu.Text = "Menu";
            // 
            // locateViaMenuItem
            // 
            this.locateViaMenuItem.MenuItems.Add(this.gpsOnlyMenuItem);
            this.locateViaMenuItem.MenuItems.Add(this.towersSometimesMenuItem);
            this.locateViaMenuItem.MenuItems.Add(this.towersOnlyMenuItem);
            this.locateViaMenuItem.MenuItems.Add(this.gpsStationaryThresholdMenuItem);
            this.locateViaMenuItem.Text = "Locate Via";
            // 
            // gpsOnlyMenuItem
            // 
            this.gpsOnlyMenuItem.Text = "GPS only";
            this.gpsOnlyMenuItem.Click += new System.EventHandler(this.gpsOnlyMenuItem_Click);
            // 
            // towersSometimesMenuItem
            // 
            this.towersSometimesMenuItem.Text = "Cell Towers when GPS fails";
            this.towersSometimesMenuItem.Click += new System.EventHandler(this.towersSometimesMenuItem_Click);
            // 
            // towersOnlyMenuItem
            // 
            this.towersOnlyMenuItem.Text = "Cell Towers only";
            this.towersOnlyMenuItem.Click += new System.EventHandler(this.towersOnlyMenuItem_Click);
            // 
            // gpsStationaryThresholdMenuItem
            // 
            this.gpsStationaryThresholdMenuItem.MenuItems.Add(this.noneMenuItem);
            this.gpsStationaryThresholdMenuItem.MenuItems.Add(this.quarterMileMenuItem);
            this.gpsStationaryThresholdMenuItem.MenuItems.Add(this.halfMileMenuItem);
            this.gpsStationaryThresholdMenuItem.MenuItems.Add(this.oneMileMenuItem);
            this.gpsStationaryThresholdMenuItem.Text = "GPS Stationary Threshold";
            // 
            // noneMenuItem
            // 
            this.noneMenuItem.Text = "None";
            this.noneMenuItem.Click += new System.EventHandler(this.noneMenuItem_Click);
            // 
            // quarterMileMenuItem
            // 
            this.quarterMileMenuItem.Text = "1/4 mile";
            this.quarterMileMenuItem.Click += new System.EventHandler(this.quarterMileMenuItem_Click);
            // 
            // halfMileMenuItem
            // 
            this.halfMileMenuItem.Text = "1/2 mile";
            this.halfMileMenuItem.Click += new System.EventHandler(this.halfMileMenuItem_Click);
            // 
            // oneMileMenuItem
            // 
            this.oneMileMenuItem.Text = "1 mile";
            this.oneMileMenuItem.Click += new System.EventHandler(this.oneMileMenuItem_Click);
            // 
            // updateIntervalMenuItem
            // 
            this.updateIntervalMenuItem.MenuItems.Add(this.oneMinuteMenuItem);
            this.updateIntervalMenuItem.MenuItems.Add(this.fiveMinutesMenuItem);
            this.updateIntervalMenuItem.MenuItems.Add(this.fifteenMinutesMenuItem);
            this.updateIntervalMenuItem.MenuItems.Add(this.thirtyMinutesMenuItem);
            this.updateIntervalMenuItem.MenuItems.Add(this.sixtyMinutesMenuItem);
            this.updateIntervalMenuItem.MenuItems.Add(this.manuallyMenuItem);
            this.updateIntervalMenuItem.Text = "Update Interval";
            // 
            // oneMinuteMenuItem
            // 
            this.oneMinuteMenuItem.Text = "1 Minute";
            this.oneMinuteMenuItem.Click += new System.EventHandler(this.oneMinuteMenuItem_Click);
            // 
            // fiveMinutesMenuItem
            // 
            this.fiveMinutesMenuItem.Text = "5 Minutes";
            this.fiveMinutesMenuItem.Click += new System.EventHandler(this.fiveMinutesMenuItem_Click);
            // 
            // fifteenMinutesMenuItem
            // 
            this.fifteenMinutesMenuItem.Text = "15 Minutes";
            this.fifteenMinutesMenuItem.Click += new System.EventHandler(this.fifteenMinutesMenuItem_Click);
            // 
            // thirtyMinutesMenuItem
            // 
            this.thirtyMinutesMenuItem.Text = "30 Minutes";
            this.thirtyMinutesMenuItem.Click += new System.EventHandler(this.thirtyMinutesMenuItem_Click);
            // 
            // sixtyMinutesMenuItem
            // 
            this.sixtyMinutesMenuItem.Text = "60 Minutes";
            this.sixtyMinutesMenuItem.Click += new System.EventHandler(this.sixtyMinutesMenuItem_Click);
            // 
            // manuallyMenuItem
            // 
            this.manuallyMenuItem.Text = "Manually";
            this.manuallyMenuItem.Click += new System.EventHandler(this.manuallyMenuItem_Click);
            // 
            // towerLocationsMenuItem
            // 
            this.towerLocationsMenuItem.MenuItems.Add(this.yahooAlwaysMenuItem);
            this.towerLocationsMenuItem.MenuItems.Add(this.googleSometimesMenuItem);
            this.towerLocationsMenuItem.MenuItems.Add(this.googleAlwaysMenuItem);
            this.towerLocationsMenuItem.Text = "Cell Tower Locations";
            // 
            // yahooAlwaysMenuItem
            // 
            this.yahooAlwaysMenuItem.Text = "Yahoo always";
            this.yahooAlwaysMenuItem.Click += new System.EventHandler(this.yahooAlwaysMenuItem_Click);
            // 
            // googleSometimesMenuItem
            // 
            this.googleSometimesMenuItem.Text = "Google when Yahoo fails";
            this.googleSometimesMenuItem.Click += new System.EventHandler(this.googleSometimesMenuItem_Click);
            // 
            // googleAlwaysMenuItem
            // 
            this.googleAlwaysMenuItem.Text = "Google always";
            this.googleAlwaysMenuItem.Click += new System.EventHandler(this.googleAlwaysMenuItem_Click);
            // 
            // backlightMenuItem
            // 
            this.backlightMenuItem.MenuItems.Add(this.systemManagedMenuItem);
            this.backlightMenuItem.MenuItems.Add(this.alwaysOnMenuItem);
            this.backlightMenuItem.Text = "Backlight";
            // 
            // systemManagedMenuItem
            // 
            this.systemManagedMenuItem.Text = "Use System Setting";
            this.systemManagedMenuItem.Click += new System.EventHandler(this.systemManagedMenuItem_Click);
            // 
            // alwaysOnMenuItem
            // 
            this.alwaysOnMenuItem.Text = "Always On";
            this.alwaysOnMenuItem.Click += new System.EventHandler(this.alwaysOnMenuItem_Click);
            // 
            // accountSetupMenuItem
            // 
            this.accountSetupMenuItem.MenuItems.Add(this.undoHineiniAuthorizationMenuItem);
            this.accountSetupMenuItem.MenuItems.Add(this.mobileWebsiteMenuItem);
            this.accountSetupMenuItem.Text = "Fire Eagle";
            // 
            // undoHineiniAuthorizationMenuItem
            // 
            this.undoHineiniAuthorizationMenuItem.MenuItems.Add(this.UndoHineiniAuthorizationConfirmMenuItem);
            this.undoHineiniAuthorizationMenuItem.Text = "Revoke Authorization";
            // 
            // UndoHineiniAuthorizationConfirmMenuItem
            // 
            this.UndoHineiniAuthorizationConfirmMenuItem.Text = "Yes, I\'m sure.";
            this.UndoHineiniAuthorizationConfirmMenuItem.Click += new System.EventHandler(this.UndoHineiniAuthorizationConfirmMenuItem_Click);
            // 
            // mobileWebsiteMenuItem
            // 
            this.mobileWebsiteMenuItem.Text = "Mobile Website";
            this.mobileWebsiteMenuItem.Click += new System.EventHandler(this.mobileWebsiteMenuItem_Click);
            // 
            // logsMenuItem
            // 
            this.logsMenuItem.MenuItems.Add(this.infoMenuItem);
            this.logsMenuItem.MenuItems.Add(this.errorMenuItem);
            this.logsMenuItem.Text = "Logs";
            // 
            // infoMenuItem
            // 
            this.infoMenuItem.Text = "Info";
            this.infoMenuItem.Click += new System.EventHandler(this.infoMenuItem_Click);
            // 
            // errorMenuItem
            // 
            this.errorMenuItem.Text = "Error";
            this.errorMenuItem.Click += new System.EventHandler(this.errorMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.MenuItems.Add(this.aboutMenuItem);
            this.helpMenuItem.MenuItems.Add(this.userManualMenuItem);
            this.helpMenuItem.MenuItems.Add(this.menuItem1);
            this.helpMenuItem.Text = "Help";
            // 
            // userManualMenuItem
            // 
            this.userManualMenuItem.Text = "User Manual";
            this.userManualMenuItem.Click += new System.EventHandler(this.UserManualMenuItem_Click);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Text = "About";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // updatePanel
            // 
            this.updatePanel.Controls.Add(this.updateTextBox);
            this.updatePanel.Controls.Add(this.leftMostUpdateBorderPanel);
            this.updatePanel.Controls.Add(this.betweenTextBoxAndLinkLabelBorderPanel);
            this.updatePanel.Controls.Add(this.updateLinkLabel);
            this.updatePanel.Controls.Add(this.rightMostUpdateBorderPanel);
            this.updatePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.updatePanel.Location = new System.Drawing.Point(0, 0);
            this.updatePanel.Name = "updatePanel";
            this.updatePanel.Size = new System.Drawing.Size(240, 21);
            // 
            // updateTextBox
            // 
            this.updateTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updateTextBox.Font = new System.Drawing.Font("Segoe Condensed", 8F, System.Drawing.FontStyle.Regular);
            this.updateTextBox.Location = new System.Drawing.Point(2, 0);
            this.updateTextBox.Name = "updateTextBox";
            this.updateTextBox.Size = new System.Drawing.Size(184, 25);
            this.updateTextBox.TabIndex = 0;
            this.updateTextBox.TextChanged += new System.EventHandler(this.updateTextBox_TextChanged);
            // 
            // leftMostUpdateBorderPanel
            // 
            this.leftMostUpdateBorderPanel.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.leftMostUpdateBorderPanel.Controls.Add(this.justLeftOfTextBoxUpdateBorderPanel);
            this.leftMostUpdateBorderPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftMostUpdateBorderPanel.Location = new System.Drawing.Point(0, 0);
            this.leftMostUpdateBorderPanel.Name = "leftMostUpdateBorderPanel";
            this.leftMostUpdateBorderPanel.Size = new System.Drawing.Size(2, 21);
            // 
            // justLeftOfTextBoxUpdateBorderPanel
            // 
            this.justLeftOfTextBoxUpdateBorderPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.justLeftOfTextBoxUpdateBorderPanel.Location = new System.Drawing.Point(1, 0);
            this.justLeftOfTextBoxUpdateBorderPanel.Name = "justLeftOfTextBoxUpdateBorderPanel";
            this.justLeftOfTextBoxUpdateBorderPanel.Size = new System.Drawing.Size(1, 21);
            // 
            // betweenTextBoxAndLinkLabelBorderPanel
            // 
            this.betweenTextBoxAndLinkLabelBorderPanel.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.betweenTextBoxAndLinkLabelBorderPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.betweenTextBoxAndLinkLabelBorderPanel.Location = new System.Drawing.Point(186, 0);
            this.betweenTextBoxAndLinkLabelBorderPanel.Name = "betweenTextBoxAndLinkLabelBorderPanel";
            this.betweenTextBoxAndLinkLabelBorderPanel.Size = new System.Drawing.Size(1, 21);
            // 
            // updateLinkLabel
            // 
            this.updateLinkLabel.BackColor = System.Drawing.SystemColors.Control;
            this.updateLinkLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.updateLinkLabel.Font = new System.Drawing.Font("Segoe Condensed", 9F, System.Drawing.FontStyle.Underline);
            this.updateLinkLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.updateLinkLabel.Location = new System.Drawing.Point(187, 0);
            this.updateLinkLabel.Name = "updateLinkLabel";
            this.updateLinkLabel.Size = new System.Drawing.Size(52, 21);
            this.updateLinkLabel.TabIndex = 2;
            this.updateLinkLabel.Text = "Update";
            this.updateLinkLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.updateLinkLabel.Click += new System.EventHandler(this.updateLinkLabel_Click);
            // 
            // rightMostUpdateBorderPanel
            // 
            this.rightMostUpdateBorderPanel.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.rightMostUpdateBorderPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightMostUpdateBorderPanel.Location = new System.Drawing.Point(239, 0);
            this.rightMostUpdateBorderPanel.Name = "rightMostUpdateBorderPanel";
            this.rightMostUpdateBorderPanel.Size = new System.Drawing.Size(1, 21);
            // 
            // locationPicturePanel
            // 
            this.locationPicturePanel.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.locationPicturePanel.Controls.Add(this.mostRecentInfoMessageLabel);
            this.locationPicturePanel.Controls.Add(this.locationPictureBox);
            this.locationPicturePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locationPicturePanel.Location = new System.Drawing.Point(0, 21);
            this.locationPicturePanel.Name = "locationPicturePanel";
            this.locationPicturePanel.Size = new System.Drawing.Size(240, 245);
            this.locationPicturePanel.Resize += new System.EventHandler(this.locationPicturePanel_Resize);
            // 
            // mostRecentInfoMessageLabel
            // 
            this.mostRecentInfoMessageLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
            this.mostRecentInfoMessageLabel.Font = new System.Drawing.Font("Segoe Condensed", 8F, System.Drawing.FontStyle.Regular);
            this.mostRecentInfoMessageLabel.Location = new System.Drawing.Point(-1, 227);
            this.mostRecentInfoMessageLabel.Name = "mostRecentInfoMessageLabel";
            this.mostRecentInfoMessageLabel.Size = new System.Drawing.Size(240, 18);
            this.mostRecentInfoMessageLabel.Text = "Loading Hineini v0.5";
            // 
            // locationPictureBox
            // 
            this.locationPictureBox.BackColor = System.Drawing.Color.White;
            this.locationPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locationPictureBox.Location = new System.Drawing.Point(0, 0);
            this.locationPictureBox.Name = "locationPictureBox";
            this.locationPictureBox.Size = new System.Drawing.Size(240, 245);
            this.locationPictureBox.Resize += new System.EventHandler(this.locationPictureBox_Resize);
            // 
            // showTagMenuItem
            // 
            this.showTagMenuItem.Text = "Show Tag";
            this.showTagMenuItem.Click += new System.EventHandler(this.showTagMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.MenuItems.Add(this.showTagMenuItem);
            this.menuItem1.MenuItems.Add(this.whatIsTagMenuItem);
            this.menuItem1.Text = "Microsoft Tag";
            // 
            // whatIsTagMenuItem
            // 
            this.whatIsTagMenuItem.Text = "What\'s a \"Tag\"?";
            this.whatIsTagMenuItem.Click += new System.EventHandler(this.whatIsTagMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 266);
            this.Controls.Add(this.locationPicturePanel);
            this.Controls.Add(this.updatePanel);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.Text = "Hineini";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.updatePanel.ResumeLayout(false);
            this.leftMostUpdateBorderPanel.ResumeLayout(false);
            this.locationPicturePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private Panel updatePanel;
        private MenuItem logsMenuItem;
        private MenuItem infoMenuItem;
        private MenuItem errorMenuItem;
        private TextBox updateTextBox;
        private LinkLabel updateLinkLabel;
        private MenuItem helpMenuItem;
        private MenuItem userManualMenuItem;
        private Panel locationPicturePanel;
        private PictureBox locationPictureBox;
        private Panel betweenTextBoxAndLinkLabelBorderPanel;
        private Panel leftMostUpdateBorderPanel;
        private Panel rightMostUpdateBorderPanel;
        private Panel justLeftOfTextBoxUpdateBorderPanel;
        private MenuItem mobileWebsiteMenuItem;
        private MenuItem gpsStationaryThresholdMenuItem;
        private MenuItem quarterMileMenuItem;
        private MenuItem halfMileMenuItem;
        private MenuItem oneMileMenuItem;
        private MenuItem noneMenuItem;
        private Label mostRecentInfoMessageLabel;
        private MenuItem aboutMenuItem;
        private MenuItem showTagMenuItem;
        private MenuItem menuItem1;
        private MenuItem whatIsTagMenuItem;
    }
}


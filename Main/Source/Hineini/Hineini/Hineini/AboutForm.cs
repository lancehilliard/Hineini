using System;
using System.IO;
using System.Windows.Forms;

namespace Hineini {
    public partial class AboutForm : Form {
        public AboutForm() {
            InitializeComponent();
            string aboutPagePath = MainUtility.GetWorkingDirectoryFileName("About.html");
            using (StreamReader streamReader = File.OpenText(aboutPagePath)) {
                webBrowser1.DocumentText = streamReader.ReadToEnd();
            }
        }

        public event EventHandler HideForm;

        private void backMenuItem_Click(object sender, EventArgs e) {
            if (HideForm != null) {
                HideForm(sender, e);
            }
        }
    }
}
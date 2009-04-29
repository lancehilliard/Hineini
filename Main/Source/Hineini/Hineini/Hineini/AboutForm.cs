using System;
using System.IO;
using System.Windows.Forms;

namespace Hineini {
    public partial class AboutForm : Form {
        readonly string aboutPagePath;
        public AboutForm() {
            InitializeComponent();
            aboutPagePath = MainUtility.GetWorkingDirectoryFileName("About.html");
        }

        public event EventHandler HideForm;

        private void backMenuItem_Click(object sender, EventArgs e) {
            if (HideForm != null) {
                HideForm(sender, e);
            }
            if (webBrowser1 != null) {
                webBrowser1.DocumentText = null;
            }
        }

        public void ResetAndShow() {
            using (StreamReader streamReader = File.OpenText(aboutPagePath)) {
                webBrowser1.DocumentText = streamReader.ReadToEnd();
            }
            Show();
        }

    }
}
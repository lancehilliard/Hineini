using System;
using System.IO;
using System.Windows.Forms;

namespace Hineini {
    public partial class HelpForm : Form {
        public event EventHandler HideForm;

        public HelpForm() {
            InitializeComponent();
            string userManualPath = MainUtility.GetWorkingDirectoryFileName("UserManual.html");
            using (StreamReader streamReader = File.OpenText(userManualPath)) {
                webBrowser1.DocumentText = streamReader.ReadToEnd();
            }
        }

        private void backMenuItem_Click(object sender, EventArgs e) {
            if (HideForm != null) {
                HideForm(sender, e);
            }
        }
    }
}
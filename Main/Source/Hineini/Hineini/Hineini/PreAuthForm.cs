using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Hineini {
    public partial class PreAuthForm : Form {
        public event EventHandler Exit;

        private string _message;
        private bool _newMessage;

        public PreAuthForm() {
            InitializeComponent();
        }

        public string Message {
            get {
                return _message;
            }
            set {
                _message = value;
                _newMessage = true;
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e) {
            if (Exit != null) {
                Exit(sender, e);
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (_newMessage) {
                FormLabel.Text = _message;
            }
        }
    }
}
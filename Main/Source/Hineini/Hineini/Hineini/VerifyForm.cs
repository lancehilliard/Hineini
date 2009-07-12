using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Hineini {
    public partial class VerifyForm : Form {
        private bool _newStatus;
        private string _status;
        private bool _verifyEventPending;

        public VerifyForm() {
            InitializeComponent();
        }

        public event EventHandler<VerifyEventArguments> Verify;
        public event EventHandler Exit;



        public string Status {
            get {
                return _status;
            }
            set {
                _status = value;
                _newStatus = true;
            }
        }

        private void continueMenuItem_Click(object sender, EventArgs e) {
            statusLabel.Text = "Verifying...";
            _verifyEventPending = true;
        }

        private void exitMenuItem_Click(object sender, EventArgs e) {
            Exit(sender, EventArgs.Empty);
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (!IsDisposed) {
                if (_newStatus) {
                    statusLabel.Text = _status;
                    _newStatus = false;
                }
                if (_verifyEventPending) {
                    Verify(sender, new VerifyEventArguments(textBox1.Text.Trim().ToUpper()));
                    _verifyEventPending = false;
                }
            }
        }
    }

    public class VerifyEventArguments : EventArgs {
        public string Verifier;
        public VerifyEventArguments(string verifier) {
            Verifier = verifier;
        }
    }

}
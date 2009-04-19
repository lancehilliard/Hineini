using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hineini.Utility;

namespace Hineini {
    public partial class MessagesForm : Form {
        private Constants.MessageType _messageType;
        private static readonly List<string> _errorMessages = new List<string>(20);
        private static readonly List<string> _infoMessages = new List<string>(20);

        public static List<string> ErrorMessages {
            get { return _errorMessages; }
        }

        public static List<string> InfoMessages {
            get { return _infoMessages; }
        }

        public MessagesForm() {
            InitializeComponent();
        }

        public event EventHandler HideForm;

        public void ShowMessages(Constants.MessageType messageType) {
            Show();
            _messageType = messageType;
            ShowMessages();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (Boolean.IsActiveApplication) {
                if (MainForm.MessageWaitingToBeShown) {
                    ShowMessages();
                    MainForm.MessageWaitingToBeShown = false;
                }
            }
        }

        public static string MostRecentInfoMessage {
            get {
                return InfoMessages.Count > 0 ? InfoMessages[0] : String.Empty;
            }
        }

        private void ShowMessages() {
            string[] messages = (Constants.MessageType.Error.Equals(_messageType) ? ErrorMessages : InfoMessages).ToArray();
            if (messages.Length > 0) {
                string labelText = String.Join(Environment.NewLine, messages);
                if (!messagesLabel.Text.Equals(labelText)) {
                    messagesLabel.Text = labelText;
                }
            }
            else {
                messagesLabel.Text = String.Empty;
            }
        }

        private void backMenuItem_Click(object sender, EventArgs e) {
            if (HideForm != null) {
                HideForm(sender, e);
            }
        }

        public static void AddMessage(DateTime? when, string message, Constants.MessageType messageType) {
            if (Helpers.StringHasValue(message)) {
                if (when != null) {
                    message = String.Format(Constants.ADD_MESSAGE_TEMPLATE, MainUtility.GetShortTimeString(when.Value), message);
                }
                List<string> messagesList = Constants.MessageType.Info.Equals(messageType) ? _infoMessages : _errorMessages;
                if (messagesList.Count == 0 || !message.Equals(messagesList[0])) {
                    if (messagesList.Count.Equals(messagesList.Capacity)) {
                        messagesList.RemoveAt(messagesList.Count - 1);
                    }
                    messagesList.Insert(0, message);
                    MainForm.MessageWaitingToBeShown = true;
                }
            }
        }
    }
}
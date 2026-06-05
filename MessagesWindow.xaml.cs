using System.Windows;
using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Services;

namespace NimbusChat
{
    public partial class MessagesWindow : Window
    {
        private readonly MessageService _messageService = new MessageService();
        private readonly int _currentUserId;
        private readonly int _otherUserId;

        public MessagesWindow(int currentUserId, int otherUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
            _otherUserId = otherUserId;
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            var text = MessageInput.Text.Trim();
            if (string.IsNullOrEmpty(text))
                return;

            var success = _messageService.SendMessage(_currentUserId, _otherUserId, text);

            if (success)
            {
                ChatList.Items.Add($"Me: {text}");
                MessageInput.Clear();
            }
            else
            {
                MessageBox.Show("Message could not be sent.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
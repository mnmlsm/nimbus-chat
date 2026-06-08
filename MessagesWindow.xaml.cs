using System;
using System.Windows;
using System.Windows.Threading;
using NimbusChat.WetterChatApp.Services;

namespace NimbusChat
{
    public partial class MessagesWindow : Window
    {
        private readonly MessageService _messageService = new MessageService();
        private readonly int _currentUserId;
        private int _lastMessageId = 0;
        private readonly DispatcherTimer _pollTimer;

        public MessagesWindow(int currentUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;

            LoadGlobalHistory();

            // Einfaches Polling alle 3 Sekunden
            _pollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            _pollTimer.Tick += PollTimer_Tick;
            _pollTimer.Start();
        }

        private void LoadGlobalHistory()
        {
            ChatList.Items.Clear();

            var items = _messageService.GetAllGlobalMessages();

            foreach (var (message, displayName) in items)
            {
                var prefix = message.SenderId == _currentUserId ? "Me" : displayName;
                ChatList.Items.Add($"{prefix}: {message.Content}");

                if (message.Id > _lastMessageId)
                    _lastMessageId = message.Id;
            }
        }

        private void PollTimer_Tick(object sender, EventArgs e)
        {
            var newItems = _messageService.GetNewGlobalMessagesSince(_lastMessageId);

            foreach (var (message, displayName) in newItems)
            {
                var prefix = message.SenderId == _currentUserId ? "Me" : displayName;
                ChatList.Items.Add($"{prefix}: {message.Content}");

                if (message.Id > _lastMessageId)
                    _lastMessageId = message.Id;
            }
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            var text = MessageInput.Text.Trim();
            if (string.IsNullOrEmpty(text))
                return;

            var success = _messageService.SendGlobalMessage(_currentUserId, text);

            if (success)
            {
                // Wichtig: NICHT direkt zur ChatList hinzufügen,
                // sonst würde sie beim nächsten Poll doppelt erscheinen.
                MessageInput.Clear();
            }
            else
            {
                MessageBox.Show("Message could not be sent.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _pollTimer?.Stop();
            base.OnClosed(e);
        }
    }
}
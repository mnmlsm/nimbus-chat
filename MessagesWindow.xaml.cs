using NimbusChat.WetterChatApp.Repositories;
using NimbusChat.WetterChatApp.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace NimbusChat
{
    public partial class MessagesWindow : Window
    {
        private readonly MessageService _messageService = new MessageService(new UserRepository(), new MessageRepository());
        private readonly int _currentUserId;
        private int _lastMessageId = 0;
        private readonly DispatcherTimer _pollTimer;

        private readonly ApiClient _apiClient;   // NEU

        public MessagesWindow(int currentUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;

            _apiClient = new ApiClient(); // NEU

            LoadGlobalHistory();

            // Einfaches Polling alle 3 Sekunden
            _pollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            _pollTimer.Tick += PollTimer_Tick;
            _pollTimer.Start();

            // Health-Check beim Start (NEU)
            _ = CheckApiAsync();
        }

        private async Task CheckApiAsync()
        {
            try
            {
                // z.B. Fenstertitel vorübergehend anpassen
                this.Title = "Nimbus Chat - Prüfe API...";

                var result = await _apiClient.GetHealthAsync();

                // z.B. Ergebnis im Titel anzeigen
                this.Title = $"Nimbus Chat - API: {result}";
            }
            catch (Exception ex)
            {
                this.Title = $"Nimbus Chat - API Fehler: {ex.Message}";
            }
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
using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace NimbusChat
{
    // Code-behind for the chat window: lists contacts plus a pinned Global
    // Chat entry, polls for new messages, and sends/loads both global and
    // private conversations.
    public partial class MessagesWindow : Window
    {
        private readonly MessageService _messageService = new MessageService();
        private readonly ApiClient _apiClient = new ApiClient();
        private readonly int _currentUserId;
        private readonly DispatcherTimer _pollTimer;

        // Sentinel id for the pinned "Global Chat" entry; never collides with a
        // real Users.Id since AUTO_INCREMENT starts at 1.
        private const int GlobalChatId = -1;

        private List<User> _allUsers = new List<User>();
        private int _selectedUserId;

        public MessagesWindow(int currentUserId)
        {
            InitializeComponent();

            _currentUserId = currentUserId;

            LanguageManager.LanguageChanged +=
        LanguageManager_LanguageChanged;

            UpdateLanguage();

            Loaded += async (s, e) => await LoadUsersAsync();

            _pollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };

            _pollTimer.Tick += PollTimer_Tick;
            _pollTimer.Start();
        }

        private void LanguageManager_LanguageChanged(
    object sender,
    EventArgs e)
        {
            UpdateLanguage();

            DisplayUsers(_allUsers);
        }

        private void UpdateLanguage()
        {
            Title =
                LanguageManager.Get("MessagesTitle");

            MessagesTitleText.Text =
                LanguageManager.Get("MessagesTitle");

            YourConversationsText.Text =
                LanguageManager.Get("YourConversations");

            SearchPlaceholder.Text =
                LanguageManager.Get("Search");

            if (_selectedUserId == 0)
            {
                ChatHeader.Text =
                    LanguageManager.Get("SelectChat");
            }
        }
        private async Task LoadUsersAsync()
        {
            _allUsers = await _apiClient.GetAllUsersAsync();
            DisplayUsers(_allUsers);
        }

        private static User CreateGlobalChatEntry() => new User
        {
            Id = GlobalChatId,
            Username = "Global Chat",
            Status = "Everyone"
        };

        private void DisplayUsers(IEnumerable<User> users)
        {
            UsersList.Items.Clear();

            // Always pinned at the top, reachable for every user.
            UsersList.Items.Add(CreateGlobalChatEntry());

            foreach (var user in users)
            {
                if (user.Id == _currentUserId)
                    continue;

                UsersList.Items.Add(user);
            }
        }

        private async Task LoadPrivateMessagesAsync()
        {
            var messages = await _messageService.GetMessagesBetweenAsync(_currentUserId, _selectedUserId);

            ChatList.Items.Clear();

            foreach (var message in messages)
            {
                ChatList.Items.Add(new ChatMessage
                {
                    Content = message.Content,
                    IsMine = message.SenderId == _currentUserId,
                    Time = message.CreatedAt.ToString("HH:mm")
                });
            }

            if (ChatList.Items.Count > 0)
                ChatList.ScrollIntoView(ChatList.Items[ChatList.Items.Count - 1]);
        }

        private async Task LoadGlobalMessagesAsync()
        {
            var messages = await _messageService.GetAllGlobalMessagesAsync();

            ChatList.Items.Clear();

            foreach (var (message, displayName) in messages)
            {
                var isMine = message.SenderId == _currentUserId;

                ChatList.Items.Add(new ChatMessage
                {
                    Content = message.Content,
                    IsMine = isMine,
                    Time = message.CreatedAt.ToString("HH:mm"),
                    SenderName = isMine ? null : displayName
                });
            }

            if (ChatList.Items.Count > 0)
                ChatList.ScrollIntoView(ChatList.Items[ChatList.Items.Count - 1]);
        }

        private async void PollTimer_Tick(object sender, EventArgs e)
        {
            if (_selectedUserId == GlobalChatId)
            {
                await LoadGlobalMessagesAsync();
            }
            else if (_selectedUserId > 0)
            {
                await LoadPrivateMessagesAsync();
            }
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            var text = MessageInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return;

            bool success;

            if (_selectedUserId == GlobalChatId)
            {
                success = await _messageService.SendGlobalMessageAsync(_currentUserId, text);
            }
            else
            {
                success = await _messageService.SendPrivateMessageAsync(_currentUserId, _selectedUserId, text);
            }

            if (success)
            {
                MessageInput.Clear();

                if (_selectedUserId == GlobalChatId)
                    await LoadGlobalMessagesAsync();
                else
                    await LoadPrivateMessagesAsync();
            }
            else
            {
                AppMessageBox.Show(
     LanguageManager.Get("MessageSendError"),
     LanguageManager.Get("Error"),
     AppMessageBoxIcon.Error,
     this);
            }
        }

        private string TranslateStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return string.Empty;

            switch (status)
            {
                case "Online":
                    return LanguageManager.Get("Online");

                case "Busy":
                    return LanguageManager.Get("Busy");

                case "Away":
                    return LanguageManager.Get("Away");

                default:
                    return status;
            }
        }
        private async void UsersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersList.SelectedItem is User user)
            {
                _selectedUserId = user.Id;

                ChatHeader.Text = user.Username;
                ChatStatus.Text = user.Status;

                AvatarLetter.Text = user.Initial;
                AvatarBorder.Background = user.AvatarBrush;

                switch (user.Status)
                {
                    case "Online":
                        ChatStatus.Foreground = Brushes.LimeGreen;
                        break;

                    case "Busy":
                        ChatStatus.Foreground = Brushes.IndianRed;
                        break;

                    case "Away":
                        ChatStatus.Foreground = Brushes.Gold;
                        break;

                    case "Everyone":
                        ChatStatus.Foreground = (Brush)FindResource("PrimaryBrush");
                        break;

                    default:
                        ChatStatus.Foreground = Brushes.Gray;
                        break;
                }

                if (user.Id == GlobalChatId)
                    await LoadGlobalMessagesAsync();
                else
                    await LoadPrivateMessagesAsync();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(search))
            {
                DisplayUsers(_allUsers);
            }
            else
            {
                var filteredUsers = _allUsers.Where(user =>
                    user.Username.ToLower().Contains(search) ||
                    user.Email.ToLower().Contains(search));

                DisplayUsers(filteredUsers);
            }

            SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Send_Click(sender, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text)
                ? Visibility.Collapsed
                : Visibility.Hidden;
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void MessageInput_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        protected override void OnClosed(EventArgs e)
        {
            _pollTimer?.Stop();
            LanguageManager.LanguageChanged -=
       LanguageManager_LanguageChanged;
            base.OnClosed(e);
        }


    }
}

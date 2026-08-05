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
    public partial class MessagesWindow : Window
    {
        private readonly MessageService _messageService = new MessageService();
        private readonly ApiClient _apiClient = new ApiClient();
        private readonly int _currentUserId;
        private readonly DispatcherTimer _pollTimer;

        private List<User> _allUsers = new List<User>();
        private int _selectedUserId;

        public MessagesWindow(int currentUserId)
        {
            InitializeComponent();

            _currentUserId = currentUserId;

            Loaded += async (s, e) => await LoadUsersAsync();

            _pollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };

            _pollTimer.Tick += PollTimer_Tick;
            _pollTimer.Start();
        }

        private async Task LoadUsersAsync()
        {
            _allUsers = await _apiClient.GetAllUsersAsync();
            DisplayUsers(_allUsers);
        }

        private void DisplayUsers(IEnumerable<User> users)
        {
            UsersList.Items.Clear();

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

        private async void PollTimer_Tick(object sender, EventArgs e)
        {
            if (_selectedUserId > 0)
            {
                await LoadPrivateMessagesAsync();
            }
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            var text = MessageInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return;

            var success = await _messageService.SendPrivateMessageAsync(_currentUserId, _selectedUserId, text);

            if (success)
            {
                MessageInput.Clear();
                await LoadPrivateMessagesAsync();
            }
            else
            {
                AppMessageBox.Show("Message could not be sent.", "Error", AppMessageBoxIcon.Error, this);
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

                    default:
                        ChatStatus.Foreground = Brushes.Gray;
                        break;
                }

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
            base.OnClosed(e);
        }
    }
}

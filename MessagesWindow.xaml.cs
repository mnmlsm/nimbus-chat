using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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

            LoadUsers();

            _pollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };

            _pollTimer.Tick += PollTimer_Tick;
            _pollTimer.Start();
        }

        private void LoadUsers()
        {
            _allUsers = _apiClient.GetAllUsersAsync().GetAwaiter().GetResult();
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

        private void LoadPrivateMessages()
        {
            ChatList.Items.Clear();

            var messages = _messageService.GetMessagesBetween(_currentUserId, _selectedUserId);

            foreach (var message in messages)
            {
                var prefix = message.SenderId == _currentUserId ? "Me" : "Them";
                ChatList.Items.Add($"{prefix}: {message.Content}");
            }
        }

        private void PollTimer_Tick(object sender, EventArgs e)
        {
            if (_selectedUserId > 0)
            {
                LoadPrivateMessages();
            }
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            var text = MessageInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return;

            var success = _messageService.SendPrivateMessage(_currentUserId, _selectedUserId, text);

            if (success)
            {
                MessageInput.Clear();
                LoadPrivateMessages();
            }
            else
            {
                MessageBox.Show("Message could not be sent.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UsersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersList.SelectedItem is User user)
            {
                _selectedUserId = user.Id;
                LoadPrivateMessages();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(search))
            {
                DisplayUsers(_allUsers);
                return;
            }

            var filteredUsers = _allUsers.Where(user =>
                user.Username.ToLower().Contains(search) ||
                user.Email.ToLower().Contains(search));

            DisplayUsers(filteredUsers);
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

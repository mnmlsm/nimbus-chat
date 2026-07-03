using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;
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
        private readonly int _currentUserId;
        private int _lastMessageId = 0;
        private readonly DispatcherTimer _pollTimer;

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

        //private void LoadGlobalHistory()
       // {
        //    ChatList.Items.Clear();

        //    var items = _messageService.GetAllGlobalMessages();

        //    foreach (var (message, displayName) in items)
        //    {
        //        var prefix = message.SenderId == _currentUserId ? "Me" : displayName;
        //        ChatList.Items.Add($"{prefix}: {message.Content}");

        //        if (message.Id > _lastMessageId)
        //            _lastMessageId = message.Id;
        //    }
        //}

        private void LoadUsers()
        {
            _allUsers = _userRepository.GetAllUsers();

            DisplayUsers(_allUsers);

        }

        private void DisplayUsers(IEnumerable<User> users)
        {
            UsersList.Items.Clear();

            foreach (var user in users)
            {
                if (user.Id == _currentUserId)
                    continue;

                string statusIcon = "⚫";

                switch (user.Status)
                {
                    case "Online":
                        statusIcon = "🟢";
                        break;

                    case "Busy":
                        statusIcon = "🔴";
                        break;

                    case "Away":
                        statusIcon = "🟡";
                        break;
                }

                UsersList.Items.Add(new User
                {
                    Id = user.Id,
                    Username = $"{statusIcon} {user.Username}",
                    Email = user.Email,
                    PasswordHash = user.PasswordHash,
                    Status = user.Status,
                    FavoriteCity = user.FavoriteCity
                });
            }
        }

        private void LoadPrivateMessages()
        {
            ChatList.Items.Clear();

            var messages =
                _messageService.GetMessagesBetween(
                    _currentUserId,
                    _selectedUserId);

            foreach (var message in messages)
            {
                var prefix =
                    message.SenderId == _currentUserId
                    ? "Me"
                    : "Them";

                ChatList.Items.Add(
                    $"{prefix}: {message.Content}");
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

            var success = _messageService.SendPrivateMessage(
           _currentUserId,
           _selectedUserId,
           text);

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

        private void UsersList_SelectionChanged(
    object sender,
    SelectionChangedEventArgs e)
        {


            if (UsersList.SelectedItem is User user)
            {
                _selectedUserId = user.Id;

                LoadPrivateMessages();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _pollTimer?.Stop();
            base.OnClosed(e);
        }
        private readonly UserRepository _userRepository =
    new UserRepository();

        private List<User> _allUsers = new List<User>();

        private int _selectedUserId;

        private void MessageInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchBox.Text.Trim().ToLower();

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

    }
}
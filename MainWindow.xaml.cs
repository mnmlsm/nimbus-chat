using NimbusChat.WetterChatApp.Services;
using NimbusChat.WetterChatApp.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace NimbusChat
{
    public partial class MainWindow : Window
    {
        private readonly ApiClient _apiClient = new ApiClient();
        private readonly DispatcherTimer _connectionCheckTimer;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();

            Loaded += async (s, e) => await CheckConnectionAsync();

            _connectionCheckTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(15)
            };
            _connectionCheckTimer.Tick += async (s, e) => await CheckConnectionAsync();
            _connectionCheckTimer.Start();

            Closed += (s, e) => _connectionCheckTimer.Stop();
        }

        private async System.Threading.Tasks.Task CheckConnectionAsync()
        {
            try
            {
                await _apiClient.GetHealthAsync();
                ConnectionDot.Fill = Brushes.LimeGreen;
                ConnectionText.Text = "SERVER · CONNECTED";
            }
            catch
            {
                ConnectionDot.Fill = Brushes.Red;
                ConnectionText.Text = "SERVER · NOT CONNECTED";
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        private void OpenRegister_Click(object sender, RoutedEventArgs e)
        {
            var register = new RegisterWindow();
            register.Show();
            Close();
        }

        private bool _showPassword = false;

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordTextBox.Text;
            }
        }

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            _showPassword = !_showPassword;

            if (_showPassword)
            {
                PasswordTextBox.Text = PasswordBox.Password;

                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;

                EyeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOffOutline;
            }
            else
            {
                PasswordBox.Password = PasswordTextBox.Text;

                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;

                EyeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOutline;
            }
        }

    }
}

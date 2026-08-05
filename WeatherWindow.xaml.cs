using System.ComponentModel;
using System.Windows;
using NimbusChat.ViewModels;

namespace NimbusChat
{
    public partial class WeatherWindow : Window
    {
        public WeatherViewModel ViewModel => (WeatherViewModel)DataContext;

        public WeatherWindow()
        {
            InitializeComponent();

        }

        private void UseForDashboard_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ViewModel.City))
                return;

            DialogResult = true;
        }
    }
}
using System.Windows;

namespace NimbusChat
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
        }

        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            new ProfileWindow().Show();
        }

        private void OpenWeather_Click(object sender, RoutedEventArgs e)
        {
            new WeatherWindow().Show();
        }

        private void OpenMessages_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Messages page coming soon!");
        }
    }
}
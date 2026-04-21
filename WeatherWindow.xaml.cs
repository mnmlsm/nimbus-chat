using System.Windows;

namespace NimbusChat
{
    public partial class WeatherWindow : Window
    {
        public WeatherWindow()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CityTextBox.Text))
            {
                MessageBox.Show("Please enter a city");
                return;
            }

            // Fake Result (without API)
            WeatherResult.Text = CityTextBox.Text + ": 14°C, Cloudy";
        }
    }
}

using NimbusChat.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;

namespace NimbusChat
{
    // Standalone weather search dialog: lets the user look up any city and,
    // if confirmed, hands the chosen city back to the dashboard.
    public partial class WeatherWindow : Window
    {
        public WeatherViewModel ViewModel => (WeatherViewModel)DataContext;

        public WeatherWindow()
        {
            InitializeComponent();

            LanguageManager.LanguageChanged +=
                LanguageManager_LanguageChanged;

            UpdateLanguage();
        }

        private void LanguageManager_LanguageChanged(
    object sender,
    EventArgs e)
        {
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            Title =
                LanguageManager.Get("WeatherSearch");

            WeatherSearchTitle.Text =
                LanguageManager.Get("WeatherSearch");

            SearchWeatherAnywhereText.Text =
                LanguageManager.Get("SearchWeatherAnywhere");

            CityLabelText.Text =
                LanguageManager.Get("City");

            SearchWeatherButton.Content =
                LanguageManager.Get("SearchWeather");

            HumidityText.Text =
                "💧 " + LanguageManager.Get("Humidity");

            WindText.Text =
                "💨 " + LanguageManager.Get("Wind");

            FeelsLikeText.Text =
                "🌡 " + LanguageManager.Get("FeelsLike");

            UseForDashboardButton.Content =
                LanguageManager.Get("UseForDashboard");
        }
        private void UseForDashboard_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ViewModel.City))
                return;

            DialogResult = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            LanguageManager.LanguageChanged -=
                LanguageManager_LanguageChanged;

            base.OnClosed(e);
        }
    }
}
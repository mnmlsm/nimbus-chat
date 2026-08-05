using System.Windows;
using System.Windows.Controls;

namespace NimbusChat
{
    public partial class WeatherIconControl : UserControl
    {
        public static readonly DependencyProperty ConditionProperty =
            DependencyProperty.Register(
                nameof(Condition),
                typeof(string),
                typeof(WeatherIconControl),
                new PropertyMetadata(string.Empty, OnConditionChanged));

        public string Condition
        {
            get => (string)GetValue(ConditionProperty);
            set => SetValue(ConditionProperty, value);
        }

        public WeatherIconControl()
        {
            InitializeComponent();
        }

        private static void OnConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WeatherIconControl control)
                control.UpdateVisual();
        }

        private void UpdateVisual()
        {
            var c = (Condition ?? string.Empty).ToLower();

            SunnyVisual.Visibility = Visibility.Collapsed;
            CloudyVisual.Visibility = Visibility.Collapsed;
            RainVisual.Visibility = Visibility.Collapsed;
            SnowVisual.Visibility = Visibility.Collapsed;

            if (c.Contains("clear"))
            {
                SunnyVisual.Visibility = Visibility.Visible;
            }
            else if (c.Contains("rain") || c.Contains("drizzle") || c.Contains("thunder"))
            {
                RainVisual.Visibility = Visibility.Visible;
            }
            else if (c.Contains("snow"))
            {
                SnowVisual.Visibility = Visibility.Visible;
            }
            else
            {
                // cloud, mist, fog, haze und Fallback
                CloudyVisual.Visibility = Visibility.Visible;
            }
        }
    }
}

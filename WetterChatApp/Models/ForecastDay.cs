using System.Windows.Media;

namespace NimbusChat.WetterChatApp.Models
{
    // One day of the 5-day forecast strip, including the tile colors used to
    // render it on the dashboard.
    public class ForecastDay
    {
        public string Day { get; set; }

        public string Temperature { get; set; }

        public string Icon { get; set; }

        public string IconUrl { get; set; }

        // Weather description (e.g. "clear sky", "light rain") for the animated icon.
        public string Condition { get; set; }

        // Tile background matching the condition, using the same categorization
        // as WeatherIconControl.UpdateVisual and DashboardWindow.UpdateWeatherVisual.
        public Brush TileBackground => ResolveColors(Condition).Background;

        public Brush TileBorderBrush => ResolveColors(Condition).Border;

        private static (Brush Background, Brush Border) ResolveColors(string condition)
        {
            var c = (condition ?? string.Empty).ToLower();

            if (c.Contains("clear"))
                return (ToBrush("#FFF3D6"), ToBrush("#F0DBA0"));

            if (c.Contains("rain") || c.Contains("drizzle") || c.Contains("thunder"))
                return (ToBrush("#D9E9FC"), ToBrush("#AED2F5"));

            if (c.Contains("snow"))
                return (ToBrush("#F1FAFF"), ToBrush("#D3ECFB"));

            // cloud, mist, fog, haze, and fallback
            return (ToBrush("#EBEFF5"), ToBrush("#D9E1EC"));
        }

        private static Brush ToBrush(string hex)
        {
            var brush = (Brush)new BrushConverter().ConvertFromString(hex);
            brush.Freeze();
            return brush;
        }
    }
}

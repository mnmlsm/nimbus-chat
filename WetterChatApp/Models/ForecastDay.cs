using System.Windows.Media;

namespace NimbusChat.WetterChatApp.Models
{
    public class ForecastDay
    {
        public string Day { get; set; }

        public string Temperature { get; set; }

        public string Icon { get; set; }

        public string IconUrl { get; set; }

        // Wetterbeschreibung (z.B. "clear sky", "light rain") für das animierte Icon.
        public string Condition { get; set; }

        // Kachel-Hintergrund passend zur Condition, dieselbe Kategorisierung wie
        // WeatherIconControl.UpdateVisual und DashboardWindow.UpdateWeatherVisual.
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

            // cloud, mist, fog, haze und Fallback
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

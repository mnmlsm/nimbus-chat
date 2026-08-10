namespace NimbusChat.Api.Models
{
    // Response shapes for WeatherController: a stored current-weather reading
    // and a single day of the forecast.
    public class WeatherDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string City { get; set; } = default!;
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string Description { get; set; } = default!;
        public string Icon { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }

    public class ForecastDayDto
    {
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public string Icon { get; set; } = default!;
        public string Condition { get; set; } = default!;
    }
}

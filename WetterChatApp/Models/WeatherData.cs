using System;

namespace NimbusChat.WetterChatApp.Models
{
    // Client-side shape of a single weather reading, as returned by the API's
    // /api/weather endpoint.
    public class WeatherData
    {
        public int Id { get; set; }          // matches the DB column Id
        public int? UserId { get; set; }
        public string City { get; set; }
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }  // m/s, as delivered by OpenWeatherMap
        public string Description { get; set; }
        public string Icon { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

using System;

namespace NimbusChat.WetterChatApp.Models
{
    public class WeatherData
    {
        public int Id { get; set; }          // entspricht der DB-Spalte Id
        public int? UserId { get; set; }
        public string City { get; set; }
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }  // m/s, wie von OpenWeatherMap geliefert
        public string Description { get; set; }
        public string Icon { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

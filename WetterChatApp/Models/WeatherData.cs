namespace NimbusChat.WetterChatApp.Models
{
    public class WeatherData
    {
        public int Id { get; set; }          // entspricht der DB-Spalte Id
        public string City { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }  // ISO-8601 Zeitstempel als Text
    }
}
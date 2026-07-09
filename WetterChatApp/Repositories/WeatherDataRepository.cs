using System;
using MySql.Data.MySqlClient;
using NimbusChat.WetterChatApp.Data;
using NimbusChat.WetterChatApp.Models;

namespace NimbusChat.WetterChatApp.Repositories
{
    public class WeatherDataRepository
    {
        public bool Create(WeatherData weather)
        {
            using (var connection = new MySqlConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                const string sql = @"
INSERT INTO WeatherData (City, Temperature, Humidity, Description, CreatedAt)
VALUES (@City, @Temperature, @Humidity, @Description, @CreatedAt);";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@City", weather.City);
                    command.Parameters.AddWithValue("@Temperature", weather.Temperature);
                    command.Parameters.AddWithValue("@Humidity", weather.Humidity);
                    command.Parameters.AddWithValue("@Description", weather.Description);

                    // weather.CreatedAt ist string im Model
                    DateTime createdAt;

                    if (string.IsNullOrWhiteSpace(weather.CreatedAt))
                    {
                        createdAt = DateTime.UtcNow;
                    }
                    else if (!DateTime.TryParse(weather.CreatedAt, out createdAt))
                    {
                        createdAt = DateTime.UtcNow;
                    }

                    command.Parameters.AddWithValue("@CreatedAt", createdAt);

                    try
                    {
                        return command.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException)
                    {
                        return false;
                    }
                }
            }
        }
    }
}
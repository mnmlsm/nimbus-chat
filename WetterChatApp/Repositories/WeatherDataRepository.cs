using System;
using System.Data.SQLite;
using NimbusChat.WetterChatApp.Data;
using NimbusChat.WetterChatApp.Models;

namespace NimbusChat.WetterChatApp.Repositories
{
    public class WeatherDataRepository
    {
        public bool Create(WeatherData weather)
        {
            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
                    INSERT INTO WeatherData (City, Temperature, Humidity, Description, CreatedAt)
                    VALUES (@City, @Temperature, @Humidity, @Description, @CreatedAt);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@City", weather.City);
                    command.Parameters.AddWithValue("@Temperature", weather.Temperature);
                    command.Parameters.AddWithValue("@Humidity", weather.Humidity);
                    command.Parameters.AddWithValue("@Description", weather.Description);
                    command.Parameters.AddWithValue("@CreatedAt", weather.CreatedAt);

                    try
                    {
                        return command.ExecuteNonQuery() > 0;
                    }
                    catch (SQLiteException)
                    {
                        // hier könntest du ggf. loggen
                        return false;
                    }
                }
            }
        }
    }
}
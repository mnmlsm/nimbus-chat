using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;



namespace NimbusChat.WetterChatApp.Data
{
    public static class DatabaseInitializer
    {
        private static string DatabaseFile = Path.Combine(Environment.CurrentDirectory, "weather.db");

        public static string ConnectionString => $"Data Source={DatabaseFile};Version=3;";

        public static void Initialize()
        {
            if (!File.Exists(DatabaseFile))
            {
                SQLiteConnection.CreateFile(DatabaseFile);
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                var sql = @"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    Email TEXT NOT NULL,
    PasswordHash TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Messages (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SenderId INTEGER NOT NULL,
    ReceiverId INTEGER NOT NULL,
    Content TEXT NOT NULL,
    CreatedAt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS WeatherData (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    City TEXT NOT NULL,
    Temperature REAL NOT NULL,
    Humidity INTEGER NOT NULL,
    Description TEXT NOT NULL,
    CreatedAt TEXT NOT NULL
);";
SS
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
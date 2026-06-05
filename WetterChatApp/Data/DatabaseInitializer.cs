using System;
using System.IO;
using System.Data.SQLite;

namespace NimbusChat.WetterChatApp.Data
{
    public static class DatabaseInitializer
    {
        private static string DatabaseFile => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Files",
            "WetterchatDatabaseLibrary.db");

        public static string ConnectionString => $"Data Source={DatabaseFile};Version=3;";

        public static void Initialize()
        {
            var directory = Path.GetDirectoryName(DatabaseFile);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

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
    PasswordHash TEXT NOT NULL,
    Status TEXT,
    FavoriteCity TEXT
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

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Optional: für bestehende DBs Spalten nachrüsten (fehler werden still ignoriert)
                var alterSql = @"
ALTER TABLE Users ADD COLUMN Status TEXT;
ALTER TABLE Users ADD COLUMN FavoriteCity TEXT;";
                try
                {
                    using (var alterCommand = new SQLiteCommand(alterSql, connection))
                    {
                        alterCommand.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException)
                {
                    // Spalten existieren bereits -> ignorieren
                }
            }
        }
    }
}
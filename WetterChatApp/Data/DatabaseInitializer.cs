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
        private const string DatabaseFile = "C:\\Users\\Memo\\Documents\\GitHub\\nimbus-chat\\Files\\WetterchatDatabaseLibrary.db";

        public static void Initialize()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DatabaseFile);

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            var connectionString = $"Data Source={dbPath};Version=3;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var sql = @"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
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

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
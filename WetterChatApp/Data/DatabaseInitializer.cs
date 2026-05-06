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
        // new path for saving the file in a portable way, relative to the application directory .. LG Memooooo
        private static string DatabaseFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"../", "../", "Files", "WetterchatDatabaseLibrary.db");

        // Old path for saving the file on the local machine, not portable
        //private const string DatabaseFile = @"C:\Users\Student\source\repos\nimbus-chat\Files\WetterchatDatabaseLibrary.db";

        public static string ConnectionString => $"Data Source={DatabaseFile};Version=3;";

        public static void Initialize()
        {
            if (Directory.Exists(Path.GetDirectoryName(DatabaseFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(DatabaseFile));
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
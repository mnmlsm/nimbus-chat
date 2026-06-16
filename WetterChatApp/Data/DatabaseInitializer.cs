using System;
using System.IO;
using System.Data.SQLite;

namespace NimbusChat.WetterChatApp.Data
{
    public static class DatabaseInitializer
    {
        // readonly property to get the path of the SQLite database file in the output directory (bin/Debug or bin/Release)
        // Path of the SQLite database file in the output directory (bin/Debug or bin/Release)
        private static string DatabaseFile => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Data",
            "WetterchatDatabaseLibrary.db");

        public static string ConnectionString => $"Data Source={DatabaseFile};Version=3;";

        public static void Initialize()
        {
            // Ensure the directory for the database file exists
            string directory = Path.GetDirectoryName(DatabaseFile);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(DatabaseFile))
            {
                // creates a new database file in the output directory,
                // originally created in the project directory, but copied to output during build
                SQLiteConnection.CreateFile(DatabaseFile);
            }

            // Open a connection to the SQLite database and create the necessary tables if they do not exist
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // SQL command to create the Users, Messages, and WeatherData tables if they do not already exist
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Users 
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Email TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        Status TEXT,
                        FavoriteCity TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Messages 
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SenderId INTEGER NOT NULL,
                        ReceiverId INTEGER NOT NULL,
                        Content TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                    );

                    CREATE TABLE IF NOT EXISTS WeatherData 
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        City TEXT NOT NULL,
                        Temperature REAL NOT NULL,
                        Humidity INTEGER NOT NULL,
                        Description TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL
                    );";
                // Execute the SQL command to create the tables
                using (var command = new SQLiteCommand(sql, connection))
                {
                    // Execute the SQL command to create the tables
                    command.ExecuteNonQuery();
                }

                // SQL command to add new columns to the Users table if they do not already exist
                string alterSql = @"
                        ALTER TABLE Users ADD COLUMN Status TEXT;
                        ALTER TABLE Users ADD COLUMN FavoriteCity TEXT;";

                // Try to execute the ALTER TABLE command to add new columns, but ignore any exceptions if the columns already exist
                try
                {
                    using (var alterCommand = new SQLiteCommand(alterSql, connection))
                    {
                        alterCommand.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException)
                {
                    // Ignore exceptions if the columns already exist
                }
            }
        }
    }
}
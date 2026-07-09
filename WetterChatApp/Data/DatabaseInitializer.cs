using System;
using MySql.Data.MySqlClient; // MySQL ADO.NET Provider

namespace NimbusChat.WetterChatApp.Data
{
    public static class DatabaseInitializer
    {
        // MySQL-Verbindungsdaten – bitte an deine Umgebung anpassen:
        private const string Server = "localhost";
        private const int Port = 3306;
        private const string User = "root";
        private const string Password = "NimbusChatPW";
        private const string DatabaseName = "nimbuschat";

        // ConnectionString für deinen MySQL-Server MIT Datenbank
        public static string ConnectionString =>
            $"Server={Server};Port={Port};Database={DatabaseName};Uid={User};Pwd={Password};";

        public static void Initialize()
        {
            // Datenbank automatisch erstellen (falls sie noch nicht existiert)
            EnsureDatabaseExists();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                // Tabellen in MySQL-Syntax anlegen
                var sql = @"
CREATE TABLE IF NOT EXISTS Users 
(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(255) NOT NULL UNIQUE,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Status VARCHAR(255),
    FavoriteCity VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS Messages 
(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    SenderId INT NOT NULL,
    ReceiverId INT NOT NULL,
    Content TEXT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_messages_sender FOREIGN KEY (SenderId) REFERENCES Users(Id),
    CONSTRAINT fk_messages_receiver FOREIGN KEY (ReceiverId) REFERENCES Users(Id)
);

CREATE TABLE IF NOT EXISTS WeatherData 
(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    City VARCHAR(255) NOT NULL,
    Temperature DOUBLE NOT NULL,
    Humidity INT NOT NULL,
    Description TEXT NOT NULL,
    CreatedAt DATETIME NOT NULL
);";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                // OPTIONAL: ALTER TABLE, falls du später Spalten ergänzt
                var alterSql = @"
ALTER TABLE Users 
    ADD COLUMN Status VARCHAR(255),
    ADD COLUMN FavoriteCity VARCHAR(255);";

                try
                {
                    using (var alterCommand = new MySqlCommand(alterSql, connection))
                    {
                        alterCommand.ExecuteNonQuery();
                    }
                }
                catch (MySqlException)
                {
                    // Ignorieren, wenn Spalten bereits existieren
                }
            }
        }

        private static void EnsureDatabaseExists()
        {
            // ConnectionString OHNE Database, nur Server + Login
            var masterConnectionString =
                $"Server={Server};Port={Port};Uid={User};Pwd={Password};";

            using (var connection = new MySqlConnection(masterConnectionString))
            {
                connection.Open();

                var sql = $"CREATE DATABASE IF NOT EXISTS `{DatabaseName}`;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
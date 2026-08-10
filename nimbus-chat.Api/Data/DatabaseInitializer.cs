using MySql.Data.MySqlClient;

namespace NimbusChat.Api.Data
{
    // Creates the database and tables when the API starts, if they don't
    // already exist. This used to live in the WPF client; now that the client
    // only ever talks through the API, it belongs on the side that owns the
    // MySQL connection.
    public static class DatabaseInitializer
    {
        public static void Initialize(string connectionString)
        {
            EnsureDatabaseExists(connectionString);

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

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
    ReceiverId INT NULL,
    Content TEXT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_messages_sender FOREIGN KEY (SenderId) REFERENCES Users(Id),
    CONSTRAINT fk_messages_receiver FOREIGN KEY (ReceiverId) REFERENCES Users(Id)
);

CREATE TABLE IF NOT EXISTS WeatherData
(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NULL,
    City VARCHAR(255) NOT NULL,
    Temperature DOUBLE NOT NULL,
    FeelsLike DOUBLE NULL,
    Humidity INT NOT NULL,
    WindSpeed DOUBLE NULL,
    Description TEXT NOT NULL,
    Icon VARCHAR(16) NULL,
    CreatedAt DATETIME NOT NULL,
    CONSTRAINT fk_weather_user FOREIGN KEY (UserId) REFERENCES Users(Id)
);";

            using var command = new MySqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private static void EnsureDatabaseExists(string connectionString)
        {
            var builder = new MySqlConnectionStringBuilder(connectionString);
            var databaseName = builder.Database;

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("Connection string 'NimbusChatDatabase' has no Database set.");

            // Connect without a database selected, otherwise opening the
            // connection fails as long as the database doesn't exist yet.
            builder.Database = string.Empty;

            using var connection = new MySqlConnection(builder.ConnectionString);
            connection.Open();

            using var command = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{databaseName}`;", connection);
            command.ExecuteNonQuery();
        }
    }
}

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
       
        private const string DatabaseFile = @"C:\Users\Memo\Documents\GitHub\nimbus-chat\Files\WetterchatDatabaseLibrary.db";

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
);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
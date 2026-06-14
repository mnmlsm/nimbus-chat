using NimbusChat.WetterChatApp.Data;
using NimbusChat.WetterChatApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace NimbusChat.WetterChatApp.Repositories
{
    public class UserRepository
    {
        // User anlegen
        public bool Create(User user)
        {
            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
INSERT INTO Users (Username, Email, PasswordHash, Status, FavoriteCity)
VALUES (@Username, @Email, @PasswordHash, @Status, @FavoriteCity);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@Status", (object)user.Status ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FavoriteCity", (object)user.FavoriteCity ?? DBNull.Value);

                    try
                    {
                        return command.ExecuteNonQuery() > 0;
                    }
                    catch (SQLiteException)
                    {
                        return false;
                    }
                }
            }
        }

        // User Finden
        public User GetByEmail(string email)
        {
            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
SELECT Id, Username, Email, PasswordHash, Status, FavoriteCity
FROM Users
WHERE Email = @Email
LIMIT 1;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return new User
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Username = reader["Username"].ToString(),
                            Email = reader["Email"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            Status = reader["Status"] == DBNull.Value ? null : reader["Status"].ToString(),
                            FavoriteCity = reader["FavoriteCity"] == DBNull.Value ? null : reader["FavoriteCity"].ToString()
                        };
                    }
                }
            }
        }

        // User finden mit ID
        public User GetById(int id)
        {
            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
SELECT Id, Username, Email, PasswordHash, Status, FavoriteCity
FROM Users
WHERE Id = @Id
LIMIT 1;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return new User
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Username = reader["Username"].ToString(),
                            Email = reader["Email"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            Status = reader["Status"] == DBNull.Value ? null : reader["Status"].ToString(),
                            FavoriteCity = reader["FavoriteCity"] == DBNull.Value ? null : reader["FavoriteCity"].ToString()
                        };
                    }
                }
            }
        }

        // User information update
        public bool Update(User user)
        {
            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
UPDATE Users
SET Username = @Username,
    Email = @Email,
    PasswordHash = @PasswordHash,
    Status = @Status,
    FavoriteCity = @FavoriteCity
WHERE Id = @Id;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@Status", (object)user.Status ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FavoriteCity", (object)user.FavoriteCity ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Id", user.Id);

                    try
                    {
                        return command.ExecuteNonQuery() > 0;
                    }
                    catch (SQLiteException)
                    {
                        return false;
                    }
                }
            }
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (var connection =
                   new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var command =
                    new SQLiteCommand(
                        @"SELECT Id,
                         Username,
                         Email,
                         PasswordHash,
                         Status,
                         FavoriteCity
                  FROM Users",
                        connection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        PasswordHash = reader.GetString(3),

                        Status = reader.IsDBNull(4)
                            ? null
                            : reader.GetString(4),

                        FavoriteCity = reader.IsDBNull(5)
                            ? null
                            : reader.GetString(5)
                    });
                }
            }

            return users;
        }

        public bool UpdateStatus(int userId, string status)
        {
            using (var connection =
                   new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
UPDATE Users
SET Status = @Status
WHERE Id = @Id;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@Id", userId);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
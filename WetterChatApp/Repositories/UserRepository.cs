using System;
using System.Data.SQLite;
using NimbusChat.WetterChatApp.Data;
using NimbusChat.WetterChatApp.Models;

namespace NimbusChat.WetterChatApp.Repositories
{
    public class UserRepository
    {
        public bool Create(User user)
        {
            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
                    INSERT INTO Users (Username, Email, PasswordHash)
                    VALUES (@Username, @Email, @PasswordHash);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

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
        public bool Update(User user)
        {
            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
                    UPDATE Users
                    SET Username = @Username,
                    Email = @Email,
                    PasswordHash = @PasswordHash
                    WHERE Id = @Id;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
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
        public User GetById(int id)
        {
            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
                    SELECT Id, Username, Email, PasswordHash
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
                            PasswordHash = reader["PasswordHash"].ToString()
                        };
                    }
                }
            }
        }

        public User GetByEmail(string email)
        {
            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
                    SELECT Id, Username, Email, PasswordHash
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
                            PasswordHash = reader["PasswordHash"].ToString()
                        };
                    }
                }
            }
        }
    }
}
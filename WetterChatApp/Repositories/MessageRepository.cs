using System;
using System.Collections.Generic;
using System.Data.SQLite;
using NimbusChat.WetterChatApp.Data;
using NimbusChat.WetterChatApp.Models;

namespace NimbusChat.WetterChatApp.Repositories
{
    public class MessageRepository
    {
        public bool Create(Message message)
        {
            var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            connection.Open();

            const string sql = @"
INSERT INTO Messages (SenderId, ReceiverId, Content, CreatedAt)
VALUES (@SenderId, @ReceiverId, @Content, @CreatedAt);";

            var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@SenderId", message.SenderId);
            command.Parameters.AddWithValue("@ReceiverId", message.ReceiverId);
            command.Parameters.AddWithValue("@Content", message.Content);
            command.Parameters.AddWithValue("@CreatedAt",
                message.CreatedAt == default
                    ? DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                    : message.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

            var rows = command.ExecuteNonQuery();
            return rows == 1;
        }

        // Bisheriger 1:1-Chat-Verlauf 

        public List<Message> GetMessagesBetween(int userId1, int userId2, int limit = 200)
        {
            var result = new List<Message>();

            var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            connection.Open();

            const string sql = @"
SELECT Id, SenderId, ReceiverId, Content, CreatedAt
FROM Messages
WHERE (SenderId = @U1 AND ReceiverId = @U2)
   OR (SenderId = @U2 AND ReceiverId = @U1)
ORDER BY datetime(CreatedAt) ASC
LIMIT @Limit;";

            var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@U1", userId1);
            command.Parameters.AddWithValue("@U2", userId2);
            command.Parameters.AddWithValue("@Limit", limit);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Message
                {
                    Id = reader.GetInt32(0),
                    SenderId = reader.GetInt32(1),
                    ReceiverId = reader.GetInt32(2),
                    Content = reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                });
            }

            return result;
        }

        public List<(Message Message, string DisplayName)> GetAllWithUser()
        {
            var result = new List<(Message, string)>();

            var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            connection.Open();

            const string sql = @"
SELECT m.Id,
       m.SenderId,
       m.ReceiverId,
       m.Content,
       m.CreatedAt,
       u.Username,
       u.Email
FROM Messages m
JOIN Users u ON u.Id = m.SenderId
ORDER BY datetime(m.CreatedAt) ASC;";

            var command = new SQLiteCommand(sql, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var message = new Message
                {
                    Id = reader.GetInt32(0),
                    SenderId = reader.GetInt32(1),
                    ReceiverId = reader.GetInt32(2),
                    Content = reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                };

                var username = reader.IsDBNull(5) ? null : reader.GetString(5);
                var email = reader.IsDBNull(6) ? null : reader.GetString(6);

                var displayName = !string.IsNullOrWhiteSpace(username) ? username : email;

                result.Add((message, displayName));
            }

            return result;
        }
        // Globaler Chat: neue Nachrichten seit einer bestimmten Id.

        public List<(Message Message, string DisplayName)> GetNewGlobalSince(int lastMessageId)
        {
            var result = new List<(Message, string)>();

            var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            connection.Open();

            const string sql = @"
SELECT m.Id,
       m.SenderId,
       m.ReceiverId,
       m.Content,
       m.CreatedAt,
       u.Username,
       u.Email
FROM Messages m
JOIN Users u ON u.Id = m.SenderId
WHERE m.Id > @LastId
ORDER BY m.Id ASC;";

            var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@LastId", lastMessageId);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var message = new Message
                {
                    Id = reader.GetInt32(0),
                    SenderId = reader.GetInt32(1),
                    ReceiverId = reader.GetInt32(2),
                    Content = reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                };

                var username = reader.IsDBNull(5) ? null : reader.GetString(5);
                var email = reader.IsDBNull(6) ? null : reader.GetString(6);
                var displayName = !string.IsNullOrWhiteSpace(username) ? username : email;

                result.Add((message, displayName));
            }

            return result;
        }
    }
}
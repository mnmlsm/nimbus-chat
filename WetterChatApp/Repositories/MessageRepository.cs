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
            Console.WriteLine($"[MessageRepository.Create] rows={rows}, sender={message.SenderId}, receiver={message.ReceiverId}");
            return rows == 1;
        }

        /// <summary>
        /// Holt den gesamten Chat-Verlauf zwischen zwei Usern.
        /// </summary>
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
        public List<Message> GetNewMessagesSince(int userId1, int userId2, int lastMessageId)
        {
            var result = new List<Message>();

            var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            connection.Open();

            const string sql = @"
SELECT Id, SenderId, ReceiverId, Content, CreatedAt
FROM Messages
WHERE Id > @LastId
  AND (
        (SenderId = @U1 AND ReceiverId = @U2)
     OR (SenderId = @U2 AND ReceiverId = @U1)
  )
ORDER BY Id ASC;";

            var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@LastId", lastMessageId);
            command.Parameters.AddWithValue("@U1", userId1);
            command.Parameters.AddWithValue("@U2", userId2);

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
    }
}
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
            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
INSERT INTO Messages (SenderId, ReceiverId, Content, CreatedAt)
VALUES (@SenderId, @ReceiverId, @Content, @CreatedAt);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@SenderId", message.SenderId);
                    command.Parameters.AddWithValue("@ReceiverId", message.ReceiverId);
                    command.Parameters.AddWithValue("@Content", message.Content);
                    command.Parameters.AddWithValue("@CreatedAt", message.CreatedAt.ToString("o")); // ISO-8601

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

        public List<Message> GetMessagesBetween(int userId1, int userId2)
        {
            var messages = new List<Message>();

            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
SELECT Id, SenderId, ReceiverId, Content, CreatedAt
FROM Messages
WHERE (SenderId = @User1 AND ReceiverId = @User2)
   OR (SenderId = @User2 AND ReceiverId = @User1)
ORDER BY Id;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@User1", userId1);
                    command.Parameters.AddWithValue("@User2", userId2);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new Message
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SenderId = Convert.ToInt32(reader["SenderId"]),
                                ReceiverId = Convert.ToInt32(reader["ReceiverId"]),
                                Content = reader["Content"].ToString(),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString())
                            });
                        }
                    }
                }
            }

            return messages;
        }



        public List<Message> GetNewMessagesSince(int userId1, int userId2, int lastMessageId)
        {
            var messages = new List<Message>();

            using (var connection = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                connection.Open();

                var sql = @"
SELECT Id, SenderId, ReceiverId, Content, CreatedAt
FROM Messages
WHERE Id > @LastId
  AND ((SenderId = @User1 AND ReceiverId = @User2)
    OR (SenderId = @User2 AND ReceiverId = @User1))
ORDER BY Id;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@LastId", lastMessageId);
                    command.Parameters.AddWithValue("@User1", userId1);
                    command.Parameters.AddWithValue("@User2", userId2);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new Message
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SenderId = Convert.ToInt32(reader["SenderId"]),
                                ReceiverId = Convert.ToInt32(reader["ReceiverId"]),
                                Content = reader["Content"].ToString(),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString())
                            });
                        }
                    }
                }
            }

            return messages;
        }
    }
}
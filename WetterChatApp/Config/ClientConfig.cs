using System;
using System.IO;
using System.Text.Json;

namespace NimbusChat.WetterChatApp.Config
{
    public class ClientConfig
    {
        public string ApiBaseUrl { get; set; } = string.Empty;

        private static readonly string ConfigFileName = "clientsettings.json";

        public static ClientConfig Load()
        {
            var baseDir = AppContext.BaseDirectory;
            var fullPath = Path.Combine(baseDir, ConfigFileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Config file '{ConfigFileName}' not found at '{fullPath}'.");
            }

            var json = File.ReadAllText(fullPath);
            var config = JsonSerializer.Deserialize<ClientConfig>(json);

            if (config == null || string.IsNullOrWhiteSpace(config.ApiBaseUrl))
            {
                throw new InvalidOperationException("ApiBaseUrl is missing in clientsettings.json.");
            }

            return config;
        }
    }
}
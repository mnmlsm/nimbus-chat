using System;
using System.IO;
using System.Text.Json;

namespace NimbusChat.WetterChatApp.Config
{
    public class ClientConfig
    {
        // Resolution order (highest wins): environment variable ->
        // clientsettings.json next to the exe (if present AND a valid
        // http/https URL) -> these hardcoded defaults. Load() must never
        // throw and must never block app startup - every source is optional
        // and any failure just falls through to the next one.
        //
        // DefaultApiKey is not a meaningful secret: clientsettings.json ships
        // the exact same value in this public repo. It only exists to keep
        // casual scraping/abuse off the API (see API-Network-setup.txt).
        private const string DefaultApiBaseUrl = "https://api.memo-dev.uk";
        private const string DefaultApiKey = "ac3d7d81b8ba483aa2d6edd2d0673eaba40cf14adb244797a2d4e49a74749321";

        private static readonly string ConfigFileName = "clientsettings.json";

        public string ApiBaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;

        public static ClientConfig Load()
        {
            var config = new ClientConfig
            {
                ApiBaseUrl = DefaultApiBaseUrl,
                ApiKey = DefaultApiKey
            };

            ApplyFile(config);
            ApplyEnvironmentVariables(config);

            return config;
        }

        private static void ApplyFile(ClientConfig config)
        {
            try
            {
                var fullPath = Path.Combine(AppContext.BaseDirectory, ConfigFileName);
                if (!File.Exists(fullPath))
                    return;

                var json = File.ReadAllText(fullPath);
                var fileConfig = JsonSerializer.Deserialize<ClientConfig>(json);
                if (fileConfig == null)
                    return;

                if (IsValidBaseUrl(fileConfig.ApiBaseUrl))
                    config.ApiBaseUrl = fileConfig.ApiBaseUrl;

                if (!string.IsNullOrWhiteSpace(fileConfig.ApiKey))
                    config.ApiKey = fileConfig.ApiKey;
            }
            catch
            {
                // Missing, unreadable, or malformed JSON: keep whatever the
                // config already had (the built-in default at this point).
            }
        }

        private static void ApplyEnvironmentVariables(ClientConfig config)
        {
            try
            {
                var url = Environment.GetEnvironmentVariable("NIMBUSCHAT_API_BASE_URL");
                if (IsValidBaseUrl(url))
                    config.ApiBaseUrl = url;

                var key = Environment.GetEnvironmentVariable("NIMBUSCHAT_API_KEY");
                if (!string.IsNullOrWhiteSpace(key))
                    config.ApiKey = key;
            }
            catch
            {
                // Never let config resolution throw.
            }
        }

        private static bool IsValidBaseUrl(string url)
        {
            return !string.IsNullOrWhiteSpace(url)
                && Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}

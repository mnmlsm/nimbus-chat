namespace NimbusChat.Api.Security
{
    // Lightweight shared-key gate in front of every endpoint. Not a real auth
    // system - it only keeps casual scraping/abuse off the publicly tunneled
    // API. The client sends the same key back on every request (ApiClient.cs).
    public sealed class ApiKeyMiddleware
    {
        public const string HeaderName = "X-Api-Key";
        public const string RejectionMessage = "Missing or invalid API key.";

        private readonly RequestDelegate _next;
        private readonly string? _configuredApiKey;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuredApiKey = configuration["Security:ApiKey"];
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var providedKey = context.Request.Headers[HeaderName].ToString();

            if (!IsAuthorized(providedKey, _configuredApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(RejectionMessage);
                return;
            }

            await _next(context);
        }

        // An unconfigured key rejects everything on purpose: a blank server-side
        // key must never turn into "no gate at all".
        public static bool IsAuthorized(string? providedKey, string? configuredKey) =>
            !string.IsNullOrEmpty(configuredKey) &&
            string.Equals(providedKey, configuredKey, StringComparison.Ordinal);
    }
}

using NimbusChat.Api.Data;

namespace nimbus_chat.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddHttpClient();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Gitignored, local override for real secrets (DB password,
            // OpenWeather key, shared API key). appsettings.json only ever
            // holds committed placeholders - see appsettings.Local.json.
            builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

            var app = builder.Build();

            // Datenbank und Tabellen anlegen, falls noch nicht vorhanden.
            // Dadurch startet die App auch nach einem kompletten DB-Reset.
            try
            {
                DatabaseInitializer.Initialize(
                    app.Configuration.GetConnectionString("NimbusChatDatabase")!);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Startup] Database initialization failed: {ex.Message}");
                Console.WriteLine("[Startup] Continuing to start the API without a confirmed database connection.");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Lightweight shared-key gate in front of every endpoint. Not a
            // real auth system - just keeps casual scraping/abuse off the
            // publicly tunneled API. The client sends the same key back on
            // every request (see ApiClient.cs).
            var configuredApiKey = app.Configuration["Security:ApiKey"];
            app.Use(async (context, next) =>
            {
                var providedKey = context.Request.Headers["X-Api-Key"].ToString();

                if (string.IsNullOrEmpty(configuredApiKey) ||
                    !string.Equals(providedKey, configuredApiKey, StringComparison.Ordinal))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Missing or invalid API key.");
                    return;
                }

                await next();
            });

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

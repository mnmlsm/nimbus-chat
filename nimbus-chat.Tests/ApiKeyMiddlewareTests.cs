using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NimbusChat.Api.Security;
using Xunit;

namespace NimbusChat.Api.Tests
{
    public class ApiKeyMiddlewareTests
    {
        private const string ValidKey = "test-key-0123456789abcdef";

        private static IConfiguration ConfigWith(string? apiKey) =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { ["Security:ApiKey"] = apiKey })
                .Build();

        private static HttpContext ContextWith(string? headerValue)
        {
            var context = new DefaultHttpContext();         // fake request/response context for testing
            context.Response.Body = new MemoryStream();    // capture response body for testing

            if (headerValue != null)
                context.Request.Headers[ApiKeyMiddleware.HeaderName] = headerValue;

            return context;
        }

        private static async Task<string> ReadBodyAsync(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            return await new StreamReader(context.Response.Body).ReadToEndAsync();
        }

        // --- IsAuthorized (pure rule) ---

        [Fact]
        public void IsAuthorized_AcceptsMatchingKey()
        {
            Assert.True(ApiKeyMiddleware.IsAuthorized(ValidKey, ValidKey));
        }

        [Fact]
        public void IsAuthorized_RejectsWrongKey()
        {
            Assert.False(ApiKeyMiddleware.IsAuthorized("wrong-key", ValidKey));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsAuthorized_RejectsMissingKey(string? providedKey)
        {
            Assert.False(ApiKeyMiddleware.IsAuthorized(providedKey, ValidKey));
        }

        // A blank server-side key must never degrade into "no gate at all".
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsAuthorized_RejectsEverything_WhenNoKeyIsConfigured(string? configuredKey)
        {
            Assert.False(ApiKeyMiddleware.IsAuthorized(ValidKey, configuredKey));
            Assert.False(ApiKeyMiddleware.IsAuthorized(configuredKey, configuredKey));
        }

        [Fact]
        public void IsAuthorized_IsCaseSensitive()
        {
            Assert.False(ApiKeyMiddleware.IsAuthorized(ValidKey.ToUpperInvariant(), ValidKey));
        }

        // --- InvokeAsync (pipeline behaviour) ---

        [Fact]
        public async Task InvokeAsync_CallsNext_WhenKeyIsValid()
        {
            var nextWasCalled = false;
            var middleware = new ApiKeyMiddleware(_ => { nextWasCalled = true; return Task.CompletedTask; }, ConfigWith(ValidKey));
            var context = ContextWith(ValidKey);

            await middleware.InvokeAsync(context);

            Assert.True(nextWasCalled);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ShortCircuitsWith401_WhenHeaderIsMissing()
        {
            var nextWasCalled = false;
            var middleware = new ApiKeyMiddleware(_ => { nextWasCalled = true; return Task.CompletedTask; }, ConfigWith(ValidKey)); // fake next
            var context = ContextWith(null);

            await middleware.InvokeAsync(context);

            Assert.False(nextWasCalled); // next delegate should not be called
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
            Assert.Equal(ApiKeyMiddleware.RejectionMessage, await ReadBodyAsync(context));
        }

        [Fact]
        public async Task InvokeAsync_ShortCircuitsWith401_WhenKeyIsWrong()
        {
            var nextWasCalled = false;
            var middleware = new ApiKeyMiddleware(_ => { nextWasCalled = true; return Task.CompletedTask; }, ConfigWith(ValidKey));
            var context = ContextWith("wrong-key");

            await middleware.InvokeAsync(context);

            Assert.False(nextWasCalled);
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ShortCircuitsWith401_WhenServerHasNoKeyConfigured()
        {
            var nextWasCalled = false;
            var middleware = new ApiKeyMiddleware(_ => { nextWasCalled = true; return Task.CompletedTask; }, ConfigWith(null));
            var context = ContextWith(ValidKey);

            await middleware.InvokeAsync(context);

            Assert.False(nextWasCalled);
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        }
    }
}

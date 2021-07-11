using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Reddit.NET.Client.Authentication;
using Reddit.NET.Client.Authentication.Credential;
using Reddit.NET.Client.Command;
using Reddit.NET.Client.Command.RateLimiting;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.UnitTests.Shared;

namespace Reddit.NET.Client.UnitTests.Authentication
{
    [SuppressMessage("Reliability", "CA2012", Justification = "ValueTask result is not used in test assertions.")]
    public class AuthenticatorTests
    {
        private ILogger<CommandExecutor> _logger;
        private IHttpClientFactory _httpClientFactory;
        private IRateLimiter _rateLimiter;
        private MockHttpMessageHandler _httpMessageHandler;
        private CommandExecutor _commandExecutor;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger<CommandExecutor>>();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _rateLimiter = Substitute.For<IRateLimiter>();
            _httpMessageHandler = new MockHttpMessageHandler();

            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

            _httpClientFactory
                .CreateClient(Constants.HttpClientName)
                .Returns(new HttpClient(_httpMessageHandler));

            var successfulLease = SuccessfulLease();

            _rateLimiter
                .AcquireAsync(Arg.Any<int>())
                .Returns(successfulLease);

            _commandExecutor = new CommandExecutor(
                _logger,
                _httpClientFactory,
                _rateLimiter);
        }

        [Test]
        public async Task GetAuthenticationContextAsync_ClientCredentialsAuthenticatorNoExistingContext_CreatesContextAndReturnsContext()
        {
            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(Tokens.ClientCredentials)
                });

            var credentials = new NonInteractiveCredentials(
                mode: AuthenticationMode.ReadOnly,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                deviceId: Guid.NewGuid());

            var authenticator = new ClientCredentialsAuthenticator(
                Substitute.For<ILogger<ClientCredentialsAuthenticator>>(),
                _commandExecutor,
                credentials);

            var context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);
            Assert.AreEqual(context.Id, "Client Credentials");
            Assert.IsNotNull(context.Token);
            Assert.AreEqual("...", context.Token.AccessToken);
            Assert.AreEqual("bearer", context.Token.TokenType);
            Assert.AreEqual(3600, context.Token.ExpiresIn);
            Assert.AreEqual("*", context.Token.Scope);
            Assert.AreEqual("...", context.Token.RefreshToken);

            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            var request = _httpMessageHandler.SeenRequests[0];
            var requestContent = await (request.Content as FormUrlEncodedContent).ReadAsStringAsync();

            Assert.AreEqual("https://www.reddit.com/api/v1/access_token", request.RequestUri.AbsoluteUri);
            Assert.AreEqual("grant_type=client_credentials&duration=permanent", requestContent);
        }

        [Test]
        public async Task GetAuthenticationContextAsync_ClientCredentialsAuthenticatorExistingUnexpiredContext_ReturnsExistingContext()
        {
            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(Tokens.ClientCredentials)
                });

            var credentials = new NonInteractiveCredentials(
                mode: AuthenticationMode.ReadOnly,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                deviceId: Guid.NewGuid());

            var authenticator = new ClientCredentialsAuthenticator(
                Substitute.For<ILogger<ClientCredentialsAuthenticator>>(),
                _commandExecutor,
                credentials);

            var context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);
            Assert.AreEqual(context.Id, "Client Credentials");
            Assert.IsNotNull(context.Token);
            Assert.AreEqual("...", context.Token.AccessToken);
            Assert.AreEqual("bearer", context.Token.TokenType);
            Assert.AreEqual(3600, context.Token.ExpiresIn);
            Assert.AreEqual("*", context.Token.Scope);
            Assert.AreEqual("...", context.Token.RefreshToken);

            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            var request = _httpMessageHandler.SeenRequests[0];
            var requestContent = await (request.Content as FormUrlEncodedContent).ReadAsStringAsync();

            Assert.AreEqual("https://www.reddit.com/api/v1/access_token", request.RequestUri.AbsoluteUri);
            Assert.AreEqual("grant_type=client_credentials&duration=permanent", requestContent);

            // Get the context again.
            context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);

            // No further requests
            Assert.AreEqual(1, _httpMessageHandler.RequestCount);
        }

        [Test]
        public async Task GetAuthenticationContextAsync_ClientCredentialsAuthenticatorExistingExpiredContext_RefreshesContextAndReturnsContext()
        {
            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    // Token has a 2 second expiry
                    Content = new StringContent(Tokens.ClientCredentialsShortExpiry)
                });

            var credentials = new NonInteractiveCredentials(
                mode: AuthenticationMode.ReadOnly,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                deviceId: Guid.NewGuid());

            var authenticator = new ClientCredentialsAuthenticator(
                Substitute.For<ILogger<ClientCredentialsAuthenticator>>(),
                _commandExecutor,
                credentials);

            var context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);
            Assert.AreEqual(context.Id, "Client Credentials");
            Assert.IsNotNull(context.Token);
            Assert.AreEqual("...", context.Token.AccessToken);
            Assert.AreEqual("bearer", context.Token.TokenType);
            Assert.AreEqual(2, context.Token.ExpiresIn);
            Assert.AreEqual("*", context.Token.Scope);
            Assert.AreEqual("...", context.Token.RefreshToken);

            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            var request = _httpMessageHandler.SeenRequests[0];
            var requestContent = await (request.Content as FormUrlEncodedContent).ReadAsStringAsync();

            Assert.AreEqual("https://www.reddit.com/api/v1/access_token", request.RequestUri.AbsoluteUri);
            Assert.AreEqual("grant_type=client_credentials&duration=permanent", requestContent);

            // Ensure the token has expired.
            await Task.Delay(TimeSpan.FromSeconds(4));

            // Get the context again.
            context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);

            Assert.AreEqual(2, _httpMessageHandler.RequestCount);

            request = _httpMessageHandler.SeenRequests[1];
            requestContent = await (request.Content as FormUrlEncodedContent).ReadAsStringAsync();

            Assert.AreEqual("https://www.reddit.com/api/v1/access_token", request.RequestUri.AbsoluteUri);
            Assert.AreEqual("grant_type=refresh_token&refresh_token=...&duration=permanent", requestContent);
        }

        [Test]
        public async Task GetAuthenticationContextAsync_UsernamePasswordAuthenticatorNoExistingContext_CreatesContextAndReturnsContext()
        {
            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(Tokens.UsernamePassword)
                });

            var credentials = new NonInteractiveCredentials(
                mode: AuthenticationMode.Script,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                username: Guid.NewGuid().ToString(),
                password: Guid.NewGuid().ToString());

            var authenticator = new UsernamePasswordAuthenticator(
                Substitute.For<ILogger<UsernamePasswordAuthenticator>>(),
                _commandExecutor,
                credentials);

            var context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);
            Assert.AreEqual(context.Id, "Username + Password");
            Assert.IsNotNull(context.Token);
            Assert.AreEqual("...", context.Token.AccessToken);
            Assert.AreEqual("bearer", context.Token.TokenType);
            Assert.AreEqual(3600, context.Token.ExpiresIn);
            Assert.AreEqual("*", context.Token.Scope);
            Assert.AreEqual(null, context.Token.RefreshToken);

            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            var request = _httpMessageHandler.SeenRequests[0];
            var requestContent = await (request.Content as FormUrlEncodedContent).ReadAsStringAsync();

            Assert.AreEqual("https://www.reddit.com/api/v1/access_token", request.RequestUri.AbsoluteUri);
            Assert.AreEqual($"grant_type=password&username={credentials.Username}&password={credentials.Password}", requestContent);
        }

        [Test]
        public async Task GetAuthenticationContextAsync_UsernamePasswordAuthenticatorExistingUnexpiredContext_ReturnsExistingContext()
        {
            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(Tokens.UsernamePassword)
                });

            var credentials = new NonInteractiveCredentials(
                mode: AuthenticationMode.Script,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                username: Guid.NewGuid().ToString(),
                password: Guid.NewGuid().ToString());

            var authenticator = new UsernamePasswordAuthenticator(
                Substitute.For<ILogger<UsernamePasswordAuthenticator>>(),
                _commandExecutor,
                credentials);

            var context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);
            Assert.AreEqual(context.Id, "Username + Password");
            Assert.IsNotNull(context.Token);
            Assert.AreEqual("...", context.Token.AccessToken);
            Assert.AreEqual("bearer", context.Token.TokenType);
            Assert.AreEqual(3600, context.Token.ExpiresIn);
            Assert.AreEqual("*", context.Token.Scope);
            Assert.AreEqual(null, context.Token.RefreshToken);

            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            var request = _httpMessageHandler.SeenRequests[0];
            var requestContent = await (request.Content as FormUrlEncodedContent).ReadAsStringAsync();

            Assert.AreEqual("https://www.reddit.com/api/v1/access_token", request.RequestUri.AbsoluteUri);
            Assert.AreEqual($"grant_type=password&username={credentials.Username}&password={credentials.Password}", requestContent);

            // Get the context again.
            context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);

            // No further requests
            Assert.AreEqual(1, _httpMessageHandler.RequestCount);
        }

        [Test]
        public async Task GetAuthenticationContextAsync_UsernamePasswordAuthenticatorExistingExpiredContext_RefreshesContextAndReturnsContext()
        {
            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    // Token has a 2 second expiry
                    Content = new StringContent(Tokens.UsernamePasswordShortExpiry)
                });

            var credentials = new NonInteractiveCredentials(
                mode: AuthenticationMode.Script,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                username: Guid.NewGuid().ToString(),
                password: Guid.NewGuid().ToString());

            var authenticator = new UsernamePasswordAuthenticator(
                Substitute.For<ILogger<UsernamePasswordAuthenticator>>(),
                _commandExecutor,
                credentials);

            var context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);
            Assert.AreEqual(context.Id, "Username + Password");
            Assert.IsNotNull(context.Token);
            Assert.AreEqual("...", context.Token.AccessToken);
            Assert.AreEqual("bearer", context.Token.TokenType);
            Assert.AreEqual(2, context.Token.ExpiresIn);
            Assert.AreEqual("*", context.Token.Scope);
            Assert.AreEqual(null, context.Token.RefreshToken);

            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            var request = _httpMessageHandler.SeenRequests[0];
            var requestContent = await (request.Content as FormUrlEncodedContent).ReadAsStringAsync();

            Assert.AreEqual("https://www.reddit.com/api/v1/access_token", request.RequestUri.AbsoluteUri);
            Assert.AreEqual($"grant_type=password&username={credentials.Username}&password={credentials.Password}", requestContent);

            // Ensure the token has expired.
            await Task.Delay(TimeSpan.FromSeconds(4));

            // Get the context again.
            context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);

            Assert.AreEqual(2, _httpMessageHandler.RequestCount);

            request = _httpMessageHandler.SeenRequests[1];
            requestContent = await (request.Content as FormUrlEncodedContent).ReadAsStringAsync();

            Assert.AreEqual("https://www.reddit.com/api/v1/access_token", request.RequestUri.AbsoluteUri);
            Assert.AreEqual($"grant_type=password&username={credentials.Username}&password={credentials.Password}", requestContent);
        }

        [Test]
        public async Task GetAuthenticationContextAsync_UserTokenAuthenticatorNoExistingContext_CreatesContextAndReturnsContext()
        {
            var credentials = new InteractiveCredentials(
                mode: AuthenticationMode.WebApp,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                redirectUri: new Uri("http://localhost/redirect"),
                sessionId: Guid.NewGuid(),
                token: JsonSerializer.Deserialize<Token>(Tokens.AuthorizationCode));

            var authenticator = new UserTokenAuthenticator(
                Substitute.For<ILogger<UserTokenAuthenticator>>(),
                _commandExecutor,
                credentials);

            var context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);
            Assert.AreEqual(context.Id, "User Token");
            Assert.IsNotNull(context.Token);
            Assert.AreEqual("...", context.Token.AccessToken);
            Assert.AreEqual("bearer", context.Token.TokenType);
            Assert.AreEqual(3600, context.Token.ExpiresIn);
            Assert.AreEqual("*", context.Token.Scope);
            Assert.AreEqual("...", context.Token.RefreshToken);

            Assert.AreEqual(0, _httpMessageHandler.RequestCount);
        }

        [Test]
        public async Task GetAuthenticationContextAsync_UserTokenAuthenticatorExistingUnexpiredContext_ReturnsExistingContext()
        {
            var credentials = new InteractiveCredentials(
                mode: AuthenticationMode.WebApp,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                redirectUri: new Uri("http://localhost/redirect"),
                sessionId: Guid.NewGuid(),
                token: JsonSerializer.Deserialize<Token>(Tokens.AuthorizationCode));

            var authenticator = new UserTokenAuthenticator(
                Substitute.For<ILogger<UserTokenAuthenticator>>(),
                _commandExecutor,
                credentials);

            var context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);
            Assert.AreEqual(context.Id, "User Token");
            Assert.IsNotNull(context.Token);
            Assert.AreEqual("...", context.Token.AccessToken);
            Assert.AreEqual("bearer", context.Token.TokenType);
            Assert.AreEqual(3600, context.Token.ExpiresIn);
            Assert.AreEqual("*", context.Token.Scope);
            Assert.AreEqual("...", context.Token.RefreshToken);

            Assert.AreEqual(0, _httpMessageHandler.RequestCount);

            // Get the context again.
            context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);

            // Still no requests
            Assert.AreEqual(0, _httpMessageHandler.RequestCount);
        }

        [Test]
        public async Task GetAuthenticationContextAsync_UserTokenAuthenticatorExistingExpiredContext_RefreshesContextAndReturnsContext()
        {
            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(Tokens.AuthorizationCode)
                });

            var credentials = new InteractiveCredentials(
                mode: AuthenticationMode.WebApp,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                redirectUri: new Uri("http://localhost/redirect"),
                sessionId: Guid.NewGuid(),
                // Token has a 2 second expiry
                token: JsonSerializer.Deserialize<Token>(Tokens.AuthorizationCodeShortExpiry));

            var authenticator = new UserTokenAuthenticator(
                Substitute.For<ILogger<UserTokenAuthenticator>>(),
                _commandExecutor,
                credentials);

            var context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);
            Assert.AreEqual(context.Id, "User Token");
            Assert.IsNotNull(context.Token);
            Assert.AreEqual("...", context.Token.AccessToken);
            Assert.AreEqual("bearer", context.Token.TokenType);
            Assert.AreEqual(2, context.Token.ExpiresIn);
            Assert.AreEqual("*", context.Token.Scope);
            Assert.AreEqual("...", context.Token.RefreshToken);

            Assert.AreEqual(0, _httpMessageHandler.RequestCount);

            // Ensure the token has expired.
            await Task.Delay(TimeSpan.FromSeconds(4));

            // Get the context again.
            context = await authenticator.GetAuthenticationContextAsync();

            Assert.IsNotNull(context);

            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            var request = _httpMessageHandler.SeenRequests[0];
            var requestContent = await (request.Content as FormUrlEncodedContent).ReadAsStringAsync();

            Assert.AreEqual("https://www.reddit.com/api/v1/access_token", request.RequestUri.AbsoluteUri);
            Assert.AreEqual("grant_type=refresh_token&refresh_token=...&duration=permanent", requestContent);
        }

        private static ValueTask<PermitLease> SuccessfulLease()
        {
            var lease = Substitute.For<PermitLease>();

            lease.IsAcquired.Returns(true);

            return ValueTask.FromResult(lease);
        }

        private static class Tokens
        {
            public const string ClientCredentials = @"
{
    ""access_token"": ""..."",
    ""token_type"": ""bearer"",
    ""expires_in"": 3600,
    ""scope"": ""*"",
    ""refresh_token"": ""...""
}";

            public const string ClientCredentialsShortExpiry = @"
{
    ""access_token"": ""..."",
    ""token_type"": ""bearer"",
    ""expires_in"": 2,
    ""scope"": ""*"",
    ""refresh_token"": ""...""
}";

            public const string UsernamePassword = @"
{
    ""access_token"": ""..."",
    ""token_type"": ""bearer"",
    ""expires_in"": 3600,
    ""scope"": ""*""
}";

            public const string UsernamePasswordShortExpiry = @"
{
    ""access_token"": ""..."",
    ""token_type"": ""bearer"",
    ""expires_in"": 2,
    ""scope"": ""*""
}";

            public const string AuthorizationCode = @"
{
    ""access_token"": ""..."",
    ""token_type"": ""bearer"",
    ""expires_in"": 3600,
    ""scope"": ""*"",
    ""refresh_token"": ""...""
}";

            public const string AuthorizationCodeShortExpiry = @"
{
    ""access_token"": ""..."",
    ""token_type"": ""bearer"",
    ""expires_in"": 2,
    ""scope"": ""*"",
    ""refresh_token"": ""...""
}";
        }
    }
}

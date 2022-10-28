using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Authentication.Credential;
using Reddit.NET.Client.Builder;
using Reddit.NET.Client.Command;
using Reddit.NET.Client.Command.RateLimiting;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.UnitTests.Shared;

namespace Reddit.NET.Client.UnitTests.Authentication
{
    [SuppressMessage("Reliability", "CA2012", Justification = "ValueTask result is not used in test assertions.")]
    public class CredentialsBuilderTests
    {
        private ILogger<CommandExecutor> _logger;
        private IHttpClientFactory _httpClientFactory;
        private IRateLimiter _rateLimiter;
        private MockHttpMessageHandler _httpMessageHandler;
        private CommandExecutor _commandExecutor;
        private ITokenStorage _tokenStorage;

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
            _tokenStorage = Substitute.For<ITokenStorage>();
        }

        [Test]
        public async Task BuildCredentialsAsync_ScriptCredentials_ReturnsNonInteractiveCredentials()
        {
            var clientId = Guid.NewGuid().ToString();
            var clientSecret = Guid.NewGuid().ToString();
            var username = Guid.NewGuid().ToString();
            var password = Guid.NewGuid().ToString();

            var credentialsBuilder = CredentialsBuilder.New;

            credentialsBuilder.Script(
                clientId: clientId,
                clientSecret: clientSecret,
                username: username,
                password: password);

            var credentials = await credentialsBuilder.BuildCredentialsAsync(
                _commandExecutor,
                _tokenStorage);

            Assert.IsNotNull(credentials);
            Assert.IsInstanceOf<NonInteractiveCredentials>(credentials);
            Assert.AreEqual(AuthenticationMode.Script, credentials.Mode);
            Assert.AreEqual(clientId, credentials.ClientId);
            Assert.AreEqual(clientSecret, credentials.ClientSecret);
            Assert.AreEqual(username, credentials.Username);
            Assert.AreEqual(password, credentials.Password);
            Assert.IsNull(credentials.RedirectUri);
            Assert.IsNull(credentials.DeviceId);

            _httpClientFactory
                .Received(0)
                .CreateClient(Arg.Any<string>());
        }

        [Test]
        public async Task BuildCredentialsAsync_ReadOnlyCredentials_ReturnsNonInteractiveCredentials()
        {
            var clientId = Guid.NewGuid().ToString();
            var clientSecret = Guid.NewGuid().ToString();
            var deviceId = Guid.NewGuid();

            var credentialsBuilder = CredentialsBuilder.New;

            credentialsBuilder.ReadOnly(
                clientId: clientId,
                clientSecret: clientSecret,
                deviceId: deviceId);

            var credentials = await credentialsBuilder.BuildCredentialsAsync(
                _commandExecutor,
                _tokenStorage);

            Assert.IsNotNull(credentials);
            Assert.IsInstanceOf<NonInteractiveCredentials>(credentials);
            Assert.AreEqual(AuthenticationMode.ReadOnly, credentials.Mode);
            Assert.AreEqual(clientId, credentials.ClientId);
            Assert.AreEqual(clientSecret, credentials.ClientSecret);
            Assert.IsNull(credentials.Username);
            Assert.IsNull(credentials.Password);
            Assert.IsNull(credentials.RedirectUri);
            Assert.AreEqual(deviceId, credentials.DeviceId);

            _httpClientFactory
                .Received(0)
                .CreateClient(Arg.Any<string>());
        }

        [Test]
        public async Task BuildCredentialsAsync_ReadOnlyInstalledAppCredentials_ReturnsNonInteractiveCredentials()
        {
            var clientId = Guid.NewGuid().ToString();
            var deviceId = Guid.NewGuid();

            var credentialsBuilder = CredentialsBuilder.New;

            credentialsBuilder.ReadOnlyInstalledApp(
                clientId: clientId,
                deviceId: deviceId);

            var credentials = await credentialsBuilder.BuildCredentialsAsync(
                _commandExecutor,
                _tokenStorage);

            Assert.IsNotNull(credentials);
            Assert.IsInstanceOf<NonInteractiveCredentials>(credentials);
            Assert.AreEqual(AuthenticationMode.ReadOnlyInstalledApp, credentials.Mode);
            Assert.AreEqual(clientId, credentials.ClientId);
            Assert.AreEqual(string.Empty, credentials.ClientSecret);
            Assert.IsNull(credentials.Username);
            Assert.IsNull(credentials.Password);
            Assert.IsNull(credentials.RedirectUri);
            Assert.AreEqual(deviceId, credentials.DeviceId);

            _httpClientFactory
                .Received(0)
                .CreateClient(Arg.Any<string>());
        }

        [Test]
        public void GetAuthorizationUri_WebAppCredentials_ReturnsCorrectAuthorizationUri()
        {
            var clientId = Guid.NewGuid().ToString();
            var clientSecret = Guid.NewGuid().ToString();
            var redirectUri = "http://localhost/redirect";
            var state = Guid.NewGuid().ToString();

            var credentialsBuilder = CredentialsBuilder.New;

            var interactiveCredentialsBuilder = credentialsBuilder.WebApp(
                clientId: clientId,
                clientSecret: clientSecret,
                redirectUri: new Uri(redirectUri),
                state: state);

            Assert.IsNotNull(interactiveCredentialsBuilder);

            var authorizationUri = interactiveCredentialsBuilder.GetAuthorizationUri();

            Assert.IsNotNull(authorizationUri);
            Assert.AreEqual(Uri.UriSchemeHttps, authorizationUri.Scheme);
            Assert.AreEqual("www.reddit.com", authorizationUri.Host);
            Assert.AreEqual("/api/v1/authorize", authorizationUri.AbsolutePath);

            var queryString = HttpUtility.ParseQueryString(authorizationUri.Query);

            Assert.AreEqual(clientId, queryString["client_id"]);
            Assert.AreEqual("code", queryString["response_type"]);
            Assert.AreEqual(state, queryString["state"]);
            Assert.AreEqual(redirectUri, queryString["redirect_uri"]);
            Assert.AreEqual("permanent", queryString["duration"]);
            Assert.AreEqual("subscribe vote mysubreddits submit save read privatemessages identity account edit history flair", queryString["scope"]);
        }

        [Test]
        public void GetAuthorizationUri_InstalledAppCredentials_ReturnsCorrectAuthorizationUri()
        {
            var clientId = Guid.NewGuid().ToString();
            var clientSecret = Guid.NewGuid().ToString();
            var redirectUri = "http://localhost/redirect";
            var state = Guid.NewGuid().ToString();

            var credentialsBuilder = CredentialsBuilder.New;

            var interactiveCredentialsBuilder = credentialsBuilder.InstalledApp(
                clientId: clientId,
                redirectUri: new Uri(redirectUri),
                state: state);

            Assert.IsNotNull(interactiveCredentialsBuilder);

            var authorizationUri = interactiveCredentialsBuilder.GetAuthorizationUri();

            Assert.IsNotNull(authorizationUri);
            Assert.AreEqual(Uri.UriSchemeHttps, authorizationUri.Scheme);
            Assert.AreEqual("www.reddit.com", authorizationUri.Host);
            Assert.AreEqual("/api/v1/authorize", authorizationUri.AbsolutePath);

            var queryString = HttpUtility.ParseQueryString(authorizationUri.Query);

            Assert.AreEqual(clientId, queryString["client_id"]);
            Assert.AreEqual("code", queryString["response_type"]);
            Assert.AreEqual(state, queryString["state"]);
            Assert.AreEqual(redirectUri, queryString["redirect_uri"]);
            Assert.AreEqual("permanent", queryString["duration"]);
            Assert.AreEqual("subscribe vote mysubreddits submit save read privatemessages identity account edit history flair", queryString["scope"]);
        }

        [Test]
        [TestCase(AuthenticationMode.WebApp)]
        [TestCase(AuthenticationMode.InstalledApp)]
        public void GetAuthorizationUri_SessionCredentials_ThrowsInvalidOperationException(AuthenticationMode mode)
        {
            var clientId = Guid.NewGuid().ToString();
            var clientSecret = Guid.NewGuid().ToString();
            var redirectUri = "http://localhost/redirect";
            var sessionId = Guid.NewGuid();

            var credentialsBuilder = CredentialsBuilder.New;

            var interactiveCredentialsBuilder = credentialsBuilder.Session(
                mode: mode,
                clientId: clientId,
                clientSecret: clientSecret,
                redirectUri: new Uri(redirectUri),
                sessionId: sessionId);

            Assert.IsNotNull(interactiveCredentialsBuilder);

            Assert.Throws<InvalidOperationException>(() => interactiveCredentialsBuilder.GetAuthorizationUri());
        }

        [Test]
        public async Task BuildCredentialsAsync_AuthorizedWithCodeSuccessfulResponse_ReturnsInteractiveCredentials()
        {
            const string TokenString = @"
{
    ""access_token"": ""..."",
    ""token_type"": ""bearer"",
    ""expires_in"": 3600,
    ""scope"": ""*"",
    ""refresh_token"": ""...""
}";

            var clientId = Guid.NewGuid().ToString();
            var clientSecret = Guid.NewGuid().ToString();
            var redirectUri = "http://localhost/redirect";
            var state = Guid.NewGuid().ToString();
            var code = Guid.NewGuid().ToString();
            var sessionId = Guid.NewGuid();

            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(TokenString)
                });

            _tokenStorage
                .StoreTokenAsync(Arg.Any<Token>())
                .Returns(sessionId);

            var credentialsBuilder = CredentialsBuilder.New;

            var interactiveCredentialsBuilder = credentialsBuilder.WebApp(
                clientId: clientId,
                clientSecret: clientSecret,
                redirectUri: new Uri(redirectUri),
                state: state);

            Assert.IsNotNull(interactiveCredentialsBuilder);

            interactiveCredentialsBuilder.Authorize(code);

            var credentials = await credentialsBuilder.BuildCredentialsAsync(
                _commandExecutor,
                _tokenStorage);

            Assert.IsNotNull(credentials);
            Assert.IsInstanceOf<InteractiveCredentials>(credentials);
            Assert.AreEqual(AuthenticationMode.WebApp, credentials.Mode);
            Assert.AreEqual(clientId, credentials.ClientId);
            Assert.AreEqual(clientSecret, credentials.ClientSecret);
            Assert.IsNull(credentials.Username);
            Assert.IsNull(credentials.Password);
            Assert.AreEqual(new Uri(redirectUri), credentials.RedirectUri);
            Assert.IsNull(credentials.DeviceId);

            var interactiveCredentials = credentials as InteractiveCredentials;

            Assert.IsNotNull(interactiveCredentials);
            Assert.AreEqual(sessionId, interactiveCredentials.SessionId);
            Assert.IsNotNull(interactiveCredentials.Token);
            Assert.AreEqual("...", interactiveCredentials.Token.AccessToken);
            Assert.AreEqual("bearer", interactiveCredentials.Token.TokenType);
            Assert.AreEqual(3600, interactiveCredentials.Token.ExpiresIn);
            Assert.AreEqual("*", interactiveCredentials.Token.Scope);
            Assert.AreEqual("...", interactiveCredentials.Token.RefreshToken);

            _ = _tokenStorage
                .Received(1)
                .StoreTokenAsync(Arg.Is<Token>(t =>
                    t.AccessToken == "..." &&
                    t.TokenType == "bearer" &&
                    t.ExpiresIn == 3600 &&
                    t.Scope == "*" &&
                    t.RefreshToken == "..."));
        }

        [Test]
        public void BuildCredentialsAsync_AuthorizedWithCodeUnsuccessfulResponse_ThrowsRedditClientResponseException()
        {
            var clientId = Guid.NewGuid().ToString();
            var clientSecret = Guid.NewGuid().ToString();
            var redirectUri = "http://localhost/redirect";
            var state = Guid.NewGuid().ToString();
            var code = Guid.NewGuid().ToString();
            var sessionId = Guid.NewGuid();

            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));

            _tokenStorage
                .StoreTokenAsync(Arg.Any<Token>())
                .Returns(sessionId);

            var credentialsBuilder = CredentialsBuilder.New;

            var interactiveCredentialsBuilder = credentialsBuilder.WebApp(
                clientId: clientId,
                clientSecret: clientSecret,
                redirectUri: new Uri(redirectUri),
                state: state);

            Assert.IsNotNull(interactiveCredentialsBuilder);

            interactiveCredentialsBuilder.Authorize(code);

            var exception = Assert.ThrowsAsync<RedditClientResponseException>(async () =>
                await credentialsBuilder.BuildCredentialsAsync(
                    _commandExecutor,
                    _tokenStorage));

            Assert.IsNotNull(exception);
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);


            _ = _tokenStorage
                .DidNotReceive()
                .StoreTokenAsync(Arg.Any<Token>());
        }

        [Test]
        [TestCase(AuthenticationMode.WebApp)]
        [TestCase(AuthenticationMode.InstalledApp)]
        public async Task BuildCredentialsAsync_AuthorizedWithValidSessionId_ReturnsInteractiveCredentials(AuthenticationMode mode)
        {
            const string TokenString = @"
{
    ""access_token"": ""..."",
    ""token_type"": ""bearer"",
    ""expires_in"": 3600,
    ""scope"": ""*"",
    ""refresh_token"": ""...""
}";

            var clientId = Guid.NewGuid().ToString();
            var clientSecret = Guid.NewGuid().ToString();
            var redirectUri = "http://localhost/redirect";
            var sessionId = Guid.NewGuid();

            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(TokenString)
                });

            _tokenStorage
                .GetTokenAsync(sessionId)
                .Returns(JsonSerializer.Deserialize<Token>(TokenString));

            var credentialsBuilder = CredentialsBuilder.New;

            var interactiveCredentialsBuilder = credentialsBuilder.Session(
                mode: mode,
                clientId: clientId,
                clientSecret: clientSecret,
                redirectUri: new Uri(redirectUri),
                sessionId: sessionId);

            Assert.IsNotNull(interactiveCredentialsBuilder);

            var credentials = await credentialsBuilder.BuildCredentialsAsync(
                _commandExecutor,
                _tokenStorage);

            Assert.IsNotNull(credentials);
            Assert.IsInstanceOf<InteractiveCredentials>(credentials);
            Assert.AreEqual(mode, credentials.Mode);
            Assert.AreEqual(clientId, credentials.ClientId);
            Assert.AreEqual(clientSecret, credentials.ClientSecret);
            Assert.IsNull(credentials.Username);
            Assert.IsNull(credentials.Password);
            Assert.AreEqual(new Uri(redirectUri), credentials.RedirectUri);
            Assert.IsNull(credentials.DeviceId);

            var interactiveCredentials = credentials as InteractiveCredentials;

            Assert.IsNotNull(interactiveCredentials);
            Assert.AreEqual(sessionId, interactiveCredentials.SessionId);
            Assert.IsNotNull(interactiveCredentials.Token);
            Assert.AreEqual("...", interactiveCredentials.Token.AccessToken);
            Assert.AreEqual("bearer", interactiveCredentials.Token.TokenType);
            Assert.AreEqual(3600, interactiveCredentials.Token.ExpiresIn);
            Assert.AreEqual("*", interactiveCredentials.Token.Scope);
            Assert.AreEqual("...", interactiveCredentials.Token.RefreshToken);

            _ = _tokenStorage
                .Received(1)
                .GetTokenAsync(sessionId);

            _httpClientFactory
                .DidNotReceive()
                .CreateClient(Arg.Any<string>());
        }

        private static ValueTask<PermitLease> SuccessfulLease()
        {
            var lease = Substitute.For<PermitLease>();

            lease.IsAcquired.Returns(true);

            return ValueTask.FromResult(lease);
        }
    }
}

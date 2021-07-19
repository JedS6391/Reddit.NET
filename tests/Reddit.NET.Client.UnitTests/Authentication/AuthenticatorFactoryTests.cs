using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Reddit.NET.Client.Authentication;
using Reddit.NET.Client.Authentication.Credential;
using Reddit.NET.Client.Command;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.UnitTests.Authentication
{
    public class AuthenticatorFactoryTests
    {
        private CommandExecutor _commandExecutor;
        private AuthenticatorFactory _authenticatorFactory;

        [SetUp]
        public void Setup()
        {
            _commandExecutor = new CommandExecutor(
                Substitute.For<ILogger<CommandExecutor>>(),
                Substitute.For<IHttpClientFactory>());

            _authenticatorFactory = new AuthenticatorFactory(
                Substitute.For<ILoggerFactory>(),
                _commandExecutor);
        }

        [Test]
        public void GetAuthenticator_NonInteractiveReadOnlyCredentials_ReturnsClientCredentialsAuthenticator()
        {
            var credentials = new NonInteractiveCredentials(
                mode: AuthenticationMode.ReadOnly,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                deviceId: Guid.NewGuid());

            var authenticator = _authenticatorFactory.GetAuthenticator(credentials);

            Assert.IsNotNull(authenticator);
            Assert.IsInstanceOf<ClientCredentialsAuthenticator>(authenticator);
        }

        [Test]
        public void GetAuthenticator_NonInteractiveReadOnlyInstalledAppCredentials_ReturnsInstalledClientAuthenticator()
        {
            var credentials = new NonInteractiveCredentials(
                mode: AuthenticationMode.ReadOnlyInstalledApp,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: string.Empty,
                deviceId: Guid.NewGuid());

            var authenticator = _authenticatorFactory.GetAuthenticator(credentials);

            Assert.IsNotNull(authenticator);
            Assert.IsInstanceOf<InstalledClientAuthenticator>(authenticator);
        }

        [Test]
        public void GetAuthenticator_NonInteractiveScriptCredentials_ReturnsUsernamePasswordAuthenticator()
        {
            var credentials = new NonInteractiveCredentials(
                mode: AuthenticationMode.Script,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                username: Guid.NewGuid().ToString(),
                password: Guid.NewGuid().ToString());

            var authenticator = _authenticatorFactory.GetAuthenticator(credentials);

            Assert.IsNotNull(authenticator);
            Assert.IsInstanceOf<UsernamePasswordAuthenticator>(authenticator);
        }

        [Test]
        public void GetAuthenticator_InteractiveWebAppCredentials_ReturnsUserTokenAuthenticator()
        {
            var credentials = new InteractiveCredentials(
                mode: AuthenticationMode.WebApp,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                redirectUri: new Uri("http://localhost/redirect"),
                sessionId: Guid.NewGuid(),
                token: new Token());

            var authenticator = _authenticatorFactory.GetAuthenticator(credentials);

            Assert.IsNotNull(authenticator);
            Assert.IsInstanceOf<UserTokenAuthenticator>(authenticator);
        }

        [Test]
        public void GetAuthenticator_InteractiveInstalledAppCredentials_ReturnsUserTokenAuthenticator()
        {
            var credentials = new InteractiveCredentials(
                mode: AuthenticationMode.InstalledApp,
                clientId: Guid.NewGuid().ToString(),
                clientSecret: Guid.NewGuid().ToString(),
                redirectUri: new Uri("http://localhost/redirect"),
                sessionId: Guid.NewGuid(),
                token: new Token());

            var authenticator = _authenticatorFactory.GetAuthenticator(credentials);

            Assert.IsNotNull(authenticator);
            Assert.IsInstanceOf<UserTokenAuthenticator>(authenticator);
        }
    }
}

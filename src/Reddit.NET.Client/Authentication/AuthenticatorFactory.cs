using System;
using Microsoft;
using Microsoft.Extensions.Logging;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Authentication.Credential;
using Reddit.NET.Client.Command;

namespace Reddit.NET.Client.Authentication
{
    /// <summary>
    /// Responsible for creating <see cref="IAuthenticator" /> instances.
    /// </summary>
    internal class AuthenticatorFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly CommandExecutor _commandExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatorFactory" /> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// A <see cref="ILoggerFactory" /> instance to create loggers for <see cref="IAuthenticator" /> instances.</param>
        /// <param name="commandExecutor">
        /// An <see cref="CommandExecutor" /> instance provided to created <see cref="IAuthenticator" /> instances.
        /// </param>
        public AuthenticatorFactory(ILoggerFactory loggerFactory, CommandExecutor commandExecutor)
        {
            _loggerFactory = Requires.NotNull(loggerFactory, nameof(loggerFactory));
            _commandExecutor = Requires.NotNull(commandExecutor, nameof(commandExecutor));
        }

        /// <summary>
        /// Gets an <see cref="IAuthenticator" /> instance appropriate for the provided credentials.
        /// </summary>
        /// <param name="credentials">Credentials used to determine the appropriate authentication.</param>
        /// <returns>An <see cref="IAuthenticator" /> instance.</returns>
        public IAuthenticator GetAuthenticator(Credentials credentials)
        {
            return credentials switch
            {
                NonInteractiveCredentials c => GetNonInteractiveAuthenticator(c),
                InteractiveCredentials c => GetInteractiveAuthenticator(c),
                _ => throw new ArgumentException($"Unsupported credential type {credentials.GetType().FullName}")
            };
        }

        private IAuthenticator GetNonInteractiveAuthenticator(NonInteractiveCredentials credentials)
        {
            return credentials.Mode switch
            {
                AuthenticationMode.Script =>
                    new UsernamePasswordAuthenticator(
                        _loggerFactory.CreateLogger<UsernamePasswordAuthenticator>(),
                        _commandExecutor,
                        credentials),

                AuthenticationMode.ReadOnly =>
                    new ClientCredentialsAuthenticator(
                        _loggerFactory.CreateLogger<ClientCredentialsAuthenticator>(),
                        _commandExecutor,
                        credentials),

                AuthenticationMode.ReadOnlyInstalledApp =>
                    new InstalledClientAuthenticator(
                        _loggerFactory.CreateLogger<InstalledClientAuthenticator>(),
                        _commandExecutor,
                        credentials),

                _ => throw new ArgumentException($"Mode '{credentials.Mode}' is not supported for non-interactive authentication.", nameof(credentials)),
            };
        }

        private IAuthenticator GetInteractiveAuthenticator(InteractiveCredentials credentials)
        {
            return credentials.Mode switch
            {
                AuthenticationMode.WebApp or AuthenticationMode.InstalledApp =>
                    new UserTokenAuthenticator(
                        _loggerFactory.CreateLogger<UserTokenAuthenticator>(),
                        _commandExecutor,
                        credentials),

                _ => throw new ArgumentException($"Mode '{credentials.Mode}' is not supported for interactive authentication.", nameof(credentials)),
            };
        }
    }
}

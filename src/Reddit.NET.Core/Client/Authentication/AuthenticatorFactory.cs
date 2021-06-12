using System;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;

namespace Reddit.NET.Core.Client.Authentication
{
    internal class AuthenticatorFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly CommandExecutor _commandExecutor;

        public AuthenticatorFactory(ILoggerFactory loggerFactory, CommandExecutor commandExecutor)
        {
            _loggerFactory = loggerFactory;
            _commandExecutor = commandExecutor;
        }

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
                    throw new NotSupportedException($"Mode '{credentials.Mode}' is not supported for non-interactive authentication."),
                    
                // TODO: Should use the installed_client grant as described here:
                // https://github.com/reddit-archive/reddit/wiki/OAuth2#application-only-oauth
                _ => throw new ArgumentException($"Mode '{credentials.Mode}' is not supported for non-interactive authentication.", nameof(credentials)),
            };
        }

        private IAuthenticator GetInteractiveAuthenticator(InteractiveCredentials credentials)
        {
            return credentials.Mode switch
            {
                AuthenticationMode.WebApp or AuthenticationMode.WebApp => 
                    new UserTokenAuthenticator(
                        _loggerFactory.CreateLogger<UserTokenAuthenticator>(),
                        _commandExecutor,
                        credentials),

                _ => throw new ArgumentException($"Mode '{credentials.Mode}' is not supported for interactive authentication.", nameof(credentials)),
            };
        }
    }
}
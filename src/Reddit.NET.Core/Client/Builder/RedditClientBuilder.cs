using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Builder.Exceptions;
using Reddit.NET.Core.Client.Command;

namespace Reddit.NET.Core.Client.Builder
{
    public class RedditClientBuilder
    {
        private IHttpClientFactory _httpClientFactory;
        private ILoggerFactory _loggerFactory;
        private UserRefreshTokenAuthenticator.AuthenticationDetails _userRefreshTokenAuthenticationDetails;
        private UsernamePasswordAuthenticator.AuthenticationDetails  _usernamePasswordAuthenticationDetails;
        private ClientCredentialsAuthenticator.AuthenticationDetails _clientCredentialsAuthenticationDetails;

        public static RedditClientBuilder New => new RedditClientBuilder();

        private RedditClientBuilder()
        {
        }

        public RedditClientBuilder WithHttpClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            return this;
        }

        public RedditClientBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            return this;
        }

        public RedditClientBuilder WithUserRefreshTokenAuthentication(UserRefreshTokenAuthenticator.AuthenticationDetails authenticationDetails)
        {
            _userRefreshTokenAuthenticationDetails = authenticationDetails;
            
            return this;
        }

        public RedditClientBuilder WithUsernamePasswordAuthentication(UsernamePasswordAuthenticator.AuthenticationDetails authenticationDetails)
        {
            _usernamePasswordAuthenticationDetails = authenticationDetails;

            return this;
        }

        public RedditClientBuilder WithClientCredentialsAuthentication(ClientCredentialsAuthenticator.AuthenticationDetails authenticationDetails)
        {
            _clientCredentialsAuthenticationDetails = authenticationDetails;
            
            return this;
        }

        public RedditClient Build() 
        {
            CheckValidity();

            var commandFactory = new CommandFactory(_httpClientFactory, _loggerFactory);
            IAuthenticator authenticator = DetermineAuthenticator(commandFactory);

            return new RedditClient(commandFactory, authenticator);
        }

        private void CheckValidity()
        {            
            if (_httpClientFactory == null) 
            {
                throw new RedditClientBuilderException("No HTTP client factory configured.");
            }

            if (_loggerFactory == null) 
            {
                throw new RedditClientBuilderException("No logger factory configured.");
            }

            if (_usernamePasswordAuthenticationDetails == null && 
                _clientCredentialsAuthenticationDetails == null &&
                _userRefreshTokenAuthenticationDetails == null)
            {
                throw new RedditClientBuilderException("No authentication details configured.");
            }
        }

        private IAuthenticator DetermineAuthenticator(CommandFactory commandFactory) 
        {
            if (_userRefreshTokenAuthenticationDetails != null) 
            {
                return new UserRefreshTokenAuthenticator(
                    _loggerFactory.CreateLogger<UserRefreshTokenAuthenticator>(),
                    commandFactory,
                    _userRefreshTokenAuthenticationDetails
                );
            }

            if (_usernamePasswordAuthenticationDetails != null)
            {
                return new UsernamePasswordAuthenticator(
                    _loggerFactory.CreateLogger<UsernamePasswordAuthenticator>(),
                    commandFactory, 
                    _usernamePasswordAuthenticationDetails);
            }

            return new ClientCredentialsAuthenticator(
                _loggerFactory.CreateLogger<ClientCredentialsAuthenticator>(),
                commandFactory, 
                _clientCredentialsAuthenticationDetails);
        }
    }
}
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Builder.Exceptions;
using Reddit.NET.Core.Client.Command;

namespace Reddit.NET.Core.Client.Builder
{
    /// <summary>
    /// Provides the ability to create <see cref="RedditClient" /> instances.
    /// </summary>
    public class RedditClientBuilder
    {
        private IHttpClientFactory _httpClientFactory;
        private ILoggerFactory _loggerFactory;
        private Credentials _credentials;

        /// <summary>
        /// Creates a new <see cref="RedditClientBuilder" /> instance to start the build process.
        /// </summary>
        public static RedditClientBuilder New => new RedditClientBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditClientBuilder" /> class.
        /// </summary>
        private RedditClientBuilder()
        {
        }

        /// <summary>
        /// Configures the builder to use the provided <see cref="IHttpClientFactory" /> instance when making HTTP calls.
        /// </summary>
        /// <param name="httpClientFactory">A <see cref="IHttpClientFactory" /> instance.</param>
        /// <returns>The updated builder.</returns>
        public RedditClientBuilder WithHttpClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            return this;
        }

        /// <summary>
        /// Configures the builder to use the provided <see cref="ILoggerFactory" /> instance when logging messages.
        /// </summary>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory" /> instance.</param>
        /// <returns>The updated builder.</returns>
        public RedditClientBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            return this;
        }

        public RedditClientBuilder WithCredentials(Credentials credentials)
        {
            _credentials = credentials;

            return this;
        }

        // /// <summary>
        // /// Configures the builder to use <see cref="UserRefreshTokenAuthenticator" /> for authentication.
        // /// </summary>
        // /// <param name="authenticationDetails">A <see cref="UserRefreshTokenAuthenticator.AuthenticationDetails" /> instance.</param>
        // /// <returns>The updated builder.</returns>
        // public RedditClientBuilder WithUserRefreshTokenAuthentication(UserRefreshTokenAuthenticator.AuthenticationDetails authenticationDetails)
        // {
        //     _userRefreshTokenAuthenticationDetails = authenticationDetails;
            
        //     return this;
        // }

        // /// <summary>
        // /// Configures the builder to use <see cref="UsernamePasswordAuthenticator" /> for authentication.
        // /// </summary>
        // /// <param name="authenticationDetails">A <see cref="UsernamePasswordAuthenticator.AuthenticationDetails" /> instance.</param>
        // /// <returns>The updated builder.</returns>
        // public RedditClientBuilder WithUsernamePasswordAuthentication(UsernamePasswordAuthenticator.AuthenticationDetails authenticationDetails)
        // {
        //     _usernamePasswordAuthenticationDetails = authenticationDetails;

        //     return this;
        // }

        // /// <summary>
        // /// Configures the builder to use <see cref="ClientCredentialsAuthenticator" /> for authentication.
        // /// </summary>
        // /// <param name="authenticationDetails">A <see cref="ClientCredentialsAuthenticator.AuthenticationDetails" /> instance.</param>
        // /// <returns>The updated builder.</returns>
        // public RedditClientBuilder WithClientCredentialsAuthentication(ClientCredentialsAuthenticator.AuthenticationDetails authenticationDetails)
        // {
        //     _clientCredentialsAuthenticationDetails = authenticationDetails;
            
        //     return this;
        // }

        /// <summary>
        /// Creates a <see cref="RedditClient" /> instance based on the builder configuration.
        /// </summary>
        /// <returns>A <see cref="RedditClient" /> instance configured based on the builder.</returns>
        public RedditClient Build() 
        {
            CheckValidity();

            var commandFactory = new CommandFactory(_httpClientFactory, _loggerFactory);
            var authenticatorFactory = new AuthenticatorFactory(_loggerFactory, commandFactory);
            var authenticator = authenticatorFactory.GetAuthenticator(_credentials);

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

            if (_credentials == null)
            {
                throw new RedditClientBuilderException("No credentials configured.");
            }
        }
    }
}
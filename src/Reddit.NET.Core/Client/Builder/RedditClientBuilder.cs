using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication;
using Reddit.NET.Core.Client.Builder.Exceptions;
using Reddit.NET.Core.Client.Command;

namespace Reddit.NET.Core.Client.Builder
{
    /// <summary>
    /// Provides the ability to create <see cref="RedditClient" /> instances.
    /// </summary>
    public sealed class RedditClientBuilder
    {
        private IHttpClientFactory _httpClientFactory;
        private ILoggerFactory _loggerFactory;
        private Action<CredentialsBuilder> _credentialsBuilderConfigurationAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditClientBuilder" /> class.
        /// </summary>
        private RedditClientBuilder()
        {
        }

        /// <summary>
        /// Creates a new <see cref="RedditClientBuilder" /> instance to start the build process.
        /// </summary>
        public static RedditClientBuilder New => new RedditClientBuilder();

        /// <summary>
        /// Configures the builder to use the provided <see cref="ILoggerFactory" /> instance when logging messages.
        /// </summary>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory" /> instance.</param>
        /// <returns>The updated builder.</returns>
        public RedditClientBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            Requires.NotNull(loggerFactory, nameof(loggerFactory));

            _loggerFactory = loggerFactory;

            return this;
        }

        /// <summary>
        /// Configures the builder to use the provided <see cref="IHttpClientFactory" /> instance when making HTTP calls.
        /// </summary>
        /// <param name="httpClientFactory">A <see cref="IHttpClientFactory" /> instance.</param>
        /// <returns>The updated builder.</returns>
        public RedditClientBuilder WithHttpClientFactory(IHttpClientFactory httpClientFactory)
        {
            Requires.NotNull(httpClientFactory, nameof(httpClientFactory));

            _httpClientFactory = httpClientFactory;

            return this;
        }

        /// <summary>
        /// Configures the builder to use the provided <see cref="Action{T}" /> to configure the credentials used by the client.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure credentials.</param>
        /// <returns>The updated builder.</returns>
        public RedditClientBuilder WithCredentialsConfiguration(Action<CredentialsBuilder> configurationAction)
        {
            Requires.NotNull(configurationAction, nameof(configurationAction));

            _credentialsBuilderConfigurationAction = configurationAction;

            return this;
        }

        /// <summary>
        /// Creates a <see cref="RedditClient" /> instance based on the builder configuration.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation. The result contains a <see cref="RedditClient" /> instance configured based on the builder.
        /// </returns>
        public async Task<RedditClient> BuildAsync() 
        {
            CheckValidity();

            var commandExecutor = new CommandExecutor(
                _loggerFactory.CreateLogger<CommandExecutor>(),
                _httpClientFactory);

            var authenticatorFactory = new AuthenticatorFactory(
                _loggerFactory, 
                commandExecutor);

            var credentialsBuilder = new CredentialsBuilder();

            _credentialsBuilderConfigurationAction.Invoke(credentialsBuilder);

            var credentials = await credentialsBuilder.BuildCredentialsAsync(commandExecutor).ConfigureAwait(false);

            var authenticator = authenticatorFactory.GetAuthenticator(credentials);

            return new RedditClient(
                _loggerFactory.CreateLogger<RedditClient>(), 
                commandExecutor, 
                authenticator);
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

            if (_credentialsBuilderConfigurationAction == null)
            {
                throw new RedditClientBuilderException("No credentials configured.");
            }
        }
    }
}
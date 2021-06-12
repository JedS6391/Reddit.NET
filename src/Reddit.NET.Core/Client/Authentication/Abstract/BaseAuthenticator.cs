using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command;

namespace Reddit.NET.Core.Client.Authentication.Abstract
{
    /// <summary>
    /// Provides base functionality for <see cref="IAuthenticator" /> implementations.
    /// </summary>
    public abstract class BaseAuthenticator : IAuthenticator
    {        
        private readonly CommandExecutor _commandExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAuthenticator" /> class.
        /// </summary>
        /// <param name="commandExecutor">An <see cref="CommandExecutor" /> instance used to execute commands against reddit.</param>
        protected BaseAuthenticator(CommandExecutor commandExecutor)
        {        
            _commandExecutor = commandExecutor;
        }

        /// <inheritdoc />
        public abstract Task<AuthenticationContext> GetAuthenticationContextAsync();

        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> via the authenticators <see cref="CommandExecutor" />.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the response of the command execution.</returns>
        internal async Task<HttpResponseMessage> ExecuteCommandAsync(ClientCommand command) =>
            await _commandExecutor.ExecuteCommandAsync(command).ConfigureAwait(false);

        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> via the authenticators <see cref="CommandExecutor" />, parsing the response to an instance of type <typeparamref name="TResponse" />.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation. The result contains the response of the command execution parsed as an instance of type <typeparamref name="TResponse" />.
        /// </returns>
        internal async Task<TResponse> ExecuteCommandAsync<TResponse>(ClientCommand command)
        {
            var response = await ExecuteCommandAsync(command).ConfigureAwait(false);

            return await response
                .Content
                .ReadFromJsonAsync<TResponse>()
                .ConfigureAwait(false);
        }
    }
}
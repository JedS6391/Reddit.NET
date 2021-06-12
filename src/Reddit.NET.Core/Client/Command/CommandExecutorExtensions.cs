using System.Net.Http.Json;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Authentication.Abstract;

namespace Reddit.NET.Core.Client.Command
{
    /// <summary>
    /// Defines extensions for <see cref="CommandExecutor" />.
    /// </summary>
    public static class CommandExecutorExtensions
    {
        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> instance, parsing the response to an instance of type <typeparamref name="TResponse" />.
        /// </summary>
        /// <param name="executor">A <see cref="CommandExecutor" /> instance.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result contains the response of the command execution parsed as an instance of type <typeparamref name="TResponse" />.
        /// </returns>
        public static async Task<TResponse> ExecuteCommandAsync<TResponse>(
            this CommandExecutor executor, 
            ClientCommand command)
        {
            var response = await executor.ExecuteCommandAsync(command).ConfigureAwait(false);

            return await response
                .Content
                .ReadFromJsonAsync<TResponse>()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> instance with authentication, parsing the response to an instance of type <typeparamref name="TResponse" />.
        /// </summary>
        /// <param name="executor">A <see cref="CommandExecutor" /> instance.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="authenticator">An <see cref="IAuthenticator" /> instance used to handle authentication for the command.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result contains the response of the command execution parsed as an instance of type <typeparamref name="TResponse" />.
        /// </returns>
        public static async Task<TResponse> ExecuteCommandAsync<TResponse>(
            this CommandExecutor executor, 
            ClientCommand command,
            IAuthenticator authenticator)
        {
            var response = await executor.ExecuteCommandAsync(command, authenticator).ConfigureAwait(false);

            return await response
                .Content
                .ReadFromJsonAsync<TResponse>()
                .ConfigureAwait(false);
        }
    }
}
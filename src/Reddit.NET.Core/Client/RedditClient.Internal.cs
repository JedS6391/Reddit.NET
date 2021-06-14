using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Interactions;

namespace Reddit.NET.Core.Client
{
    /// <summary>
    /// Provides mechanisms for interacting with <see href="https://www.reddit.com">reddit</see>. 
    /// </summary>
    /// <remarks>
    /// <see cref="RedditClient" /> cannot be directly instantiated and should instead be created via the <see cref="Builder.RedditClientBuilder" /> class.
    /// </remarks>
    public sealed partial class RedditClient
    {
        /// <summary>
        /// Gets an interactor for operations relating to a specific submission.
        /// </summary>
        /// <param name="submission">The submission to interact with.</param>            
        /// <returns>A <see cref="SubmissionInteractor" /> instance that provides mechanisms for interacting with the requested submission.</returns>
        internal SubmissionInteractor Submission(SubmissionDetails submission) => new SubmissionInteractor(this, submission);
    
        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> via the clients <see cref="CommandExecutor" />.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the response of the command execution.</returns>
        internal async Task<HttpResponseMessage> ExecuteCommandAsync(ClientCommand command) =>
            await _commandExecutor.ExecuteCommandAsync(command, _authenticator).ConfigureAwait(false);

        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> via the clients <see cref="CommandExecutor" />, parsing the response to an instance of type <typeparamref name="TResponse" />.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result contains the response of the command execution parsed as an instance of type <typeparamref name="TResponse" />.
        /// </returns>
        internal async Task<TResponse> ExecuteCommandAsync<TResponse>(ClientCommand command) =>
            await _commandExecutor.ExecuteCommandAsync<TResponse>(command, _authenticator).ConfigureAwait(false);
    }
}
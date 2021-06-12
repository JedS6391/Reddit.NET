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
        /// <param name="type">The type of the submission to interact with.</param>
        /// <param name="id">The ID of the submission to interact with.</param>
        /// <param name="subreddit">The name of the subreddit the submission is in.</param>
        /// <returns>A <see cref="SubmissionInteractor" /> instance that provides mechanisms for interacting with the requested submission.</returns>
        internal SubmissionInteractor Submission(SubmissionDetails submission) => new SubmissionInteractor(this, submission);
    
        internal async Task<HttpResponseMessage> ExecuteCommandAsync(ClientCommand command) =>
            await _commandExecutor.ExecuteCommandAsync(command, _authenticator).ConfigureAwait(false);

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
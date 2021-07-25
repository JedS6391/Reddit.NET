using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Command;
using Reddit.NET.Client.Interactions;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client
{
    /// <summary>
    /// Provides mechanisms for interacting with <see href="https://www.reddit.com">reddit</see>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="RedditClient" /> class is the main entry-point for accessing reddit's API.
    /// </para>
    /// <para>
    /// The client exposes its functionality through <i>interactors</i>, which are responsible for specific high-level concepts (e.g. subreddits, users, etc).
    /// </para>
    /// <para>
    /// Obtaining an interactor instance does not perform any network operations, as they simply provide access to a set of related functionality.
    /// </para>
    /// <para>
    /// <see cref="RedditClient" /> cannot be directly instantiated and should instead be created via the <see cref="Builder.RedditClientBuilder" /> class.
    /// </para>
    /// </remarks>
    /// <example>
    /// An example of interacting with a specific subreddit:
    /// <code language="cs">
    /// // Interact with a subreddit
    /// SubredditInteractor askReddit = client.Subreddit("askreddit");
    ///
    /// SubredditDetails askRedditDetails = await askReddit.GetDetailsAsync();
    /// </code>
    /// </example>
    public sealed class RedditClient
    {
        private readonly CommandExecutor _commandExecutor;
        private readonly IAuthenticator _authenticator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditClient" /> class.
        /// </summary>
        /// <param name="commandExecutor">An <see cref="CommandExecutor" /> instance used to execute commands against reddit.</param>
        /// <param name="authenticator">An <see cref="IAuthenticator" /> instance used to authenticate with reddit.</param>
        internal RedditClient(CommandExecutor commandExecutor, IAuthenticator authenticator)
        {
            _commandExecutor = Requires.NotNull(commandExecutor, nameof(commandExecutor));
            _authenticator = Requires.NotNull(authenticator, nameof(authenticator));
        }

        /// <summary>
        /// Gets an interactor for operations relating to the authenticated user.
        /// </summary>
        /// <returns>A <see cref="MeInteractor" /> instance that provides mechanisms for interacting with the authenticated user.</returns>
        public MeInteractor Me() => new MeInteractor(this);

        /// <summary>
        /// Gets an interactor for operations relating to a specific subreddit.
        /// </summary>
        /// <param name="name">The name of the subreddit to interact with.</param>
        /// <returns>A <see cref="SubredditInteractor" /> instance that provides mechanisms for interacting with the requested subreddit.</returns>
        public SubredditInteractor Subreddit(string name) => new SubredditInteractor(this, name);

        /// <summary>
        /// Gets an interactor for operations relating to a group of subreddits.
        /// </summary>
        /// <remarks>
        /// Note that some operations will not be possible when combining subreddits, e.g. you cannot create a new submission.
        /// </remarks>
        /// <param name="names">The names of the subreddits to interact with.</param>
        /// <returns>A <see cref="SubredditInteractor" /> instance that provides mechanisms for interacting with the requested subreddits.</returns>
        public SubredditInteractor Subreddits(params string[] names) => new SubredditInteractor(this, string.Join('+', names));

        /// <summary>
        /// Gets an interactor for operations relating to a specific user.
        /// </summary>
        /// <param name="name">The name of the user to interact with.</param>
        /// <returns>An <see cref="UserInteractor" /> instance that provides mechanisms for interacting with the requested user.</returns>
        public UserInteractor User(string name) => new UserInteractor(this, name);

        /// <summary>
        /// Gets the submissions on the front page.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the submissions on the front page.</returns>
        public IAsyncEnumerable<SubmissionDetails> GetFrontPageSubmissionsAsync(
            Action<FrontPageSubmissionsListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new FrontPageSubmissionsListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new FrontPageSubmissionsListingEnumerable(
                this,
                optionsBuilder.Options);
        }

        /// <summary>
        /// Gets an interactor for operations relating to a specific submission.
        /// </summary>
        /// <param name="submissionId">The base-36 ID of the submission to interact with.</param>
        /// <returns>A <see cref="SubmissionInteractor" /> instance that provides mechanisms for interacting with the provided submission.</returns>
        internal SubmissionInteractor Submission(string submissionId) => new SubmissionInteractor(this, submissionId);

        /// <summary>
        /// Gets an interactor for operations relating to a specific comment.
        /// </summary>
        /// <param name="submissionId">The base-36 ID of the submission the comment belongs to.</param>
        /// <param name="commentId">The base-36 ID of the comment to interact with.</param>
        /// <returns>A <see cref="CommentInteractor" /> instance that provides mechanisms for interacting with the provided comment.</returns>
        internal CommentInteractor Comment(string submissionId, string commentId) => new CommentInteractor(this, submissionId, commentId);

        /// <summary>
        /// Gets an interactor for operations relating to a specific multireddit.
        /// </summary>
        /// <param name="username">The name of the user the multireddit belongs to.</param>
        /// <param name="multiredditName">The name of the multireddit to interact with.</param>
        /// <returns>A <see cref="MultiredditInteractor" /> instance that provides mechanisms for interacting with the provided multireddit.</returns>
        internal MultiredditInteractor Multireddit(string username, string multiredditName) =>
            new MultiredditInteractor(this, username, multiredditName);

        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> via the clients <see cref="CommandExecutor" />.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the response of the command execution.</returns>
        internal async Task<HttpResponseMessage> ExecuteCommandAsync(ClientCommand command, CancellationToken cancellationToken) =>
            await _commandExecutor.ExecuteCommandAsync(command, _authenticator, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> via the clients <see cref="CommandExecutor" />, parsing the response to an instance of type <typeparamref name="TResponse" />.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result contains the response of the command execution parsed as an instance of type <typeparamref name="TResponse" />.
        /// </returns>
        internal async Task<TResponse> ExecuteCommandAsync<TResponse>(ClientCommand command, CancellationToken cancellationToken) =>
            await _commandExecutor.ExecuteCommandAsync<TResponse>(command, _authenticator, cancellationToken).ConfigureAwait(false);
    }
}

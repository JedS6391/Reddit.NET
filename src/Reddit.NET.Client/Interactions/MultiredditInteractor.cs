using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Reddit.NET.Client.Command.Multireddits;
using Reddit.NET.Client.Interactions.Abstract;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with an authenticated user's multireddit.
    /// </summary>
    public sealed class MultiredditInteractor : IInteractor
    {
        private readonly RedditClient _client;
        private readonly string _username;
        private readonly string _multiredditName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiredditInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="username">The name of the user the multireddit belongs to.</param>
        /// <param name="multiredditName">The name of the multireddit to interact with.</param>
        internal MultiredditInteractor(RedditClient client, string username, string multiredditName)
        {
            _client = client;
            _username = username;
            _multiredditName = multiredditName;
        }

        /// <summary>
        /// Gets the details of the multireddit.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the multireddit.</returns>
        public async Task<MultiredditDetails> GetDetailsAsync(CancellationToken cancellationToken = default)
        {
            var commandParameters = new GetMultiredditDetailsCommand.Parameters()
            {
                Username = _username,
                MultiredditName = _multiredditName
            };

            var getMultiredditDetailsCommand = new GetMultiredditDetailsCommand(commandParameters);

            var multireddit = await _client
                .ExecuteCommandAsync<Multireddit>(getMultiredditDetailsCommand, cancellationToken)
                .ConfigureAwait(false);

            return new MultiredditDetails(multireddit);
        }

        /// <summary>
        /// Gets the submissions of the multireddit.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the submissions of the multireddit.</returns>
        public IAsyncEnumerable<SubmissionDetails> GetSubmissionsAsync(
            Action<MultiredditSubmissionsListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new MultiredditSubmissionsListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new MultiredditSubmissionsListingEnumerable(
                _client,
                optionsBuilder.Options,
                new MultiredditSubmissionsListingEnumerable.ListingParameters()
                {
                    Username = _username,
                    MultiredditName = _multiredditName
                });
        }

        /// <summary>
        /// Deletes the multireddit.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            var commandParameters = new DeleteMultiredditCommand.Parameters()
            {
                Username = _username,
                MultiredditName = _multiredditName
            };

            var deleteMultiredditCommand = new DeleteMultiredditCommand(commandParameters);

            await _client.ExecuteCommandAsync(deleteMultiredditCommand, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds a subreddit to the multireddit.
        /// </summary>
        /// <param name="subredditName">The name of the subreddit to add.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddSubredditAsync(string subredditName, CancellationToken cancellationToken = default)
        {
            var commandParameters = new AddSubredditToMultiredditCommand.Parameters()
            {
                Username = _username,
                MultiredditName = _multiredditName,
                SubredditName = subredditName
            };

            var addSubredditToMultiredditCommand = new AddSubredditToMultiredditCommand(commandParameters);

            await _client.ExecuteCommandAsync(addSubredditToMultiredditCommand, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes a subreddit from the multireddit.
        /// </summary>
        /// <param name="subredditName">The name of the subreddit to remove.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveSubredditAsync(string subredditName, CancellationToken cancellationToken = default)
        {
            var commandParameters = new RemoveSubredditFromMultiredditCommand.Parameters()
            {
                Username = _username,
                MultiredditName = _multiredditName,
                SubredditName = subredditName
            };

            var removeSubredditFromMultiredditCommand = new RemoveSubredditFromMultiredditCommand(commandParameters);

            await _client.ExecuteCommandAsync(removeSubredditFromMultiredditCommand, cancellationToken).ConfigureAwait(false);
        }
    }
}

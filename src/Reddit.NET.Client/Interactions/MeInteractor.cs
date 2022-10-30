using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Command.Users;
using Reddit.NET.Client.Interactions.Abstract;
using System.Linq;
using Reddit.NET.Client.Models.Public.Write;
using Reddit.NET.Client.Models.Public.Streams;
using System.Threading;
using Microsoft;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with the authenticated user.
    /// </summary>
    public sealed class MeInteractor : IInteractor
    {
        private readonly RedditClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        internal MeInteractor(RedditClient client)
        {
            _client = Requires.NotNull(client, nameof(client));
        }

        /// <summary>
        /// Gets an interactor for operations relating to the authenticated user's inbox.
        /// </summary>
        /// <returns>A <see cref="InboxInteractor" /> instance that provides mechanisms for interacting with the authenticated user's inbox.</returns>
        public InboxInteractor Inbox() => new InboxInteractor(_client);

        /// <summary>
        /// Gets a <see cref="UserStreamProvider" /> that can be used to access streams of submissions or comments.
        /// </summary>
        public UserStreamProvider Stream => new UserStreamProvider(_client);

        /// <summary>
        /// Gets the details of the authenticated user.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the authenticated user.</returns>
        public async Task<UserDetails> GetDetailsAsync(CancellationToken cancellationToken = default)
        {
            var getMyDetailsCommand = new GetMyDetailsCommand();

            // We use the data model type to deserialize the response as the user details
            // API returns a plain data object, rather than wrapping the data within a thing.
            var user = await _client
                .ExecuteCommandAsync<User.Details>(getMyDetailsCommand, cancellationToken)
                .ConfigureAwait(false);

            return new UserDetails(user);
        }

        /// <summary>
        /// Gets the subreddits the authenticated user is subscribed to.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the authenticated user's subreddits.</returns>
        public IAsyncEnumerable<SubredditDetails> GetSubredditsAsync(
            Action<MySubredditsListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new MySubredditsListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new MySubredditsListingEnumerable(_client, optionsBuilder.Options);
        }

        /// <summary>
        /// Gets the multireddits that belong to the authenticated user.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the multireddits of the authenticated user.</returns>
        public async Task<IReadOnlyList<MultiredditDetails>> GetMultiredditsAsync(CancellationToken cancellationToken = default)
        {
            var getMyMultiredditsCommand = new GetMyMultiredditsCommand();

            var multireddits = await _client
                .ExecuteCommandAsync<IReadOnlyList<Multireddit>>(getMyMultiredditsCommand, cancellationToken)
                .ConfigureAwait(false);

            return multireddits.Select(mr => new MultiredditDetails(mr)).ToList();
        }

        /// <summary>
        /// Gets the history of the authenticated user.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the authenticated user's history.</returns>
        public IAsyncEnumerable<UserContentDetails> GetHistoryAsync(
            Action<UserHistoryListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new UserHistoryListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new UserHistoryListingEnumerable(
                _client,
                optionsBuilder.Options,
                new UserHistoryListingEnumerable.ListingParameters()
                {
                    UseAuthenticatedUser = true
                });
        }

        /// <summary>
        /// Gets the friends of the authenticated user.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the authenticated user's friends.</returns>
        public IAsyncEnumerable<FriendDetails> GetFriendsAsync(
            Action<MyFriendsListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new MyFriendsListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new MyFriendsListingEnumerable(
                _client,
                optionsBuilder.Options);
        }

        /// <summary>
        /// Gets the karma breakdown of the authenticated user.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the karma breakdown of the authenticated user.</returns>
        public async Task<IReadOnlyList<KarmaBreakdownDetails>> GetKarmaBreakdownAsync(CancellationToken cancellationToken = default)
        {
            var getMyKarmaBreakdownCommand = new GetMyKarmaBreakdownCommand();

            var karmaBreakdown = await _client
                .ExecuteCommandAsync<KarmaList>(getMyKarmaBreakdownCommand, cancellationToken)
                .ConfigureAwait(false);

            return karmaBreakdown
                .Data
                .Select(kb => new KarmaBreakdownDetails(kb))
                .ToList();
        }

        /// <summary>
        /// Gets the trophies of the authenticated user.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the trophies of the authenticated user.</returns>
        public async Task<IReadOnlyList<TrophyDetails>> GetTrophiesAsync(CancellationToken cancellationToken = default)
        {
            var getMyTrophiesCommand = new GetMyTrophiesCommand();

            var trophyList = await _client
                .ExecuteCommandAsync<TrophyList>(getMyTrophiesCommand, cancellationToken)
                .ConfigureAwait(false);

            return trophyList
                .Data
                .Trophies
                .Select(t => new TrophyDetails(t))
                .ToList();
        }

        /// <summary>
        /// Creates a new multireddit belonging to the authenticated user.
        /// </summary>
        /// <param name="details">The details of a multireddit to create.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the created multireddit details.
        /// </returns>
        public async Task<MultiredditDetails> CreateMultiredditAsync(MultiredditCreationDetails details, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(details, nameof(details));

            var commandParameters = new CreateMultiredditCommand.Parameters()
            {
                Name = details.Name,
                Subreddits = details.Subreddits
            };

            var createMultiredditCommand = new CreateMultiredditCommand(commandParameters);

            var multireddit = await _client
                .ExecuteCommandAsync<Multireddit>(createMultiredditCommand, cancellationToken)
                .ConfigureAwait(false);

            return new MultiredditDetails(multireddit);
        }
    }
}

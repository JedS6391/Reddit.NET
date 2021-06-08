using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Models.Public.Listings;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Subreddits;
using Reddit.NET.Core.Client.Interactions.Abstract;

namespace Reddit.NET.Core.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a subreddit.
    /// </summary>
    public class SubredditInteractor : IInteractor
    {                 
        private readonly CommandFactory _commandFactory; 
        private readonly IAuthenticator _authenticator;
        private readonly string _subredditName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditInteractor" /> class.
        /// </summary>
        /// <param name="commandFactory">A <see cref="CommandFactory" /> instance used for creating commands for interactions with reddit.</param>
        /// <param name="authenticator">An <see cref="IAuthenticator" /> instance used to authenticate with reddit.</param>
        /// <param name="subredditName">The name of the subreddit to interact with.</param>
        public SubredditInteractor(
            CommandFactory commandFactory,
            IAuthenticator authenticator,
            string subredditName)
        {
            _commandFactory = commandFactory;
            _authenticator = authenticator;
            _subredditName = subredditName;
        }

        /// <summary>
        /// Gets the details of the subreddit.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the subreddit.</returns>
        public async Task<SubredditDetails> GetDetailsAsync()
        {
            var authenticationContext = await _authenticator.GetAuthenticationContextAsync().ConfigureAwait(false);

            var getSubredditCommand = _commandFactory.Create<GetSubredditDetailsCommand>();

            var result = await getSubredditCommand.ExecuteAsync(
                authenticationContext, 
                new GetSubredditDetailsCommand.Parameters()
                {
                    SubredditName = _subredditName
                })
                .ConfigureAwait(false);

            return result.Details;
        }

        /// <summary>
        /// Gets the 'hot' submissions of the subreddit.
        /// </summary>
        /// <returns>An asynchronous enumerator over the 'hot' submissions of the subreddit.</returns>
        public IAsyncEnumerable<SubmissionDetails> GetHotSubmissionsAsync() => 
            new HotSubredditSubmissionsListingGenerator(
                _commandFactory, 
                _authenticator,
                new SubredditSubmissionsListingGenerator.ListingParameters()
                {
                    SubredditName = _subredditName
                });
    }
}
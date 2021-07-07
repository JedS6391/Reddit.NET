using System.Threading.Tasks;
using Reddit.NET.Client.Command.Multireddits;
using Reddit.NET.Client.Interactions.Abstract;
using Reddit.NET.Client.Models.Internal;
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
        public MultiredditInteractor(RedditClient client, string username, string multiredditName)
        {
            _client = client;
            _username = username;
            _multiredditName = multiredditName;
        }

        /// <summary>
        /// Gets the details of the multireddit.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the multireddit.</returns>
        public async Task<MultiredditDetails> GetDetailsAsync()
        {
            var commandParameters = new GetMultiredditDetailsCommand.Parameters()
            {
                Username = _username,
                MultiredditName = _multiredditName
            };

            var getMultiredditDetailsCommand = new GetMultiredditDetailsCommand(commandParameters);

            var multireddit = await _client.ExecuteCommandAsync<Multireddit>(getMultiredditDetailsCommand).ConfigureAwait(false);

            return new MultiredditDetails(multireddit);
        }

        /// <summary>
        /// Adds a subreddit to the multireddit.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddSubredditAsync(string subredditName)
        {
            var commandParameters = new AddSubredditToMultiredditCommand.Parameters()
            {
                Username = _username,
                MultiredditName = _multiredditName,
                SubredditName = subredditName
            };

            var addSubredditToMultiredditCommand = new AddSubredditToMultiredditCommand(commandParameters);

            await _client.ExecuteCommandAsync(addSubredditToMultiredditCommand).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes a subreddit from the multireddit.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveSubredditAsync(string subredditName)
        {
            var commandParameters = new RemoveSubredditFromMultiredditCommand.Parameters()
            {
                Username = _username,
                MultiredditName = _multiredditName,
                SubredditName = subredditName
            };

            var removeSubredditFromMultiredditCommand = new RemoveSubredditFromMultiredditCommand(commandParameters);

            await _client.ExecuteCommandAsync(removeSubredditFromMultiredditCommand).ConfigureAwait(false);
        }
    }
}
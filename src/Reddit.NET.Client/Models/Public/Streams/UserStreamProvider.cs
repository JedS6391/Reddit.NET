using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reddit.NET.Client.Command.Users;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Models.Public.Streams
{
    /// <summary>
    /// Provides access to streams of submissions or comments from a user.
    /// </summary>
    public sealed class UserStreamProvider
    {
        private readonly RedditClient _client;
        private readonly string _username;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditStreamProvider" /> class.
        /// </summary>
        /// <remarks>
        /// This constructor is intended to be used in a context when the streams provided should be for the current user.
        /// </remarks>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        public UserStreamProvider(RedditClient client)
        {
            _client = client;
            _username = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditStreamProvider" /> class.
        /// </summary>
        /// <remarks>
        /// This constructor is intended to be used in a context when the streams provided should be for a specific user.
        /// </remarks>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="username">The name of the user to provide streams for.</param>
        public UserStreamProvider(RedditClient client, string username)
        {
            _client = client;
            _username = username;
        }

        /// <summary>
        /// Gets new submissions made by the user.
        /// </summary>
        /// <remarks>
        /// The enumerator will return the oldest submissions first, retrieving 100 historical submissions in the initial query.
        /// </remarks>
        /// <returns>An asynchronous enumerator over new submissions made by the user.</returns>
        public IAsyncEnumerable<SubmissionDetails> SubmissionsAsync() =>
            PollingStream.Create(new PollingStreamOptions<Submission, SubmissionDetails, string>(
                () => GetNewHistoryAsync<Submission>(UserHistoryType.Submitted),
                mapper: s => new SubmissionDetails(s),
                idSelector: s => s.Data.Id));

        /// <summary>
        /// Gets new comments made by the user.
        /// </summary>
        /// <remarks>
        /// The enumerator will return the oldest comments first, retrieving 100 historical comments in the initial query.
        /// </remarks>
        /// <returns>An asynchronous enumerator over new comments made by the user.</returns>
        public IAsyncEnumerable<CommentDetails> CommentsAsync() =>
            PollingStream.Create(new PollingStreamOptions<Comment, CommentDetails, string>(
                () => GetNewHistoryAsync<Comment>(UserHistoryType.Comments),
                mapper: c => new CommentDetails(c),
                idSelector: c => c.Data.Id));

        private async Task<IEnumerable<TThing>> GetNewHistoryAsync<TThing>(UserHistoryType historyType)
        {
            var commandParameters = new GetUserHistoryCommand.Parameters()
            {
                HistoryType = historyType.Name,
                Sort = UserHistorySort.New.Name,
                Limit = 100
            };

            if (string.IsNullOrEmpty(_username))
            {
                // Resolve the username for the currently authenticated user.
                var getMyDetailsCommand = new GetMyDetailsCommand();

                var user = await _client
                    .ExecuteCommandAsync<User.Details>(getMyDetailsCommand)
                    .ConfigureAwait(false);

                commandParameters.Username = user.Name;
            }
            else
            {
                commandParameters.Username = _username;
            }

            var getUserHistoryCommand = new GetUserHistoryCommand(commandParameters);

            var history = await _client
                .ExecuteCommandAsync<Listing<IUserContent>>(getUserHistoryCommand)
                .ConfigureAwait(false);

            return history.Children.OfType<TThing>();
        }
    }
}

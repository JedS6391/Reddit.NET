using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
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
            : this(client, username: null)
        {
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
            _client = Requires.NotNull(client, nameof(client));
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
                (ct) => GetNewHistoryAsync<Submission>(UserHistoryType.Submitted, ct),
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
                (ct) => GetNewHistoryAsync<Comment>(UserHistoryType.Comments, ct),
                mapper: c => new CommentDetails(c),
                idSelector: c => c.Data.Id));

        private async Task<IEnumerable<TThing>> GetNewHistoryAsync<TThing>(UserHistoryType historyType, CancellationToken cancellationToken)
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
                    .ExecuteCommandAsync<User.Details>(getMyDetailsCommand, cancellationToken)
                    .ConfigureAwait(false);

                commandParameters.Username = user.Name;
            }
            else
            {
                commandParameters.Username = _username;
            }

            var getUserHistoryCommand = new GetUserHistoryCommand(commandParameters);

            var history = await _client
                .ExecuteCommandAsync<Listing<IUserContent>>(getUserHistoryCommand, cancellationToken)
                .ConfigureAwait(false);

            return history.Children.OfType<TThing>();
        }
    }
}

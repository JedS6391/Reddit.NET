using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Client.Command.Subreddits;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Models.Public.Streams
{
    /// <summary>
    /// Provides access to streams of submissions or comments from a subreddit.
    /// </summary>
    public sealed class SubredditStreamProvider
    {
        private readonly RedditClient _client;
        private readonly string _subredditName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditStreamProvider" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="subredditName">The name of the subreddit to provide streams for.</param>
        public SubredditStreamProvider(RedditClient client, string subredditName)
        {
            _client = client;
            _subredditName = subredditName;
        }

        /// <summary>
        /// Gets new submissions made in the subreddit.
        /// </summary>
        /// <remarks>
        /// The enumerator will return the oldest submissions first.
        /// </remarks>
        /// <returns>An asynchronous enumerator over new submissions in the subreddit.</returns>
        public IAsyncEnumerable<SubmissionDetails> SubmissionsAsync() =>
            PollingStream.Create(new PollingStreamOptions<IThing<Submission.Details>, SubmissionDetails, string>(
                GetNewSubmissionsAsync,
                mapper: s => new SubmissionDetails(s),
                idSelector: s => s.Data.Id));

        /// <summary>
        /// Gets new comments made in the subreddit.
        /// </summary>
        /// <remarks>
        /// The enumerator will return the oldest comments first.
        /// </remarks>
        /// <returns>An asynchronous enumerator over new comments in the subreddit.</returns>
        public IAsyncEnumerable<CommentDetails> CommentsAsync() =>
            PollingStream.Create(new PollingStreamOptions<IThing<Comment.Details>, CommentDetails, string>(
                GetNewCommentsAsync,
                mapper: c => new CommentDetails(c),
                idSelector: c => c.Data.Id));

        private async Task<IReadOnlyList<IThing<Submission.Details>>> GetNewSubmissionsAsync()
        {
            var commandParameters = new GetSubredditSubmissionsCommand.Parameters()
            {
                SubredditName = _subredditName,
                Sort = SubredditSubmissionSort.New.Name,
                Limit = 100
            };

            var getSubredditSubmissionsCommand = new GetSubredditSubmissionsCommand(commandParameters);

            var submissions = await _client
                .ExecuteCommandAsync<Submission.Listing>(getSubredditSubmissionsCommand)
                .ConfigureAwait(false);

            return submissions.Children;
        }

        private async Task<IReadOnlyList<IThing<Comment.Details>>> GetNewCommentsAsync()
        {
            var commandParameters = new GetSubredditCommentsCommand.Parameters()
            {
                SubredditName = _subredditName,
                Limit = 100
            };

            var getSubredditCommentsCommand = new GetSubredditCommentsCommand(commandParameters);

            var comments = await _client
                .ExecuteCommandAsync<Comment.Listing>(getSubredditCommentsCommand)
                .ConfigureAwait(false);

            return comments.Children;
        }
    }
}
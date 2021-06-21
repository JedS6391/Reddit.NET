using System.Threading.Tasks;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Command.Vote;
using Reddit.NET.Client.Interactions.Abstract;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a comment.
    /// </summary>
    public sealed class CommentInteractor : IInteractor
    {
        private readonly RedditClient _client;
        private readonly string _commentId;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentInteractor" /> class.
        /// </summary>        
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="commentId">The base-36 ID of the comment to interact with.</param>
        public CommentInteractor(RedditClient client, string commentId)
        {
            _client = client;
            _commentId = commentId;
        }

        /// <summary>
        /// Upvotes the comment.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpvoteAsync() => await ApplyVote(ApplyVoteCommand.VoteDirection.Upvote).ConfigureAwait(false);

        /// <summary>
        /// Downvotes the comment.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DownvoteAsync() => await ApplyVote(ApplyVoteCommand.VoteDirection.Downvote).ConfigureAwait(false); 

        private async Task ApplyVote(ApplyVoteCommand.VoteDirection direction) 
        {
            var applyVoteToSubmissionCommand = new ApplyVoteCommand(new ApplyVoteCommand.Parameters()
            {
                // We need to provide the full name of the comment to vote on.
                Id = $"{Constants.Kind.Comment}_{_commentId}",
                Direction = direction
            });

            await _client.ExecuteCommandAsync(applyVoteToSubmissionCommand).ConfigureAwait(false);           
        }        
    }
}
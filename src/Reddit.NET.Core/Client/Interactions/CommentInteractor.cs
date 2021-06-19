using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Vote;
using Reddit.NET.Core.Client.Interactions.Abstract;

namespace Reddit.NET.Core.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a comment.
    /// </summary>
    public sealed class CommentInteractor : IInteractor
    {
        private readonly RedditClient _client;
        private readonly CommentDetails _comment;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentInteractor" /> class.
        /// </summary>        
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="comment">The details of the comment to interact with.</param>
        public CommentInteractor(RedditClient client, CommentDetails comment)
        {
            _client = client;
            _comment = comment;
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
                Id = _comment.FullName,
                Direction = direction
            });

            await _client.ExecuteCommandAsync(applyVoteToSubmissionCommand);           
        }        
    }
}
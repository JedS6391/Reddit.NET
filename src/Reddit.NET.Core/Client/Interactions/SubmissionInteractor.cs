using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Public.Listings;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Submissions;
using Reddit.NET.Core.Client.Interactions.Abstract;

namespace Reddit.NET.Core.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a submission.
    /// </summary>
    public class SubmissionInteractor : IInteractor
    {
        private readonly RedditClient _client;
        private readonly SubmissionDetails _submission;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionInteractor" /> class.
        /// </summary>        
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="submission">The details of the submission to interact with.</param>
        public SubmissionInteractor(RedditClient client, SubmissionDetails submission)
        {
            _client = client;            
            _submission = submission;
        }

        /// <summary>
        /// Upvotes the submission.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpvoteAsync() => await ApplyVote(ApplyVoteToSubmissionCommand.VoteDirection.Upvote).ConfigureAwait(false);

        /// <summary>
        /// Downvotes the submission.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DownvoteAsync() => await ApplyVote(ApplyVoteToSubmissionCommand.VoteDirection.Downvote).ConfigureAwait(false);

        /// <summary>
        /// Gets the comments on the submission.
        /// </summary>
        /// <returns>An asynchronous enumerator over the comments on the submission.</returns>
        public IAsyncEnumerable<CommentDetails> GetCommentsAsync() =>
            new SubmissionCommentsListingGenerator(
                _client,
                new SubmissionCommentsListingGenerator.ListingParameters()
                {
                    SubredditName = _submission.Subreddit,
                    SubmissionId = _submission.Id
                });

        private async Task ApplyVote(ApplyVoteToSubmissionCommand.VoteDirection direction) 
        {
            var applyVoteToSubmissionCommand = new ApplyVoteToSubmissionCommand(new ApplyVoteToSubmissionCommand.Parameters()
            {
                Id = $"{_submission.Kind}_{_submission.Id}",
                Direction = direction
            });

            await _client.ExecuteCommandAsync(applyVoteToSubmissionCommand);           
        }   
    }
}
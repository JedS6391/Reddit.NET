using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Public.Listings;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Vote;
using Reddit.NET.Core.Client.Interactions.Abstract;

namespace Reddit.NET.Core.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a submission.
    /// </summary>
    public sealed class SubmissionInteractor : IInteractor
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
        public async Task UpvoteAsync() => await ApplyVote(ApplyVoteCommand.VoteDirection.Upvote).ConfigureAwait(false);

        /// <summary>
        /// Downvotes the submission.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DownvoteAsync() => await ApplyVote(ApplyVoteCommand.VoteDirection.Downvote).ConfigureAwait(false);

        /// <summary>
        /// Gets the comments on the submission.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the comments on the submission.</returns>
        public IAsyncEnumerable<CommentDetails> GetCommentsAsync(
            Action<SubmissionCommentsListingEnumerable.Options.Builder> configurationAction = null) 
        {
            var optionsBuilder = new SubmissionCommentsListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new SubmissionCommentsListingEnumerable(
                _client,
                optionsBuilder.Options,
                new SubmissionCommentsListingEnumerable.ListingParameters()
                {
                    SubredditName = _submission.Subreddit,
                    SubmissionId = _submission.Id
                });
        }

        private async Task ApplyVote(ApplyVoteCommand.VoteDirection direction) 
        {
            var applyVoteToSubmissionCommand = new ApplyVoteCommand(new ApplyVoteCommand.Parameters()
            {
                Id = _submission.FullName,
                Direction = direction
            });

            await _client.ExecuteCommandAsync(applyVoteToSubmissionCommand);           
        }   
    }
}
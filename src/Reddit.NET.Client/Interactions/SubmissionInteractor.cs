using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Command.Vote;
using Reddit.NET.Client.Interactions.Abstract;
using Reddit.NET.Client.Command.Submissions;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a submission.
    /// </summary>
    public sealed class SubmissionInteractor : IInteractor
    {
        private readonly RedditClient _client;
        private readonly string _submissionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionInteractor" /> class.
        /// </summary>        
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="submissionId">The base-36 ID of the submission to interact with.</param>
        public SubmissionInteractor(RedditClient client, string submissionId)
        {
            _client = client;            
            _submissionId = submissionId;
        }

        /// <summary>
        /// Gets the details of the submission.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the submission.</returns>
        public async Task<SubmissionDetails> GetDetailsAsync()
        {
            var commandParameters = new GetSubmissionDetailsWithCommentsCommand.Parameters()
            {
                SubmissionId = _submissionId,
                // Don't fetch any comments since we're just interested in the submission details
                Limit = 0
            };

            var getSubmissionDetailsWithCommentsCommand = new GetSubmissionDetailsWithCommentsCommand(commandParameters);

            var submissionWithComments = await _client
                .ExecuteCommandAsync<Submission.SubmissionWithComments>(getSubmissionDetailsWithCommentsCommand)
                .ConfigureAwait(false);

            return new SubmissionDetails(submissionWithComments.Submission);
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
                    SubmissionId = _submissionId
                });
        }

        private async Task ApplyVote(ApplyVoteCommand.VoteDirection direction) 
        {
            var applyVoteToSubmissionCommand = new ApplyVoteCommand(new ApplyVoteCommand.Parameters()
            {
                Id = $"{Constants.Kind.Submission}_{_submissionId}",
                Direction = direction
            });

            await _client.ExecuteCommandAsync(applyVoteToSubmissionCommand).ConfigureAwait(false);           
        }   
    }
}
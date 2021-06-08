using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
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
        private readonly CommandFactory _commandFactory; 
        private readonly IAuthenticator _authenticator;
        private readonly SubmissionDetails _submission;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionInteractor" /> class.
        /// </summary>
        /// <param name="commandFactory">A <see cref="CommandFactory" /> instance used for creating commands for interactions with reddit.</param>
        /// <param name="authenticator">An <see cref="IAuthenticator" /> instance used to authenticate with reddit.</param>
        /// <param name="submission">The details of the submission to interact with.</param>
        public SubmissionInteractor(
            CommandFactory commandFactory,
            IAuthenticator authenticator,
            SubmissionDetails submission)
        {
            _commandFactory = commandFactory;
            _authenticator = authenticator;
            _submission = submission;
        }

        /// <summary>
        /// Upvotes the submission.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpvoteAsync() => await ApplyVote(ApplyVoteToSubmissionCommand.VoteDirection.Upvote);

        /// <summary>
        /// Downvotes the submission.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DownvoteAsync() => await ApplyVote(ApplyVoteToSubmissionCommand.VoteDirection.Downvote);

        /// <summary>
        /// Gets the comments on the submission.
        /// </summary>
        /// <returns>An asynchronous enumerator over the comments on the submission.</returns>
        public IAsyncEnumerable<CommentDetails> GetCommentsAsync() =>
            new SubmissionCommentsListingGenerator(
                _commandFactory,
                _authenticator,
                new SubmissionCommentsListingGenerator.ListingParameters()
                {
                    SubredditName = _submission.Subreddit,
                    SubmissionId = _submission.Id
                });

        private async Task ApplyVote(ApplyVoteToSubmissionCommand.VoteDirection direction) 
        {
            var authenticationContext = await _authenticator.GetAuthenticationContextAsync().ConfigureAwait(false);

            var getSubredditCommand = _commandFactory.Create<ApplyVoteToSubmissionCommand>();

            await getSubredditCommand.ExecuteAsync(
                authenticationContext, 
                new ApplyVoteToSubmissionCommand.Parameters()
                {
                    Id = $"{_submission.Kind}_{_submission.Id}",
                    Direction = direction                   
                })
                .ConfigureAwait(false);            
        }   
    }
}
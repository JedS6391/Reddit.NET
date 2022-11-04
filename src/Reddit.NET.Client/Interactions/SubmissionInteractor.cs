using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Reddit.NET.Client.Command.Submissions;
using Reddit.NET.Client.Interactions.Abstract;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a submission.
    /// </summary>
    public sealed class SubmissionInteractor : UserContentInteractor, IInteractor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="submissionId">The base-36 ID of the submission to interact with.</param>
        internal SubmissionInteractor(RedditClient client, string submissionId)
            : base(client, kind: Constants.Kind.Submission, id: submissionId)
        {
        }

        /// <summary>
        /// Gets the details of the submission.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the submission.</returns>
        public async Task<SubmissionDetails> GetDetailsAsync(CancellationToken cancellationToken = default)
        {
            var commandParameters = new GetSubmissionDetailsWithCommentsCommand.Parameters()
            {
                SubmissionId = Id,
                // Don't fetch any comments since we're just interested in the submission details
                Limit = 0
            };

            var getSubmissionDetailsWithCommentsCommand = new GetSubmissionDetailsWithCommentsCommand(commandParameters);

            var submissionWithComments = await Client
                .ExecuteCommandAsync<Submission.SubmissionWithComments>(getSubmissionDetailsWithCommentsCommand, cancellationToken)
                .ConfigureAwait(false);

            return new SubmissionDetails(submissionWithComments.Submission);
        }

        /// <summary>
        /// Gets the duplicates of the submission.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the duplicates of the submission.</returns>
        public IAsyncEnumerable<SubmissionDetails> GetDuplicatesAsync(
            Action<DuplicateSubmissionsListingEnumerable.Options.Builder> configurationAction = null
        )
        {
            var optionsBuilder = new DuplicateSubmissionsListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new DuplicateSubmissionsListingEnumerable(
                Client,
                optionsBuilder.Options,
                new DuplicateSubmissionsListingEnumerable.ListingParameters()
                {
                    SubmissionId = Id
                });
        }

        /// <summary>
        /// Gets a <see cref="CommentThreadNavigator" /> over the comments on the submission.
        /// </summary>
        /// <param name="limit">The number of comments to retrieve.</param>
        /// <param name="sort">The sort order to retrieve comments with.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result contains a navigator over the comments.
        /// </returns>
        public async Task<CommentThreadNavigator> GetCommentsAsync(
            int? limit = null,
            SubmissionsCommentSort sort = null,
            CancellationToken cancellationToken = default)
        {
            if (sort == null)
            {
                sort = SubmissionsCommentSort.Confidence;
            }

            var commandParameters = new GetSubmissionDetailsWithCommentsCommand.Parameters()
            {
                SubmissionId = Id,
                Limit = limit,
                Sort = sort.Name
            };

            var getSubmissionDetailsWithCommentsCommand = new GetSubmissionDetailsWithCommentsCommand(commandParameters);

            var submissionWithComments = await Client
                .ExecuteCommandAsync<Submission.SubmissionWithComments>(getSubmissionDetailsWithCommentsCommand, cancellationToken)
                .ConfigureAwait(false);

            return new CommentThreadNavigator(
                submission: submissionWithComments.Submission,
                replies: submissionWithComments.Comments.Children,
                sort: sort);
        }
    }
}

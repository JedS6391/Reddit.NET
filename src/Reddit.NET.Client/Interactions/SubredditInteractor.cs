using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Command.Subreddits;
using Reddit.NET.Client.Interactions.Abstract;
using Reddit.NET.Client.Models.Public.Write;
using Reddit.NET.Client.Command.Submissions;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Listings.Options;
using System.Linq;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.Models.Public.Streams;
using System.Threading;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a subreddit.
    /// </summary>
    public sealed class SubredditInteractor : IInteractor
    {
        private readonly RedditClient _client;
        private readonly string _subredditName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="subredditName">The name of the subreddit to interact with.</param>
        internal SubredditInteractor(RedditClient client, string subredditName)
        {
            _client = client;
            _subredditName = subredditName;
        }

        /// <summary>
        /// Gets a <see cref="SubredditStreamProvider" /> that can be used to access streams of submissions or comments.
        /// </summary>
        public SubredditStreamProvider Stream => new SubredditStreamProvider(_client, _subredditName);

        /// <summary>
        /// Gets the details of the subreddit.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the subreddit.</returns>
        public async Task<SubredditDetails> GetDetailsAsync(CancellationToken cancellationToken = default)
        {
            var getSubredditDetailsCommand = new GetSubredditDetailsCommand(new GetSubredditDetailsCommand.Parameters()
            {
                SubredditName = _subredditName
            });

            var subreddit = await _client
                .ExecuteCommandAsync<Subreddit>(getSubredditDetailsCommand, cancellationToken)
                .ConfigureAwait(false);

            return new SubredditDetails(subreddit);
        }

        /// <summary>
        /// Gets the submissions of the subreddit.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the submissions of the subreddit.</returns>
        public IAsyncEnumerable<SubmissionDetails> GetSubmissionsAsync(
            Action<SubredditSubmissionsListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new SubredditSubmissionsListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new SubredditSubmissionsListingEnumerable(
                _client,
                optionsBuilder.Options,
                new SubredditSubmissionsListingEnumerable.ListingParameters()
                {
                    SubredditName = _subredditName
                });
        }

        /// <summary>
        /// Searches the submissions of the subreddit.
        /// </summary>
        /// <remarks>
        /// Note that the query is provided as a raw string, but can use the syntax of a particular <see cref="SearchQuerySyntax" /> option.
        /// </remarks>
        /// <param name="query">The search query.</param>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the submissions of the subreddit.</returns>
        public IAsyncEnumerable<SubmissionDetails> SearchSubmissionsAsync(
            string query,
            Action<SubredditSearchListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new SubredditSearchListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new SubredditSearchListingEnumerable(
                _client,
                optionsBuilder.Options,
                new SubredditSearchListingEnumerable.ListingParameters()
                {
                    SubredditName = _subredditName,
                    Query = query
                });
        }

        /// <summary>
        /// Subscribes to the subreddit.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SubscribeAsync(CancellationToken cancellationToken = default) =>
            await UpdateSubredditSubscriptionAsync(
                UpdateSubredditSubscriptionCommand.SubscriptionAction.Subscribe,
                cancellationToken);

        /// <summary>
        /// Unsubscribes from the subreddit.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UnsubscribeAsync(CancellationToken cancellationToken = default) =>
            await UpdateSubredditSubscriptionAsync(
                UpdateSubredditSubscriptionCommand.SubscriptionAction.Unubscribe,
                cancellationToken);

        /// <summary>
        /// Creates a link submission in the subreddit.
        /// </summary>
        /// <param name="details">The details of a link submission to create.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the created submission details.
        /// </returns>
        public async Task<SubmissionDetails> CreateSubmissionAsync(LinkSubmissionCreationDetails details, CancellationToken cancellationToken = default) =>
            await CreateSubmissionAsync(new CreateSubredditSubmissionCommand.Parameters()
            {
                SubredditName = _subredditName,
                Type = CreateSubredditSubmissionCommand.SubmissionType.Link,
                Title = details.Title,
                Url = details.Uri.AbsoluteUri,
                ForceResubmit = details.Resubmit
            },
            cancellationToken);

        /// <summary>
        /// Creates a text submission in the subreddit.
        /// </summary>
        /// <param name="details">The details of a text submission to create.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the created submission details.
        /// </returns>
        public async Task<SubmissionDetails> CreateSubmissionAsync(TextSubmissionCreationDetails details, CancellationToken cancellationToken = default) =>
            await CreateSubmissionAsync(new CreateSubredditSubmissionCommand.Parameters()
            {
                SubredditName = _subredditName,
                Type = CreateSubredditSubmissionCommand.SubmissionType.Self,
                Title = details.Title,
                Text = details.Text
            },
            cancellationToken);

        private async Task UpdateSubredditSubscriptionAsync(UpdateSubredditSubscriptionCommand.SubscriptionAction action, CancellationToken cancellationToken)
        {
            var commandParameters = new UpdateSubredditSubscriptionCommand.Parameters()
            {
                SubredditName = _subredditName,
                Action = action
            };

            var updateSubredditSubscriptionCommand = new UpdateSubredditSubscriptionCommand(commandParameters);

            await _client.ExecuteCommandAsync(updateSubredditSubscriptionCommand, cancellationToken).ConfigureAwait(false);
        }

        private async Task<SubmissionDetails> CreateSubmissionAsync(CreateSubredditSubmissionCommand.Parameters parameters, CancellationToken cancellationToken)
        {
            var createSubredditSubmissionCommand = new CreateSubredditSubmissionCommand(parameters);

            var response = await _client
                .ExecuteCommandAsync<JsonDataResponse<CreateSubmissionDataNode>>(createSubredditSubmissionCommand, cancellationToken)
                .ConfigureAwait(false);

            if (response.Json.Errors.Any())
            {
                throw new RedditClientApiException("Failed to create submission.", ErrorDetails.FromResponse(response));
            }

            return await GetSubmissionDetailsAsync(submissionId: response.Data.Id, cancellationToken);
        }

        private async Task<SubmissionDetails> GetSubmissionDetailsAsync(string submissionId, CancellationToken cancellationToken)
        {
            var commandParameters = new GetSubmissionDetailsWithCommentsCommand.Parameters()
            {
                SubmissionId = submissionId,
                // Don't fetch any comments since we're just interested in the submission details
                Limit = 0
            };

            var getSubmissionDetailsWithCommentsCommand = new GetSubmissionDetailsWithCommentsCommand(commandParameters);

            var submissionWithComments = await _client
                .ExecuteCommandAsync<Submission.SubmissionWithComments>(getSubmissionDetailsWithCommentsCommand, cancellationToken)
                .ConfigureAwait(false);

            return new SubmissionDetails(submissionWithComments.Submission);
        }
    }
}

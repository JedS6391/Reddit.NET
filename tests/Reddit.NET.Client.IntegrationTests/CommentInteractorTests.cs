using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Models.Public.Write;

namespace Reddit.NET.Client.IntegrationTests
{
    public class CommentInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetScriptClient();
        }

        [Test]
        public async Task GetDetailsAsync_ValidComment_ShouldGetDetails()
        {
            // https://old.reddit.com/r/AskReddit/comments/9whgf4/stan_lee_has_passed_away_at_95_years_old/e9kveve/
            var comment = _client.Comment(submissionId: "9whgf4", commentId: "e9kveve");

            var details = await comment.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.IsNotNull(details.Body);
        }

        [Test]
        public async Task GetDetailsAsync_ReloadModel_ShouldGetDetails()
        {
            var comment = _client.Comment(submissionId: "9whgf4", commentId: "e9kveve");

            var details = await comment.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.IsNotNull(details.Body);

            var lastLoadedAtUtcBeforeReload = details.LastLoadedAtUtc;

            await details.ReloadAsync(_client);

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.IsNotNull(details.Body);
            Assert.AreNotEqual(lastLoadedAtUtcBeforeReload, details.LastLoadedAtUtc);
        }

        [Test]
        public async Task ReplyAsync_ValidSubmissionValidComment_ShouldAddRepliesToSubmission()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            // Create a submission to comment on.
            var newSubmissionDetails = new TextSubmissionCreationDetails(
                title: $"Test submission {Guid.NewGuid()}",
                text: "Test submission made by Reddit.NET client integration tests.");

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);

            var submission = createdSubmission.Interact(_client);

            var firstCommentText = $"Test reply made by Reddit.NET client integration tests [{Guid.NewGuid()}].";

            var firstCommentDetails = await submission.ReplyAsync(firstCommentText);

            var replyCommentText = $"Test reply made by Reddit.NET client integration tests. [{Guid.NewGuid()}]";

            var replyCommentDetails = await firstCommentDetails
                .Interact(_client)
                .ReplyAsync(replyCommentText);

            Assert.IsNotNull(firstCommentDetails);
            Assert.IsNotNull(replyCommentDetails);
            Assert.AreEqual(firstCommentText, firstCommentDetails.Body);
            Assert.AreEqual(replyCommentText, replyCommentDetails.Body);
        }

        [Test]
        public async Task UpvoteAsync_ValidComment_ShouldUpvote()
        {
            var commentDetails = await GetRandomSubmissionComment();

            Assert.IsNotNull(commentDetails);

            var comment = commentDetails.Interact(_client);

            await comment.UpvoteAsync();

            await commentDetails.ReloadAsync(_client);

            Assert.AreEqual(VoteDirection.Upvoted, commentDetails.Vote);
        }

        [Test]
        public async Task DownvoteAsync_ValidComment_ShouldDownvote()
        {
            var commentDetails = await GetRandomSubmissionComment();

            Assert.IsNotNull(commentDetails);

            var comment = commentDetails.Interact(_client);

            await comment.DownvoteAsync();

            await commentDetails.ReloadAsync(_client);

            Assert.AreEqual(VoteDirection.Downvoted, commentDetails.Vote);
        }

        [Test]
        public async Task UnvoteAsync_ValidComment_ShouldUnvote()
        {
            var commentDetails = await GetRandomSubmissionComment();

            Assert.IsNotNull(commentDetails);

            var comment = commentDetails.Interact(_client);

            await comment.UnvoteAsync();

            await commentDetails.ReloadAsync(_client);

            Assert.AreEqual(VoteDirection.NoVote, commentDetails.Vote);
        }

        [Test]
        public async Task SaveAsync_ValidSubmission_ShouldSave()
        {
            // https://old.reddit.com/r/AskReddit/comments/9whgf4/stan_lee_has_passed_away_at_95_years_old/e9kveve/
            var comment = _client.Comment(submissionId: "9whgf4", commentId: "e9kveve");

            var commentDetails = await comment.GetDetailsAsync();

            Assert.IsNotNull(commentDetails);

            await comment.SaveAsync();

            await commentDetails.ReloadAsync(_client);

            Assert.IsTrue(commentDetails.Saved);
        }

        [Test]
        public async Task UnsaveAsync_ValidSubmission_ShouldUnsave()
        {
            // https://old.reddit.com/r/AskReddit/comments/9whgf4/stan_lee_has_passed_away_at_95_years_old/e9kveve/
            var comment = _client.Comment(submissionId: "9whgf4", commentId: "e9kveve");

            var commentDetails = await comment.GetDetailsAsync();

            Assert.IsNotNull(commentDetails);

            await comment.UnsaveAsync();

            await commentDetails.ReloadAsync(_client);

            Assert.IsFalse(commentDetails.Saved);
        }

        // Note we are only testing the failure scenario as otherwise it would
        // require a testing account with a balance which means $$$.
        [Test]
        public void AwardAsync_InsufficientCoins_ThrowsRedditClientApiException()
        {
            // https://old.reddit.com/r/AskReddit/comments/9whgf4/stan_lee_has_passed_away_at_95_years_old/e9kveve/
            var comment = _client.Comment(submissionId: "9whgf4", commentId: "e9kveve");

            var exception = Assert.ThrowsAsync<RedditClientApiException>(async () => await comment.AwardAsync());

            Assert.IsNotNull(exception);
            Assert.IsNotNull(exception.Details);
            Assert.AreEqual("INSUFFICIENT_COINS", exception.Details.Type);
        }

        [Test]
        public async Task EditAsync_ExistingComment_ShouldUpdateCommentText()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            // Create a submission to comment on.
            var newSubmissionDetails = new TextSubmissionCreationDetails(
                title: $"Test submission {Guid.NewGuid()}",
                text: "Test submission made by Reddit.NET client integration tests.");

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);

            var submission = createdSubmission.Interact(_client);

            var originalText = $"Test comment made by Reddit.NET client integration tests.";

            var commentDetails = await submission.ReplyAsync(originalText);

            Assert.IsNotNull(commentDetails);
            Assert.AreEqual(originalText, commentDetails.Body);

            var comment = commentDetails.Interact(_client);

            var updatedText = $"{commentDetails.Body} [edited {Guid.NewGuid()}]";

            await comment.EditAsync(updatedText);

            await commentDetails.ReloadAsync(_client);

            Assert.IsNotNull(commentDetails);
            Assert.AreEqual(updatedText, commentDetails.Body);
        }

        private async Task<CommentDetails> GetRandomSubmissionComment()
        {
            var subreddit = _client.Subreddit("askreddit");

            var submissionDetails = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.Hot)
                        .WithItemsPerRequest(1)
                        .WithMaximumItems(1))
                .FirstAsync();

            var submission = submissionDetails.Interact(_client);

            var commentThread = await submission.GetCommentsAsync();

            Assert.IsNotEmpty(commentThread);

            return commentThread[RandomNumberGenerator.GetInt32(commentThread.Count)].Details;
        }
    }
}

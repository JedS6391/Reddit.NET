using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Interactions;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Models.Public.Write;

namespace Reddit.NET.Client.IntegrationTests
{
    public class SubmissionInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetScriptClient();
        }

        [Test]
        public async Task GetDetailsAsync_ValidSubmission_ShouldGetDetails()
        {
            // https://old.reddit.com/r/AskReddit/comments/9whgf4/stan_lee_has_passed_away_at_95_years_old/
            var submission = _client.Submission(submissionId: "9whgf4");

            var details = await submission.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.AreEqual("Stan Lee has passed away at 95 years old", details.Title);
        }

        [Test]
        public async Task GetDetailsAsync_ReloadModel_ShouldGetDetails()
        {
            var submission = _client.Submission("9whgf4");

            var details = await submission.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.AreEqual("Stan Lee has passed away at 95 years old", details.Title);

            var lastLoadedAtUtcBeforeReload = details.LastLoadedAtUtc;

            await details.ReloadAsync(_client);

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.AreEqual("Stan Lee has passed away at 95 years old", details.Title);
            Assert.AreNotEqual(lastLoadedAtUtcBeforeReload, details.LastLoadedAtUtc);
        }

        [Test]
        public async Task GetCommentsAsync_HotSubmissions_ShouldGetComments()
        {
            var subreddit = _client.Subreddit("askreddit");

            var fiveHotSubmissions = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.Hot)
                        .WithItemsPerRequest(5)
                        .WithMaximumItems(5))
                .ToListAsync();

            Assert.IsNotNull(fiveHotSubmissions);
            Assert.IsNotEmpty(fiveHotSubmissions);

            foreach (var submissionDetails in fiveHotSubmissions)
            {
                var submission = submissionDetails.Interact(_client);

                var commentThreadNavigator = await submission.GetCommentsAsync(
                    limit: 20,
                    sort: SubmissionsCommentSort.Confidence);

                Assert.IsNotNull(commentThreadNavigator);
                // There may be less than 20 top-level comments available.
                Assert.IsTrue(commentThreadNavigator.Count <= 20);
            }
        }

        [Test]
        public async Task UpvoteAsync_ValidSubmission_ShouldUpvote()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var submissionDetails = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.New)
                        .WithItemsPerRequest(1)
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(submissionDetails);

            var submission = submissionDetails.Interact(_client);

            await submission.UpvoteAsync();

            await submissionDetails.ReloadAsync(_client);

            Assert.AreEqual(VoteDirection.Upvoted, submissionDetails.Vote);
        }

        [Test]
        public async Task DownvoteAsync_ValidSubmission_ShouldDownvote()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var submissionDetails = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.New)
                        .WithItemsPerRequest(1)
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(submissionDetails);

            var submission = submissionDetails.Interact(_client);

            await submission.DownvoteAsync();

            await submissionDetails.ReloadAsync(_client);

            Assert.AreEqual(VoteDirection.Downvoted, submissionDetails.Vote);
        }

        [Test]
        public async Task UnvoteAsync_ValidSubmission_ShouldUnvote()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var submissionDetails = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.New)
                        .WithItemsPerRequest(1)
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(submissionDetails);

            var submission = submissionDetails.Interact(_client);

            await submission.UnvoteAsync();

            await submissionDetails.ReloadAsync(_client);

            Assert.AreEqual(VoteDirection.NoVote, submissionDetails.Vote);
        }

        [Test]
        public async Task SaveAsync_ValidSubmission_ShouldSave()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var submissionDetails = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.New)
                        .WithItemsPerRequest(1)
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(submissionDetails);

            var submission = submissionDetails.Interact(_client);

            await submission.SaveAsync();

            await submissionDetails.ReloadAsync(_client);

            Assert.IsTrue(submissionDetails.Saved);
        }

        [Test]
        public async Task UnsaveAsync_ValidSubmission_ShouldUnsave()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var submissionDetails = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.New)
                        .WithItemsPerRequest(1)
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(submissionDetails);

            var submission = submissionDetails.Interact(_client);

            await submission.UnsaveAsync();

            await submissionDetails.ReloadAsync(_client);

            Assert.IsFalse(submissionDetails.Saved);
        }

        // Note we are only testing the failure scenario as otherwise it would
        // require a testing account with a balance which means $$$.
        [Test]
        public async Task AwardAsync_InsufficientCoins_ThrowsRedditClientApiException()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var submissionDetails = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.New)
                        .WithItemsPerRequest(1)
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(submissionDetails);

            var submission = submissionDetails.Interact(_client);

            var exception = Assert.ThrowsAsync<RedditClientApiException>(async () => await submission.AwardAsync());

            Assert.IsNotNull(exception);
            Assert.IsNotNull(exception.Details);
            Assert.AreEqual("INSUFFICIENT_COINS", exception.Details.Type);
        }

        [Test]
        public async Task EditAsync_TextSubmission_ShouldUpdateSubmissionText()
        {
            const string OriginalText = "Test submission made by Reddit.NET client integration tests.";

            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var newSubmissionDetails = new TextSubmissionCreationDetails(
                title: $"Test submission {Guid.NewGuid()}",
                text: OriginalText);

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);
            Assert.AreEqual(OriginalText, createdSubmission.SelfText);

            var submission = createdSubmission.Interact(_client);

            var updatedText = $"{createdSubmission.SelfText} [edited {Guid.NewGuid()}]";

            await submission.EditAsync(updatedText);

            await createdSubmission.ReloadAsync(_client);

            Assert.IsNotNull(createdSubmission);
            Assert.AreEqual(updatedText, createdSubmission.SelfText);
        }

        [Test]
        public async Task DeleteAsync_LinkSubmission_ShouldDeleteSubmission()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var newSubmissionDetails = new LinkSubmissionCreationDetails(
                title: $"Test submission {Guid.NewGuid()}",
                uri: new Uri("https://github.com/JedS6391/Reddit.NET"),
                resubmit: true);

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);

            var submission = createdSubmission.Interact(_client);

            await submission.DeleteAsync();

            // We can't simply check if a submission is deleted, so we instead get the latest submission and
            // check it is not the one we've deleted.
            var latestSubmission = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.New)
                        .WithItemsPerRequest(1)
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(latestSubmission);

            Assert.AreNotEqual(createdSubmission.Id, latestSubmission.Id);
            Assert.AreNotEqual(createdSubmission.Title, latestSubmission.Title);
        }

        [Test]
        public async Task DeleteAsync_TextSubmission_ShouldDeleteSubmission()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var newSubmissionDetails = new TextSubmissionCreationDetails(
                title: $"Test submission {Guid.NewGuid()}",
                text: "Test submission made by Reddit.NET client integration tests.");

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);

            var submission = createdSubmission.Interact(_client);

            await submission.DeleteAsync();

            // We can't simply check if a submission is deleted, so we instead get the latest submission and
            // check it is not the one we've deleted.
            var latestSubmission = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.New)
                        .WithItemsPerRequest(1)
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(latestSubmission);

            Assert.AreNotEqual(createdSubmission.Id, latestSubmission.Id);
            Assert.AreNotEqual(createdSubmission.Title, latestSubmission.Title);
        }
    }
}

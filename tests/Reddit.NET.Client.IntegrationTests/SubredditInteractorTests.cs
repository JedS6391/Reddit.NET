using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Models.Public.Write;

namespace Reddit.NET.Client.IntegrationTests
{
    public class SubredditInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetScriptClient();
        }

        [Test]
        public async Task GetDetailsAsync_ValidSubreddit_ShouldGetDetails()
        {
            var subreddit = _client.Subreddit("askreddit");

            var details = await subreddit.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Name);
        }

        [Test]
        public async Task GetDetailsAsync_ReloadModel_ShouldGetDetails()
        {
            var subreddit = _client.Subreddit("askreddit");

            var details = await subreddit.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Name);

            var lastLoadedAtUtcBeforeReload = details.LastLoadedAtUtc;

            await details.ReloadAsync(_client);

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Name);
            Assert.AreNotEqual(lastLoadedAtUtcBeforeReload, details.LastLoadedAtUtc);
        }

        [Test]
        public async Task StreamSubmissionsAsync_ValidSubreddit_ShouldStreamSubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var stream = subreddit.Stream.SubmissionsAsync();

            Assert.IsNotNull(stream);

            // We only take the first 100 as we don't want to actually wait polling in the test.
            var submissions = await stream.Take(100).ToListAsync();

            Assert.IsNotNull(submissions);
            Assert.IsNotEmpty(submissions);
            Assert.AreEqual(100, submissions.Count);
        }

        [Test]
        public async Task StreamCommentsAsync_ValidSubreddit_ShouldStreamComments()
        {
            var subreddit = _client.Subreddit("askreddit");

            var stream = subreddit.Stream.CommentsAsync();

            Assert.IsNotNull(stream);

            // See the comment above for the submissions stream regarding why the number 100.
            var comments = await stream.Take(100).ToListAsync();

            Assert.IsNotNull(comments);
            Assert.IsNotEmpty(comments);
            Assert.AreEqual(100, comments.Count);
        }

        [Test]
        public async Task GetSubmissionsAsync_OneHundredNewSubmissions_ShouldGetSubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var oneHundredNewSubmissions = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.New)
                        .WithMaximumItems(100))
                .ToListAsync();

            Assert.IsNotNull(oneHundredNewSubmissions);
            Assert.IsTrue(oneHundredNewSubmissions.Count == 100);
        }

        [Test]
        public async Task GetSubmissionsAsync_FiftyHotSubmissionsTwentyFivePerRequest_ShouldGetSubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var fiftyHotSubmissions = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.Hot)
                        .WithItemsPerRequest(25)
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(fiftyHotSubmissions);
            Assert.IsTrue(fiftyHotSubmissions.Count == 50);
        }

        [Test]
        public async Task GetSubmissionsAsync_FiftyTopAllTimeSubmissionsTwentyFivePerRequest_ShouldGetSubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var fiftyTopSubmissions = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.Top)
                        .WithTimeRange(TimeRangeSort.AllTime)
                        .WithItemsPerRequest(25)
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(fiftyTopSubmissions);
            Assert.IsTrue(fiftyTopSubmissions.Count == 50);
        }

        [Test]
        public async Task GetSubmissionsAsync_MultipleSubreddits_ShouldGetSubmissions()
        {
            var subreddit = _client.Subreddit("askreddit+pics");

            var fiftyHotSubmissions = await subreddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.Hot)
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(fiftyHotSubmissions);
            Assert.IsTrue(fiftyHotSubmissions.Count == 50);
        }

        [Test]
        public async Task SearchSubmissionsAsync_FiftyRelevantSubmissions_ShouldSearchSubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var searchResults = await subreddit
                .SearchSubmissionsAsync(
                    "Test",
                    builder => builder
                        .WithSort(SubredditSearchSort.Relevance)
                        .WithSyntax(SearchQuerySyntax.Lucene)
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(searchResults);
            Assert.IsTrue(searchResults.Count == 50);
        }

        [Test]
        public async Task SubscribeAsync_ValidSubreddit_ShouldSubscribe()
        {
            var subreddit = _client.Subreddit("askreddit");

            await subreddit.SubscribeAsync();

            var details = await subreddit.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.IsTrue(details.IsSubscribed);
        }

        [Test]
        public async Task UnsubscribeAsync_ValidSubreddit_ShouldUnsubscribe()
        {
            var subreddit = _client.Subreddit("askreddit");

            await subreddit.UnsubscribeAsync();

            var details = await subreddit.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.IsFalse(details.IsSubscribed);
        }

        [Test]
        public async Task CreateSubmissionAsync_LinkSubmissionWithResubmit_ShouldCreateLinkSubmission()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var newSubmissionDetails = new LinkSubmissionCreationDetails(
                title: $"Test submission {Guid.NewGuid()}",
                uri: new Uri("https://github.com/JedS6391/Reddit.NET"),
                resubmit: true);

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);
            Assert.IsTrue(createdSubmission.Title == newSubmissionDetails.Title);
            Assert.IsTrue(createdSubmission.Url == newSubmissionDetails.Uri.AbsoluteUri);
        }

        [Test]
        public void CreateSubmissionAsync_LinkSubmissionWithoutResubmit_ThrowsRedditClientApiException()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var newSubmissionDetails = new LinkSubmissionCreationDetails(
                title: $"Test submission {Guid.NewGuid()}",
                uri: new Uri("https://github.com/JedS6391/Reddit.NET"),
                resubmit: false);

            var exception = Assert.ThrowsAsync<RedditClientApiException>(async () =>
                await subreddit.CreateSubmissionAsync(newSubmissionDetails));

            Assert.IsNotNull(exception);
            Assert.IsNotNull(exception.Details);
            Assert.AreEqual("ALREADY_SUB", exception.Details.Type);
        }

        [Test]
        public async Task CreateSubmissionAsync_TextSubmission_ShouldCreateTextSubmission()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var newSubmissionDetails = new TextSubmissionCreationDetails(
                title: $"Test submission {Guid.NewGuid()}",
                text: "Test submission made by Reddit.NET client integration tests.");

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);
            Assert.IsTrue(createdSubmission.Title == newSubmissionDetails.Title);
            Assert.IsTrue(createdSubmission.SelfText == newSubmissionDetails.Text);
        }

        [Test]
        public async Task CreateSubmissionAsync_LinkSubmissionWithFlair_ShouldCreateLinkSubmissionWithFlair()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var flairs = await subreddit.GetSubmissionFlairsAsync();

            var flair = flairs.ElementAt(0);

            var newSubmissionDetails = new LinkSubmissionCreationDetails(
                title: $"Test submission {Guid.NewGuid()}",
                uri: new Uri("https://github.com/JedS6391/Reddit.NET"),
                resubmit: true,
                flairId: flair.Id);

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);
            Assert.IsTrue(createdSubmission.Title == newSubmissionDetails.Title);
            Assert.IsTrue(createdSubmission.Url == newSubmissionDetails.Uri.AbsoluteUri);
            Assert.IsTrue(createdSubmission.FlairId == flair.Id);
            Assert.IsTrue(createdSubmission.FlairText == flair.Text);
        }

        [Test]
        public async Task CreateSubmissionAsync_TextSubmissionWithFlair_ShouldCreateTextSubmissionWithFlair()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var flairs = await subreddit.GetSubmissionFlairsAsync();

            var flair = flairs.ElementAt(0);

            var newSubmissionDetails = new TextSubmissionCreationDetails(
                title: $"Test submission {Guid.NewGuid()}",
                text: "Test submission made by Reddit.NET client integration tests.",
                flairId: flair.Id);

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);
            Assert.IsTrue(createdSubmission.Title == newSubmissionDetails.Title);
            Assert.IsTrue(createdSubmission.SelfText == newSubmissionDetails.Text);
            Assert.IsTrue(createdSubmission.FlairId == flair.Id);
            Assert.IsTrue(createdSubmission.FlairText == flair.Text);
        }

        [Test]
        public async Task GetSubmissionFlairsAsync_SubredditWithFlairs_ShouldRetrieveAvailableFlairs()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var flairs = await subreddit.GetSubmissionFlairsAsync();

            Assert.IsNotNull(flairs);
            Assert.AreEqual(2, flairs.Count);
            Assert.That(flairs, Has.Exactly(1).Matches<SubmissionFlairDetails>(f => f.Text == "flair1"));
            Assert.That(flairs, Has.Exactly(1).Matches<SubmissionFlairDetails>(f => f.Text == "flair2"));
        }
    }
}

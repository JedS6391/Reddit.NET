using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Write;

namespace Reddit.NET.Client.IntegrationTests
{
    public class MultiredditInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetScriptClient();
        }

        [Test]
        public async Task GetDetailsAsync_ValidMultireddit_ShouldGetDetails()
        {
            var me = _client.Me();
            var myDetails = await me.GetDetailsAsync();

            var multireddit = _client.Multireddit(myDetails.Name, Environment.GetEnvironmentVariable("TEST_MULTIREDDIT_NAME"));

            var multiredditDetails = await multireddit.GetDetailsAsync();

            Assert.IsNotNull(multiredditDetails);
            Assert.AreEqual(Environment.GetEnvironmentVariable("TEST_MULTIREDDIT_NAME"), multiredditDetails.Name);
        }

        [Test]
        public async Task GetSubmissionsAsync_OneHundredNewSubmissions_ShouldGetSubmissions()
        {
            var me = _client.Me();
            var myDetails = await me.GetDetailsAsync();

            var multireddit = _client.Multireddit(myDetails.Name, Environment.GetEnvironmentVariable("TEST_MULTIREDDIT_NAME"));

            var oneHundredNewSubmissions = await multireddit
                .GetSubmissionsAsync(builder =>
                    builder
                        .WithSort(SubredditSubmissionSort.New)
                        .WithMaximumItems(100))
                .ToListAsync();

            Assert.IsNotNull(oneHundredNewSubmissions);
            Assert.IsTrue(oneHundredNewSubmissions.Count == 100);
        }

        [Test]
        public async Task GetSubmissionsAsync_FiftyTopAllTimeSubmissionsTwentyFivePerRequest_ShouldGetSubmissions()
        {
            var me = _client.Me();
            var myDetails = await me.GetDetailsAsync();

            var multireddit = _client.Multireddit(myDetails.Name, Environment.GetEnvironmentVariable("TEST_MULTIREDDIT_NAME"));

            var fiftyTopSubmissions = await multireddit
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
        public async Task DeleteAsync_ValidMultireddit_ShouldDelete()
        {
            var me = _client.Me();

            var multiredditDetails = await me.CreateMultiredditAsync(new MultiredditCreationDetails(
                name: "Test multireddit deletion",
                subreddits: new string[] { "askreddit", "pics" }));

            Assert.IsNotNull(multiredditDetails);
            Assert.AreEqual("Test multireddit deletion", multiredditDetails.Name);

            await multiredditDetails.Interact(_client).DeleteAsync();

            var exception = Assert.ThrowsAsync<RedditClientResponseException>(async () =>
                await multiredditDetails.ReloadAsync(_client));

            Assert.AreEqual(HttpStatusCode.NotFound, exception.StatusCode);
        }

        [Test]
        public async Task AddRemoveSubredditAsync_ValidMultireddit_ShouldAddSubreddit()
        {
            var me = _client.Me();
            var myDetails = await me.GetDetailsAsync();

            var multireddit = _client.Multireddit(myDetails.Name, Environment.GetEnvironmentVariable("TEST_MULTIREDDIT_NAME"));

            var multiredditDetails = await multireddit.GetDetailsAsync();

            Assert.IsNotNull(multiredditDetails);
            Assert.AreEqual(Environment.GetEnvironmentVariable("TEST_MULTIREDDIT_NAME"), multiredditDetails.Name);

            await multireddit.AddSubredditAsync("askreddit");

            await multiredditDetails.ReloadAsync(_client);

            CollectionAssert.Contains(multiredditDetails.Subreddits, "AskReddit");

            await multireddit.RemoveSubredditAsync("askreddit");

            await multiredditDetails.ReloadAsync(_client);

            CollectionAssert.DoesNotContain(multiredditDetails.Subreddits, "AskReddit");
        }

        [Test]
        public async Task AddSubredditAsync_InvalidSubreddit_ThrowsRedditClientApiExceptionException()
        {
            var me = _client.Me();
            var myDetails = await me.GetDetailsAsync();

            var multireddit = _client.Multireddit(myDetails.Name, Environment.GetEnvironmentVariable("TEST_MULTIREDDIT_NAME"));

            var multiredditDetails = await multireddit.GetDetailsAsync();

            Assert.IsNotNull(multiredditDetails);
            Assert.AreEqual(Environment.GetEnvironmentVariable("TEST_MULTIREDDIT_NAME"), multiredditDetails.Name);

            var exception = Assert.ThrowsAsync<RedditClientApiException>(async () =>
                await multireddit.AddSubredditAsync(Guid.NewGuid().ToString()));

            Assert.IsNotNull(exception);
            Assert.IsNotNull(exception.Details);
            Assert.AreEqual("BAD_SR_NAME", exception.Details.Type);
        }

        [Test]
        public async Task RemoveSubredditAsync_InvalidSubreddit_ThrowsRedditClientApiExceptionException()
        {
            var me = _client.Me();
            var myDetails = await me.GetDetailsAsync();

            var multireddit = _client.Multireddit(myDetails.Name, Environment.GetEnvironmentVariable("TEST_MULTIREDDIT_NAME"));

            var multiredditDetails = await multireddit.GetDetailsAsync();

            Assert.IsNotNull(multiredditDetails);
            Assert.AreEqual(Environment.GetEnvironmentVariable("TEST_MULTIREDDIT_NAME"), multiredditDetails.Name);

            var exception = Assert.ThrowsAsync<RedditClientApiException>(async () =>
                await multireddit.RemoveSubredditAsync(Guid.NewGuid().ToString()));

            Assert.IsNotNull(exception);
            Assert.IsNotNull(exception.Details);
            Assert.AreEqual("SUBREDDIT_NOEXIST", exception.Details.Type);
        }
    }
}

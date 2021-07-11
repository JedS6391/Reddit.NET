using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Write;

namespace Reddit.NET.Client.IntegrationTests
{
    public class MeInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetScriptClient();
        }

        [Test]
        public async Task GetDetailsAsync_ValidUser_ShouldGetDetails()
        {
            var me = _client.Me();

            var details = await me.GetDetailsAsync();

            Assert.IsNotNull(details);
        }

        [Test]
        public async Task GetSubredditsAsync_OneHundredSubreddits_ShouldGetSubreddits()
        {
            var me = _client.Me();

            var mySubreddits = await me
                .GetSubredditsAsync(builder =>
                    builder
                        .WithMaximumItems(100))
                .ToListAsync();

            Assert.IsNotNull(mySubreddits);
            Assert.IsNotEmpty(mySubreddits);
        }

        [Test]
        public async Task GetHistoryAsync_Submissions_ShouldGetSubmissionHistory()
        {
            var me = _client.Me();

            var history = await me
                .GetHistoryAsync(builder =>
                    builder
                        .WithType(UserHistoryType.Submitted)
                        .WithMaximumItems(10))
                .ToListAsync();

            Assert.IsNotNull(history);
            Assert.IsNotEmpty(history);
        }

        [Test]
        public async Task GetHistoryAsync_Saved_ShouldGetSavedHistory()
        {
            var me = _client.Me();

            var history = await me
                .GetHistoryAsync(
                    builder => builder
                        .WithType(UserHistoryType.Saved)
                        .WithMaximumItems(10))
                .ToListAsync();

            Assert.IsNotNull(history);
            Assert.IsNotEmpty(history);
        }

        [Test]
        public async Task GetMultiredditsAsync_ValidUser_ShouldGetMultireddits()
        {
            var me = _client.Me();

            var multireddits = await me.GetMultiredditsAsync();

            Assert.IsNotNull(multireddits);
            Assert.IsNotEmpty(multireddits);
        }

        [Test]
        public async Task GetKarmaBreakdownAsync_ValidUser_ShouldGetKarmabreakdown()
        {
            var me = _client.Me();

            var karmaBreakdownDetails = await me.GetKarmaBreakdownAsync();

            Assert.IsNotNull(karmaBreakdownDetails);
        }

        [Test]
        public async Task GetTrophiesAsync_ValidUser_ShouldGetTrophies()
        {
            var me = _client.Me();

            var trophyDetails = await me.GetTrophiesAsync();

            Assert.IsNotNull(trophyDetails);
        }

        [Test]
        public async Task CreateMultiredditAsync_ValidUser_ShouldCreateMultireddit()
        {
            var me = _client.Me();

            var newMultiredditDetails = await me.CreateMultiredditAsync(new MultiredditCreationDetails(
                name: "Test multireddit",
                subreddits: new string[] { "askreddit", "pics", "askscience" }
            ));

            Assert.IsNotNull(newMultiredditDetails);
            Assert.AreEqual("Test multireddit", newMultiredditDetails.Name);
            Assert.IsNotEmpty(newMultiredditDetails.Subreddits);
            CollectionAssert.AreEquivalent(
                new string[] { "AskReddit", "pics", "askscience" },
                newMultiredditDetails.Subreddits);

            var multireddit = newMultiredditDetails.Interact(_client);

            await multireddit.AddSubredditAsync("todayilearned");

            await newMultiredditDetails.ReloadAsync(_client);

            Assert.IsNotEmpty(newMultiredditDetails.Subreddits);
            CollectionAssert.AreEquivalent(
                new string[] { "AskReddit", "pics", "askscience", "todayilearned" },
                newMultiredditDetails.Subreddits);
        }
    }
}

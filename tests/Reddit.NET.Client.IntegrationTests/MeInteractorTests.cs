using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;

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
        public async Task GetSubmissionsAsync_FiftyHotSubmissionsTwentyFivePerRequest_ShouldGetFiftySubmissions()
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
        public async Task GetHistoryAsync_Submissions_ShouldGetSubmissions()
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
    }
}
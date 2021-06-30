using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;

namespace Reddit.NET.Client.IntegrationTests
{
    public class SubredditInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetClient();
        }

        [Test]
        public async Task GetDetailsAsync_ValidSubreddit_ShouldGetDetails()
        {
            var subreddit = _client.Subreddit("askreddit");

            var details = await subreddit.GetDetailsAsync();

            Assert.IsNotNull(details);
        }       
    }
}
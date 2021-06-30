using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;

namespace Reddit.NET.Client.IntegrationTests
{
    public class MeInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetClient();
        }

        [Test]
        public async Task GetDetailsAsync_ValidUser_ShouldGetDetails()
        {
            var me = _client.Me();

            var details = await me.GetDetailsAsync();

            Assert.IsNotNull(details);
        }       
    }
}
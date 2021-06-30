using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;

namespace Reddit.NET.Client.IntegrationTests
{
    public class InboxInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetClient();
        }

        [Test]
        public async Task GetMessagesAsync_ValidUser_ShouldGetMessages()
        {
            var inbox = _client.Me().Inbox();

            var messages = await inbox
                .GetMessagesAsync(builder =>
                    builder
                        .WithMessageType(InboxMessageType.All)
                        .WithMaximumItems(10))
                .ToListAsync();

            Assert.IsNotNull(messages);
            Assert.IsNotEmpty(messages);
        }         
    }
}
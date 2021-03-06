using System;
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
            _client = TestRedditClientProvider.GetScriptClient();
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

        [Test]
        public async Task ReplyAsync_ValidMessage_ShouldAddRepliesToMessage()
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

            var message = messages.First();
            var replyText = $"Test reply made by Reddit.NET client integration tests [{Guid.NewGuid()}].";

            var reply = await inbox.ReplyAsync(message, replyText);

            Assert.IsNotNull(reply);
            Assert.AreEqual(replyText, reply.Body);
        }
    }
}

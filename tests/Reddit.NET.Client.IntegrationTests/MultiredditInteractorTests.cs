using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.IntegrationTests.Shared;

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

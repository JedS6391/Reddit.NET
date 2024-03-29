using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Write;

namespace Reddit.NET.Client.IntegrationTests
{
    public class UserInteractorTests
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
            var user = _client.User(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"));

            var details = await user.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"), details.Name);
        }

        [Test]
        public async Task GetDetailsAsync_ReloadModel_ShouldGetDetails()
        {
            var user = _client.User(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"));

            var details = await user.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"), details.Name);

            var lastLoadedAtUtcBeforeReload = details.LastLoadedAtUtc;

            await details.ReloadAsync(_client);

            Assert.IsNotNull(details);
            Assert.AreEqual(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"), details.Name);
            Assert.AreNotEqual(lastLoadedAtUtcBeforeReload, details.LastLoadedAtUtc);
        }

        [Test]
        public async Task StreamSubmissionsAsync_ValidUser_ShouldStreamSubmissions()
        {
            var user = _client.User(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"));

            var stream = user.Stream.SubmissionsAsync();

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
            var user = _client.User(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"));

            var stream = user.Stream.CommentsAsync();

            Assert.IsNotNull(stream);

            // See the comment above for the submissions stream regarding why the number 100.
            var comments = await stream.Take(100).ToListAsync();

            Assert.IsNotNull(comments);
            Assert.IsNotEmpty(comments);
            Assert.AreEqual(100, comments.Count);
        }

        [Test]
        public async Task GetHistoryAsync_Submissions_ShouldGetSubmissionHistory()
        {
            var user = _client.User(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"));

            var history = await user
                .GetHistoryAsync(builder =>
                    builder
                        .WithType(UserHistoryType.Submitted)
                        .WithMaximumItems(10))
                .ToListAsync();

            Assert.IsNotNull(history);
            Assert.IsNotEmpty(history);
        }

        [Test]
        public void GetHistoryAsync_Saved_ThrowsArgumentException()
        {
            var user = _client.User(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"));

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                var history = await user
                    .GetHistoryAsync(
                        builder => builder
                            .WithType(UserHistoryType.Saved)
                            .WithMaximumItems(10))
                    .ToListAsync();
            });

            Assert.IsNotNull(exception);
        }

        [Test]
        public async Task SendMessageAsync_ValidUser_ShouldSendMessage()
        {
            var user = _client.User(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"));

            var privateMessage = new PrivateMessageCreationDetails(
                subject: $"Test message {Guid.NewGuid()}",
                body: "Test message sent by Reddit.NET client integration tests.");

            await user.SendMessageAsync(privateMessage);

            // To assert we need to retrieve the first message in the user's inbox to make sure
            // it matches what we sent above.
            var inbox = _client.Me().Inbox();

            var message = await inbox
                .GetMessagesAsync(builder =>
                    builder
                        .WithMessageType(InboxMessageType.PrivateMessages)
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(message);
            Assert.AreEqual(privateMessage.Body, message.Body);
        }

        [Test]
        public async Task FriendAsync_ValidUser_ShouldCreateFriendship()
        {
            var user = _client.User(Environment.GetEnvironmentVariable("TEST_REDDIT_FRIEND_USERNAME2"));

            var userDetails = await user.GetDetailsAsync();

            Assert.IsNotNull(userDetails);

            await user.FriendAsync();

            await userDetails.ReloadAsync(_client);

            Assert.IsNotNull(userDetails);
            Assert.IsTrue(userDetails.IsFriend);
        }

        [Test]
        public async Task UnfriendAsync_ValidUser_ShouldCreateFriendship()
        {
            var user = _client.User(Environment.GetEnvironmentVariable("TEST_REDDIT_FRIEND_USERNAME2"));

            var userDetails = await user.GetDetailsAsync();

            Assert.IsNotNull(userDetails);

            await user.UnfriendAsync();

            await userDetails.ReloadAsync(_client);

            Assert.IsNotNull(userDetails);
            Assert.IsFalse(userDetails.IsFriend);
        }
    }
}

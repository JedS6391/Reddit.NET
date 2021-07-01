using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Write;

namespace Reddit.NET.Client.IntegrationTests
{
    public class CommentInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetClient();
        }

        [Test]
        public async Task ReplyAsync_ValidSubmissionValidComment_ShouldAddRepliesToSubmission()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));            

            // Create a submission to comment on.
            var newSubmissionDetails = new TextSubmissionDetails(
                title: $"Test submission {Guid.NewGuid()}",
                text: "Test submission made by Reddit.NET client integration tests.");

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);            

            var submission = createdSubmission.Interact(_client);

            var firstCommentDetails = await submission.ReplyAsync(
                text: $"Test reply made by Reddit.NET client integration tests [{Guid.NewGuid()}].");
        
            var replyCommentDetails = await firstCommentDetails
                .Interact(_client)
                .ReplyAsync(text: $"Test reply made by Reddit.NET client integration tests. [{Guid.NewGuid()}]");            

            // Wait a little bit to ensure everything has been updated.
            await Task.Delay(TimeSpan.FromSeconds(4));

            var comments = await submission.GetCommentsAsync();

            Assert.AreEqual(firstCommentDetails.Body, comments[0].Details.Body);
            Assert.AreEqual(replyCommentDetails.Body, comments[0].Replies[0].Details.Body);
        }
    }
}
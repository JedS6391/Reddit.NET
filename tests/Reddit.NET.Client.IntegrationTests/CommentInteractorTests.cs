using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Write;

namespace Reddit.NET.Client.IntegrationTests
{
    public class CommentInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetScriptClient();
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

            var firstCommentText = $"Test reply made by Reddit.NET client integration tests [{Guid.NewGuid()}].";

            var firstCommentDetails = await submission.ReplyAsync(firstCommentText);

            var replyCommentText = $"Test reply made by Reddit.NET client integration tests. [{Guid.NewGuid()}]";

            var replyCommentDetails = await firstCommentDetails
                .Interact(_client)
                .ReplyAsync(replyCommentText);

            Assert.IsNotNull(firstCommentDetails);
            Assert.IsNotNull(replyCommentDetails);
            Assert.AreEqual(firstCommentText, firstCommentDetails.Body);
            Assert.AreEqual(replyCommentText, replyCommentDetails.Body);
        }
    }
}
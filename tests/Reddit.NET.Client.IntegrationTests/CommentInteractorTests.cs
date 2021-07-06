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
        public async Task GetDetailsAsync_ValidComment_ShouldGetDetails()
        {
            // https://old.reddit.com/r/AskReddit/comments/9whgf4/stan_lee_has_passed_away_at_95_years_old/e9kveve/
            var comment = _client.Comment(submissionId: "9whgf4", commentId: "e9kveve");

            var details = await comment.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.IsNotNull(details.Body);
        } 
        
        [Test]
        public async Task GetDetailsAsync_ReloadModel_ShouldGetDetails()
        {
            var comment = _client.Comment(submissionId: "9whgf4", commentId: "e9kveve");

            var details = await comment.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.IsNotNull(details.Body);

            var lastLoadedAtUtcBeforeReload = details.LastLoadedAtUtc;

            await details.ReloadAsync(_client);

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.IsNotNull(details.Body);
            Assert.AreNotEqual(lastLoadedAtUtcBeforeReload, details.LastLoadedAtUtc);             
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
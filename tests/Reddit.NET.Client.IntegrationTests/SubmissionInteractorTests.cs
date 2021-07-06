using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Interactions;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Models.Public.Write;

namespace Reddit.NET.Client.IntegrationTests
{
    public class SubmissionInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetScriptClient();
        }

        [Test]
        public async Task GetDetailsAsync_ValidSubmission_ShouldGetDetails()
        {
            // https://old.reddit.com/r/AskReddit/comments/9whgf4/stan_lee_has_passed_away_at_95_years_old/            
            var submission = _client.Submission(submissionId: "9whgf4");

            var details = await submission.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.AreEqual("Stan Lee has passed away at 95 years old", details.Title);
        } 
        
        [Test]
        public async Task GetDetailsAsync_ReloadModel_ShouldGetDetails()
        {
            var submission = _client.Submission("9whgf4");

            var details = await submission.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.AreEqual("Stan Lee has passed away at 95 years old", details.Title);

            var lastLoadedAtUtcBeforeReload = details.LastLoadedAtUtc;

            await details.ReloadAsync(_client);

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);
            Assert.AreEqual("Stan Lee has passed away at 95 years old", details.Title);
            Assert.AreNotEqual(lastLoadedAtUtcBeforeReload, details.LastLoadedAtUtc); 
        }                

        [Test]
        public async Task GetCommentsAsync_HotSubmissions_ShouldGetComments()
        {
            var subreddit = _client.Subreddit("askreddit");

            var fiveHotSubmissions = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.Hot) 
                        .WithItemsPerRequest(5)                 
                        .WithMaximumItems(5))
                .ToListAsync();

            Assert.IsNotNull(fiveHotSubmissions);
            Assert.IsNotEmpty(fiveHotSubmissions);

            foreach (var submissionDetails in fiveHotSubmissions)
            {
                var submission = submissionDetails.Interact(_client);

                var commentThreadNavigator = await submission.GetCommentsAsync(
                    limit: 20,
                    sort: SubmissionsCommentSort.Confidence);

                Assert.IsNotNull(commentThreadNavigator);
                // There may be less than 20 top-level comments available.
                Assert.IsTrue(commentThreadNavigator.Count <= 20);
            }
        }     

        [Test]
        public async Task UpvoteDownvoteUnvoteAsync_ValidSubmission_ShouldUpvote()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));            

            var submissionDetails = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.New) 
                        .WithItemsPerRequest(1)                 
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(submissionDetails);            

            var submission = submissionDetails.Interact(_client);

            await RunFuncAndAssertVote(submission, VoteDirection.Upvoted, submission => submission.UpvoteAsync());

            await RunFuncAndAssertVote(submission, VoteDirection.Downvoted, submission => submission.DownvoteAsync());

            await RunFuncAndAssertVote(submission, VoteDirection.NoVote, submission => submission.UnvoteAsync());

            static async Task RunFuncAndAssertVote(SubmissionInteractor submission, VoteDirection expectedVoteDirection, Func<SubmissionInteractor, Task> func)
            {
                // The vote endpoint seems to have a higher rate limit than others, and may be rate limited
                // regardless of the 60 request/min limit.
                // This delay is an attempt to avoid hitting that limit.
                await Task.Delay(TimeSpan.FromSeconds(2));

                await func.Invoke(submission);

                var submissionDetails = await submission.GetDetailsAsync();

                Assert.AreEqual(expectedVoteDirection, submissionDetails.Vote);
            }
        }

        [Test]
        public async Task SaveUnsaveAsync_ValidSubmission_ShouldSave()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));            

            var submissionDetails = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.New) 
                        .WithItemsPerRequest(1)                 
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(submissionDetails);            

            var submission = submissionDetails.Interact(_client);

            await RunFuncAndAssertSaved(submission, true, submission => submission.SaveAsync());

            await RunFuncAndAssertSaved(submission, false, submission => submission.UnsaveAsync());

            static async Task RunFuncAndAssertSaved(SubmissionInteractor submission, bool expectedSaved, Func<SubmissionInteractor, Task> func)
            {
                await func.Invoke(submission);

                var submissionDetails = await submission.GetDetailsAsync();

                Assert.AreEqual(expectedSaved, submissionDetails.Saved);
            }
        }

        [Test]
        public async Task DeleteSubmissionAsync_TextSubmission_ShouldDeleteSubmission()
        {
            var subreddit = _client.Subreddit(Environment.GetEnvironmentVariable("TEST_SUBREDDIT_NAME"));

            var newSubmissionDetails = new TextSubmissionDetails(
                title: $"Test submission {Guid.NewGuid()}",
                text: "Test submission made by Reddit.NET client integration tests.");

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);

            var submission = createdSubmission.Interact(_client);

            await submission.DeleteAsync();

            var submissionDetails = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.New) 
                        .WithItemsPerRequest(1)                 
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(submissionDetails);

            Assert.AreNotEqual(createdSubmission.Title, submissionDetails.Title);
        }                   
    }
}
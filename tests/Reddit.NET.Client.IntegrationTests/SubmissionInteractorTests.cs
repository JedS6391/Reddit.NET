using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Interactions;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.IntegrationTests
{
    public class SubmissionInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetClient();
        }

        [Test]
        public async Task GetDetailsAsync_HotSubmission_ShouldGetDetails()
        {
            var subreddit = _client.Subreddit("askreddit");

            var submissionDetails = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.Hot) 
                        .WithItemsPerRequest(1)                 
                        .WithMaximumItems(1))
                .FirstAsync();

            Assert.IsNotNull(submissionDetails);

            var submission = submissionDetails.Interact(_client);

            var details = await submission.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual("AskReddit", details.Subreddit);            
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
    }
}
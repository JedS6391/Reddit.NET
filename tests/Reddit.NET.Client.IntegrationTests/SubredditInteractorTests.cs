using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Write;

namespace Reddit.NET.Client.IntegrationTests
{
    public class SubredditInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetClient();
        }

        [Test]
        public async Task GetDetailsAsync_ValidSubreddit_ShouldGetDetails()
        {
            var subreddit = _client.Subreddit("askreddit");

            var details = await subreddit.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.AreEqual(details.Name, "AskReddit");
        }

        [Test]
        public async Task GetSubmissionsAsync_OneHundredNewSubmissions_ShouldGetOneHundredSubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var fiftyHotSubmissions = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.Hot)                  
                        .WithMaximumItems(100))
                .ToListAsync();

            Assert.IsNotNull(fiftyHotSubmissions);
            Assert.IsTrue(fiftyHotSubmissions.Count == 100);
        }        

        [Test]
        public async Task GetSubmissionsAsync_FiftyHotSubmissionsTwentyFivePerRequest_ShouldGetFiftySubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var fiftyHotSubmissions = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.Hot) 
                        .WithItemsPerRequest(25)                 
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(fiftyHotSubmissions);
            Assert.IsTrue(fiftyHotSubmissions.Count == 50);
        }

        [Test]
        public async Task GetSubmissionsAsync_FiftyTopAllTimeSubmissionsTwentyFivePerRequest_ShouldGetFiftySubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var fiftyTopSubmissions = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.Top) 
                        .WithTimeRange(TimeRangeSort.AllTime)
                        .WithItemsPerRequest(25)                 
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(fiftyTopSubmissions);
            Assert.IsTrue(fiftyTopSubmissions.Count == 50);
        } 

        [Test]
        public async Task SearchSubmissionsAsync_FiftyRelevantSubmissions_ShouldGetFiftySubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var searchResults = await subreddit
                .SearchSubmissionsAsync(
                    "Test",
                    builder => builder
                        .WithSort(SubredditSearchSort.Relevance)
                        .WithSyntax(SearchQuerySyntax.Lucene)
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(searchResults);
            Assert.IsTrue(searchResults.Count == 50);
        }

        [Test]
        public async Task SubscribeAsync_ValidSubreddit_ShouldSubscribe()
        {
            var subreddit = _client.Subreddit("askreddit");

            await subreddit.SubscribeAsync();

            var details = await subreddit.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.IsTrue(details.IsSubscribed);
        }

        [Test]
        public async Task UnsubscribeAsync_ValidSubreddit_ShouldSubscribe()
        {
            var subreddit = _client.Subreddit("askreddit");

            await subreddit.UnsubscribeAsync();

            var details = await subreddit.GetDetailsAsync();

            Assert.IsNotNull(details);
            Assert.IsFalse(details.IsSubscribed);
        }        

        [Test]
        public async Task CreateSubmissionAsync_LinkSubmission_ShouldCreateLinkSubmission()
        {
            var subreddit = _client.Subreddit("redditclienttests1");

            var newSubmissionDetails = new LinkSubmissionDetails(
                title: $"Test submission {Guid.NewGuid()}",
                uri: new Uri("https://github.com/JedS6391/Reddit.NET"));

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);
            Assert.IsTrue(createdSubmission.Title == newSubmissionDetails.Title);
            Assert.IsTrue(createdSubmission.Url == newSubmissionDetails.Uri.AbsoluteUri);
        }

        [Test]
        public async Task CreateSubmissionAsync_TextSubmission_ShouldCreateTextSubmission()
        {
            var subreddit = _client.Subreddit("redditclienttests1");

            var newSubmissionDetails = new TextSubmissionDetails(
                title: $"Test submission {Guid.NewGuid()}",
                text: "Test submission made by Reddit.NET client integration tests.");

            var createdSubmission = await subreddit.CreateSubmissionAsync(newSubmissionDetails);

            Assert.IsNotNull(createdSubmission);
            Assert.IsTrue(createdSubmission.Title == newSubmissionDetails.Title);
            Assert.IsTrue(createdSubmission.SelfText == newSubmissionDetails.Text);
        }           
    }
}
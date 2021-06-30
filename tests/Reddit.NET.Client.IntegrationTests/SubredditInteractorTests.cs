using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;

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
        public async Task GetSubmissionsAsync_FiftyBestSubmissionsTwentyFivePerRequest_ShouldGetFiftySubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var fiftyBestSubmissions = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.Best) 
                        .WithItemsPerRequest(25)                 
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(fiftyBestSubmissions);
            Assert.IsTrue(fiftyBestSubmissions.Count == 50);
        } 

        [Test]
        public async Task GetSubmissionsAsync_FiftyNewSubmissionsTwentyFivePerRequest_ShouldGetFiftySubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var fiftyNewSubmissions = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.New) 
                        .WithItemsPerRequest(25)                 
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(fiftyNewSubmissions);
            Assert.IsTrue(fiftyNewSubmissions.Count == 50);
        }

        [Test]
        public async Task GetSubmissionsAsync_FiftyRisingSubmissionsTwentyFivePerRequest_ShouldGetFiftySubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var fiftyRisingSubmissions = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.Rising) 
                        .WithItemsPerRequest(25)                 
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(fiftyRisingSubmissions);
            Assert.IsTrue(fiftyRisingSubmissions.Count == 50);
        }      

        [Test]
        public async Task GetSubmissionsAsync_FiftyTopSubmissionsTwentyFivePerRequest_ShouldGetFiftySubmissions()
        {
            var subreddit = _client.Subreddit("askreddit");

            var fiftyTopSubmissions = await subreddit
                .GetSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.Top) 
                        .WithItemsPerRequest(25)                 
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(fiftyTopSubmissions);
            Assert.IsTrue(fiftyTopSubmissions.Count == 50);
        }
    }
}
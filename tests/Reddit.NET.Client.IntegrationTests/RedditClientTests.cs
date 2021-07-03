using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;

namespace Reddit.NET.Client.IntegrationTests
{
    public class RedditClientTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetClient();
        }

        [Test]
        public async Task GetFrontPageSubmissionsAsync_FiftyHotSubmissionsTwentyFivePerRequest_ShouldGetFiftySubmissions()
        {
            var fiftyHotSubmissions = await _client
                .GetFrontPageSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.Hot) 
                        .WithItemsPerRequest(25)                 
                        .WithMaximumItems(50))
                .ToListAsync();

            Assert.IsNotNull(fiftyHotSubmissions);
            Assert.IsTrue(fiftyHotSubmissions.Count == 50);            
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;

namespace Reddit.NET.Client.IntegrationTests
{
    public class ReadOnlyClientTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetReadOnlyClient();
        }

        [Test]
        public async Task GetFrontPageSubmissionsAsync_ReadOnlyClient_ShouldGetSubmissions()
        {
            var fiftyHotSubmissions = await _client
                .GetFrontPageSubmissionsAsync(builder => 
                    builder                    
                        .WithSort(SubredditSubmissionSort.Hot)                                         
                        .WithMaximumItems(10))
                .ToListAsync();

            Assert.IsNotNull(fiftyHotSubmissions);
            Assert.IsTrue(fiftyHotSubmissions.Count == 10);            
        }      

        [Test]
        public void GetMeDetails_ReadOnlyClient_ThrowsCommandNotSupportedException()
        {
            var me = _client.Me();

            Assert.ThrowsAsync<CommandNotSupportedException>(async () => await me.GetDetailsAsync());
        }             
    }
}
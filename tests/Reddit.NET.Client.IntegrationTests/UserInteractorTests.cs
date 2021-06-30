using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.IntegrationTests.Shared;
using Reddit.NET.Client.Models.Public.Listings.Options;

namespace Reddit.NET.Client.IntegrationTests
{
    public class UserInteractorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetClient();
        }

        [Test]
        public async Task GetDetailsAsync_ValidUser_ShouldGetDetails()
        {
            var user = _client.User(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"));

            var details = await user.GetDetailsAsync();

            Assert.IsNotNull(details);
        }

        [Test]
        public async Task GetHistoryAsync_Submissions_ShouldGetSubmissions()
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
        public void GetHistoryAsync_Saved_ThrowsInvalidUserHistoryTypeException()
        {
            var user = _client.User(Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME"));

            var exception = Assert.ThrowsAsync<InvalidUserHistoryTypeException>(async () =>
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
    }
}
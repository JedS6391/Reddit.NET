using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.IntegrationTests.Shared;

namespace Reddit.NET.Client.IntegrationTests
{
    public class CommentThreadNavigatorTests
    {
        private RedditClient _client;

        [SetUp]
        public void Setup()
        {
            _client = TestRedditClientProvider.GetScriptClient();
        }

        [Test]
        public async Task LoadAllCommentsAsync_ValidSubmission_T()
        {
            // https://old.reddit.com/r/AskReddit/comments/ygv57q/what_movie_is_a_1010/
            var submission = _client.Submission("ygv57q");

            var comments = await submission.GetCommentsAsync();

            var beforeCount = comments.Count;

            await comments.LoadAllCommentsAsync(_client, limit: 3);

            Assert.IsTrue(beforeCount < comments.Count);
        }
    }
}

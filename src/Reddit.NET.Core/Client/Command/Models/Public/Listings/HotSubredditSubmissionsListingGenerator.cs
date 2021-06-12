using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Subreddits;

namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    public class HotSubredditSubmissionsListingGenerator : SubredditSubmissionsListingGenerator
    {
        public HotSubredditSubmissionsListingGenerator(
            RedditClient client,
            SubredditSubmissionsListingGenerator.ListingParameters parameters) 
            : base(client, parameters)
        {
        }

        internal async override Task<Submission.Listing> GetListingAsync(string after = null)
        {
            var getHotSubredditSubmissionsCommand = new GetHotSubredditSubmissionsCommand(new GetHotSubredditSubmissionsCommand.Parameters()
            {
                SubredditName = Parameters.SubredditName,
                After = after
            });

            var submissions = await Client.ExecuteCommandAsync<Submission.Listing>(getHotSubredditSubmissionsCommand);

            return submissions;
        }
    }
}
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Users;

namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    public class UserSubredditsListingGenerator 
        : ListingGenerator<Subreddit.Listing, Subreddit.Details, SubredditDetails>
    {
        private readonly RedditClient _client;

        public UserSubredditsListingGenerator(RedditClient client)
        {
            _client = client;
        }

        internal async override Task<Subreddit.Listing> GetInitialListingAsync() => await GetListingAsync().ConfigureAwait(false);

        internal async override Task<Subreddit.Listing> GetNextListingAsync(Subreddit.Listing currentListing)
        {
            if (string.IsNullOrEmpty(currentListing.Data.After))
            {
                return null;
            }

            return await GetListingAsync(currentListing.Data.After).ConfigureAwait(false);
        }

        private async Task<Subreddit.Listing> GetListingAsync(string after = null)
        {
            var getUserSubredditsCommand = new GetUserSubredditsCommand(new GetUserSubredditsCommand.Parameters()
            {
                After = after
            });

            var subreddits = await _client.ExecuteCommandAsync<Subreddit.Listing>(getUserSubredditsCommand);

            return subreddits;
        }

        internal override SubredditDetails MapThing(Thing<Subreddit.Details> thing) => new SubredditDetails(thing);
    }
}
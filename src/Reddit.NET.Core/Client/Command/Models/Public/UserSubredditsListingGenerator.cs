using System.Threading.Tasks;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Users;

namespace Reddit.NET.Core.Client.Command.Models.Public
{
    public class UserSubredditsListingGenerator : ListingGenerator<Subreddit.Listing, Subreddit.Details, SubredditDetails>
    {
        private readonly CommandFactory _commandFactory;
        private readonly IAuthenticator _authenticator;

        public UserSubredditsListingGenerator(CommandFactory commandFactory, IAuthenticator authenticator)
        {
            _commandFactory = commandFactory;
            _authenticator = authenticator;
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
            var authenticationContext = await _authenticator.GetAuthenticationContextAsync().ConfigureAwait(false);

            var getUserSubredditsCommand = _commandFactory.Create<GetUserSubredditsCommand>();

            var result = await getUserSubredditsCommand
                .ExecuteAsync(authenticationContext, new GetUserSubredditsCommand.Parameters()
                {
                    After = after
                })
                .ConfigureAwait(false);

            return result.Listing;
        }

        internal override SubredditDetails MapData(Subreddit.Details data) => new SubredditDetails()
        {
            Name = data.DisplayName,
            Title = data.Title
        };
    }
}
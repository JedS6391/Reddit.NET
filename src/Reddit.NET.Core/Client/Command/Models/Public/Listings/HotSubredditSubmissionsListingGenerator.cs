using System.Threading.Tasks;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Subreddits;

namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    public class HotSubredditSubmissionsListingGenerator : SubredditSubmissionsListingGenerator
    {
        public HotSubredditSubmissionsListingGenerator(
            CommandFactory commandFactory, 
            IAuthenticator authenticator,
            SubredditSubmissionsListingGenerator.ListingParameters parameters) 
            : base(commandFactory, authenticator, parameters)
        {
        }

        internal async override Task<Submission.Listing> GetListingAsync(string after = null)
        {
            var authenticationContext = await Authenticator.GetAuthenticationContextAsync().ConfigureAwait(false);

            var getHotSubredditSubmissionsCommand = CommandFactory.Create<GetHotSubredditSubmissionsCommand>();

            var result = await getHotSubredditSubmissionsCommand
                .ExecuteAsync(authenticationContext, new GetHotSubredditSubmissionsCommand.Parameters()
                {
                    SubredditName = Parameters.SubredditName,
                    After = after
                })
                .ConfigureAwait(false);

            return result.Listing;
        }
    }
}
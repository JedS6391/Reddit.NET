using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Models.Public.Listings;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Users;
using Reddit.NET.Core.Client.Interactions.Abstract;

namespace Reddit.NET.Core.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with the authenticated user.
    /// </summary>
    public class MeInteractor : IInteractor
    {
        private readonly CommandFactory _commandFactory; 
        private readonly IAuthenticator _authenticator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeInteractor" /> class.
        /// </summary>
        /// <param name="commandFactory">A <see cref="CommandFactory" /> instance used for creating commands for interactions with reddit.</param>
        /// <param name="authenticator">An <see cref="IAuthenticator" /> instance used to authenticate with reddit.</param>
        public MeInteractor(CommandFactory commandFactory, IAuthenticator authenticator)
        {
            _commandFactory = commandFactory;
            _authenticator = authenticator;
        }

        /// <summary>
        /// Gets the details of the authenticated user.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the authenticated user.</returns>
        public async Task<UserDetails> GetDetailsAsync()
        {            
            var authenticationContext = await _authenticator.GetAuthenticationContextAsync().ConfigureAwait(false);

            var getUserCommand = _commandFactory.Create<GetUserDetailsCommand>();

            var result = await getUserCommand.ExecuteAsync(authenticationContext, new GetUserDetailsCommand.Parameters());

            return result.Details;
        }

        /// <summary>
        /// Gets the subreddits the authenticated user is subscribed to.
        /// </summary>
        /// <returns>An asynchronous enumerator over the authenticated user's subreddits.</returns>
        public IAsyncEnumerable<SubredditDetails> GetSubredditsAsync() => new UserSubredditsListingGenerator(_commandFactory, _authenticator); 
    }
}
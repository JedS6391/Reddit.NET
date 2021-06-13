using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;
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
        private readonly RedditClient _client; 

        /// <summary>
        /// Initializes a new instance of the <see cref="MeInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        public MeInteractor(RedditClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets the details of the authenticated user.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the authenticated user.</returns>
        public async Task<UserDetails> GetDetailsAsync()
        {
            var getMyDetailsCommand = new GetMyDetailsCommand();

            var user = await _client.ExecuteCommandAsync<User.Details>(getMyDetailsCommand);

            return new UserDetails(user);
        }

        /// <summary>
        /// Gets the subreddits the authenticated user is subscribed to.
        /// </summary>
        /// <returns>An asynchronous enumerator over the authenticated user's subreddits.</returns>
        public IAsyncEnumerable<SubredditDetails> GetSubredditsAsync() => new UserSubredditsListingGenerator(_client); 
    }
}
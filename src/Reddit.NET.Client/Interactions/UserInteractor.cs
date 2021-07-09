using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reddit.NET.Client.Command.Users;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.Interactions.Abstract;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Models.Public.Write;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a user.
    /// </summary>
    public sealed class UserInteractor : IInteractor
    {
        private static readonly UserHistoryType[] s_unsupportedHistoryTypeOptions = new UserHistoryType[]
        {
            UserHistoryType.Saved,
            UserHistoryType.Upvoted,
            UserHistoryType.Downvoted
        };

        private readonly RedditClient _client;
        private readonly string _username;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="username">The name of the user to interact with.</param>
        public UserInteractor(RedditClient client, string username)
        {
            _client = client;
            _username = username;
        }

        /// <summary>
        /// Gets the details of the user.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the user.</returns>
        public async Task<UserDetails> GetDetailsAsync()
        {
            var getUserDetailsCommand = new GetUserDetailsCommand(new GetUserDetailsCommand.Parameters()
            {
                Username = _username
            });

            var user = await _client.ExecuteCommandAsync<User>(getUserDetailsCommand).ConfigureAwait(false);

            return new UserDetails(user);
        }

        /// <summary>
        /// Gets the history of the user.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the user's history.</returns>
        public IAsyncEnumerable<UserContentDetails> GetHistoryAsync(
            Action<UserHistoryListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new UserHistoryListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            if (s_unsupportedHistoryTypeOptions.Any(t => t.Name == optionsBuilder.Options.Type.Name))
            {
                throw new ArgumentException(
                    $"History type option {optionsBuilder.Options.Type.Name} is only supported for the currently authenticated user.");
            }

            return new UserHistoryListingEnumerable(
                _client,
                optionsBuilder.Options,
                new UserHistoryListingEnumerable.ListingParameters()
                {
                    UseAuthenticatedUser = false,
                    Username = _username
                });
        }

        /// <summary>
        /// Gets the trophies of the user.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the trophies of the user.</returns>
        public async Task<IReadOnlyList<TrophyDetails>> GetTrophiesAsync()
        {
            var getUserTrophiesCommand = new GetUserTrophiesCommand(new GetUserTrophiesCommand.Parameters()
            {
                Username = _username
            });

            var trophyList = await _client
                .ExecuteCommandAsync<TrophyList>(getUserTrophiesCommand)
                .ConfigureAwait(false);

            return trophyList
                .Data
                .Trophies
                .Select(t => new TrophyDetails(t))
                .ToList();
        }

        /// <summary>
        /// Sends a private message to the user.
        /// </summary>
        /// <param name="details">The details of the message to send.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendMessageAsync(PrivateMessageCreationDetails details)
        {
            var sendMessageCommand = new SendMessageCommand(new SendMessageCommand.Parameters()
            {
                Username = _username,
                Subject = details.Subject,
                Body = details.Body
            });

            await _client.ExecuteCommandAsync(sendMessageCommand).ConfigureAwait(false);
        }
    }
}

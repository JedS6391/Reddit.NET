using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Reddit.NET.Client.Command.Users;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Models.Public.Streams
{
    /// <summary>
    /// Provides access to streams of messages from the authenticated user's inbox.
    /// </summary>
    public sealed class InboxStreamProvider
    {
        private readonly RedditClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="InboxStreamProvider" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        public InboxStreamProvider(RedditClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets new unread messages in the authenticated user's inbox.
        /// </summary>
        /// <remarks>
        /// The enumerator will return the oldest messages first, retrieving 100 historical messages in the initial query.
        /// </remarks>
        /// <returns>An asynchronous enumerator over unread messages in the authenticated user's inbox.</returns>
        public IAsyncEnumerable<MessageDetails> UnreadMessagesAsync() =>
            PollingStream.Create(new PollingStreamOptions<IThing<Message.Details>, MessageDetails, string>(
                ct => GetUnreadMessagesAsync(ct),
                mapper: m => new MessageDetails(m),
                idSelector: s => s.Data.Id));

        private async Task<IEnumerable<IThing<Message.Details>>> GetUnreadMessagesAsync(CancellationToken cancellationToken)
        {
            var getMyInboxMessagesCommand = new GetMyInboxMessagesCommand(new GetMyInboxMessagesCommand.Parameters()
            {
                MessageType = InboxMessageType.Unread.Name,
                Limit = 100
            });

            var messages = await _client
                .ExecuteCommandAsync<Message.Listing>(getMyInboxMessagesCommand, cancellationToken)
                .ConfigureAwait(false);

            return messages.Children;
        }
    }
}

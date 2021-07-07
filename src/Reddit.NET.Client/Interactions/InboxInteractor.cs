using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Client.Command.UserContent;
using Reddit.NET.Client.Interactions.Abstract;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with the authenticated user's inbox.
    /// </summary>
    public class InboxInteractor : IInteractor
    {
        private readonly RedditClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="InboxInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        public InboxInteractor(RedditClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets the messages in the authenticated user's inbox.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the authenticated user's inbox messages.</returns>
        public IAsyncEnumerable<MessageDetails> GetMessagesAsync(
            Action<InboxMessagesListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new InboxMessagesListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new InboxMessagesListingEnumerable(
                _client,
                optionsBuilder.Options);
        }

        /// <summary>
        /// Replies to the message.
        /// </summary>
        /// <param name="message">The message to reply to.</param>
        /// <param name="text">The text of the reply to create.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the reply message details.
        /// </returns>
        public async Task<MessageDetails> ReplyAsync(MessageDetails message, string text)
        {
            var createCommentCommand = new ReplyToMessageCommand(new ReplyToMessageCommand.Parameters()
            {
                MessageFullName = message.FullName,
                Text = text
            });

            var response = await _client
                .ExecuteCommandAsync<JsonDataResponse<CreateCommentDataNode>>(createCommentCommand)
                .ConfigureAwait(false);

            return new MessageDetails(thing: response.Data.Things[0] as IThing<Message.Details>);
        }
    }
}

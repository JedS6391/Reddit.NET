using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Reddit.NET.Client.Command.Users;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over the messages in the authenticated user's inbox.
    /// </summary>
    public sealed class InboxMessagesListingEnumerable
        : ListingEnumerable<Message.Listing, Message.Details, MessageDetails, InboxMessagesListingEnumerable.Options>
    {
        private readonly RedditClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="InboxMessagesListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="options">The options available to the listing.</param>
        public InboxMessagesListingEnumerable(RedditClient client, Options options)
            : base(options)
        {
            _client = Requires.NotNull(client, nameof(client));
        }

        /// <inheritdoc />
        internal override async Task<Message.Listing> GetInitialListingAsync(CancellationToken cancellationToken) =>
            await GetListingAsync(cancellationToken).ConfigureAwait(false);

        /// <inheritdoc />
        internal override async Task<Message.Listing> GetNextListingAsync(Message.Listing currentListing, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(currentListing.Data.After))
            {
                return null;
            }

            return await GetListingAsync(cancellationToken, currentListing.Data.After).ConfigureAwait(false);
        }

        /// <inheritdoc />
        internal override MessageDetails MapThing(IThing<Message.Details> thing) => new MessageDetails(thing);

        private async Task<Message.Listing> GetListingAsync(CancellationToken cancellationToken, string after = null)
        {
            var getMyInboxMessagesCommand = new GetMyInboxMessagesCommand(new GetMyInboxMessagesCommand.Parameters()
            {
                MessageType = ListingOptions.MessageType.Name,
                Limit = ListingOptions.ItemsPerRequest,
                After = after
            });

            var messages = await _client
                .ExecuteCommandAsync<Message.Listing>(getMyInboxMessagesCommand, cancellationToken)
                .ConfigureAwait(false);

            return messages;
        }

        /// <summary>
        /// Defines the options available for <see cref="InboxMessagesListingEnumerable" />.
        /// </summary>
        public class Options : ListingEnumerableOptions
        {
            /// <summary>
            /// Gets the option to use for the message type.
            /// </summary>
            /// <remarks>Defaults to all.</remarks>
            internal InboxMessageType MessageType { get; set; } = InboxMessageType.All;

            /// <summary>
            /// Provides the ability to create <see cref="Options" /> instances.
            /// </summary>
            public class Builder : ListingEnumerableOptionsBuilder<Options, Builder>
            {
                /// <inheritdoc />
                protected override Builder Instance => this;

                /// <summary>
                /// Sets the message type option.
                /// </summary>
                /// <param name="type">The option to use for the message type.</param>
                /// <returns>The updated builder.</returns>
                public Builder WithMessageType(InboxMessageType type)
                {
                    Requires.NotNull(type, nameof(type));

                    Options.MessageType = type;

                    return this;
                }
            }
        }
    }
}

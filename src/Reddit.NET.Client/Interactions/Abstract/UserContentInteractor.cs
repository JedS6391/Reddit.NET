using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Reddit.NET.Client.Command.UserContent;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Interactions.Abstract
{
    /// <summary>
    /// Defines functionality shared by user content interactors i.e. submissions and comments.
    /// </summary>
    public abstract class UserContentInteractor : IInteractor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserContentInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="kind">The kind of the user content to interact with.</param>
        /// <param name="id">The base-36 ID of the user content to interact with.</param>
        protected UserContentInteractor(RedditClient client, string kind, string id)
        {
            Client = Requires.NotNull(client, nameof(client));
            Kind = Requires.NotNull(kind, nameof(kind));
            Id = Requires.NotNull(id, nameof(id));
        }

        /// <summary>
        /// Gets the <see cref="RedditClient" /> instance.
        /// </summary>
        protected RedditClient Client { get; }

        /// <summary>
        /// Gets the kind of the user content.
        /// </summary>
        protected string Kind { get; }

        /// <summary>
        /// Gets the base-36 ID of the user content.
        /// </summary>
        protected string Id { get; }

        /// <summary>
        /// Gets the fullname of the user content.
        /// </summary>
        protected string FullName => $"{Kind}_{Id}";

        /// <summary>
        /// Upvotes the content.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpvoteAsync(CancellationToken cancellationToken = default) =>
            await ApplyVoteAsync(VoteDirection.Upvoted, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Downvotes the content.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DownvoteAsync(CancellationToken cancellationToken = default) =>
            await ApplyVoteAsync(VoteDirection.Downvoted, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Removes any vote on the content.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UnvoteAsync(CancellationToken cancellationToken = default) =>
            await ApplyVoteAsync(VoteDirection.NoVote, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Saves the content.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveAsync(CancellationToken cancellationToken = default) =>
            await SaveOrUnsaveAsync(unsave: false, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Unsaves the content.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UnsaveAsync(CancellationToken cancellationToken = default) =>
            await SaveOrUnsaveAsync(unsave: true, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Adds a reply to the content.
        /// </summary>
        /// <remarks>
        /// The behaviour of this method depends on the kind of content being interacted with:
        /// <list type="bullet">
        ///     <item>
        ///         <term>Submission</term>
        ///         <description>Adds a new top-level comment on the submission</description>
        ///     </item>
        ///     <item>
        ///         <term>Comment</term>
        ///         <description>Adds a reply to the comment</description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="text">The text of the comment to create.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the created comment details.
        /// </returns>
        public async Task<CommentDetails> ReplyAsync(string text, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(text, nameof(text));

            var createCommentCommand = new CreateCommentCommand(new CreateCommentCommand.Parameters()
            {
                ParentFullName = FullName,
                Text = text
            });

            var response = await Client
                .ExecuteCommandAsync<JsonDataResponse<CreateCommentDataNode>>(createCommentCommand, cancellationToken)
                .ConfigureAwait(false);

            return new CommentDetails(thing: response.Data.Things[0] as IThing<Comment.Details>);
        }

        /// <summary>
        /// Deletes the content.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            var deleteContentCommand = new DeleteContentCommand(new DeleteContentCommand.Parameters()
            {
                FullName = FullName,
            });

            await Client.ExecuteCommandAsync(deleteContentCommand, cancellationToken).ConfigureAwait(false);
        }

        private async Task ApplyVoteAsync(VoteDirection direction, CancellationToken cancellationToken)
        {
            var applyVoteCommand = new ApplyVoteCommand(new ApplyVoteCommand.Parameters()
            {
                FullName = FullName,
                Direction = direction
            });

            await Client.ExecuteCommandAsync(applyVoteCommand, cancellationToken).ConfigureAwait(false);
        }

        private async Task SaveOrUnsaveAsync(bool unsave, CancellationToken cancellationToken)
        {
            var saveOrUnsaveContentCommand = new SaveOrUnsaveContentCommand(new SaveOrUnsaveContentCommand.Parameters()
            {
                FullName = FullName,
                Unsave = unsave
            });

            await Client.ExecuteCommandAsync(saveOrUnsaveContentCommand, cancellationToken).ConfigureAwait(false);
        }
    }
}

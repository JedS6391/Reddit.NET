using System.Threading.Tasks;
using Reddit.NET.Client.Command.UserContent;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Interactions.Abstract
{
    /// <summary>
    /// Defines functionality shared by user content interactors.
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
            Client = client;
            Kind = kind;
            Id = id;
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
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpvoteAsync() => await ApplyVoteAsync(ApplyVoteCommand.VoteDirection.Upvote).ConfigureAwait(false);

        /// <summary>
        /// Downvotes the content.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DownvoteAsync() => await ApplyVoteAsync(ApplyVoteCommand.VoteDirection.Downvote).ConfigureAwait(false);

        /// <summary>
        /// Removes any vote on the content.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UnvoteAsync() => await ApplyVoteAsync(ApplyVoteCommand.VoteDirection.Unvote).ConfigureAwait(false);

        /// <summary>
        /// Saves the content.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveAsync() => await SaveOrUnsaveAsync(unsave: false).ConfigureAwait(false);

        /// <summary>
        /// Unsaves the content.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UnsaveAsync() => await SaveOrUnsaveAsync(unsave: true).ConfigureAwait(false);

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
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the created comment details.
        /// </returns>
        public async Task<CommentDetails> ReplyAsync(string text)
        {
            var createCommentCommand = new CreateCommentCommand(new CreateCommentCommand.Parameters()
            {
                ParentFullName = FullName,
                Text = text
            });

            var response = await Client
                .ExecuteCommandAsync<JsonDataResponse<CreateCommentDataNode>>(createCommentCommand)
                .ConfigureAwait(false);

            return new CommentDetails(thing: response.Data.Things[0]);
        }

        private async Task ApplyVoteAsync(ApplyVoteCommand.VoteDirection direction) 
        {
            var applyVoteCommand = new ApplyVoteCommand(new ApplyVoteCommand.Parameters()
            {
                FullName = FullName,
                Direction = direction
            });

            await Client.ExecuteCommandAsync(applyVoteCommand).ConfigureAwait(false);           
        }

        private async Task SaveOrUnsaveAsync(bool unsave)
        {
            var saveOrUnsaveContentCommand = new SaveOrUnsaveContentCommand(new SaveOrUnsaveContentCommand.Parameters()
            {
                FullName = FullName,
                Unsave = unsave
            });

            await Client.ExecuteCommandAsync(saveOrUnsaveContentCommand).ConfigureAwait(false);
        }               
    }
}
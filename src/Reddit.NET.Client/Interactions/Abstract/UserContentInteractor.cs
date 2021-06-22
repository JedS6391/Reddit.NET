using System.Threading.Tasks;
using Reddit.NET.Client.Command.Vote;

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
        public async Task UpvoteAsync() => await ApplyVote(ApplyVoteCommand.VoteDirection.Upvote).ConfigureAwait(false);

        /// <summary>
        /// Downvotes the content.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DownvoteAsync() => await ApplyVote(ApplyVoteCommand.VoteDirection.Downvote).ConfigureAwait(false);        

        private async Task ApplyVote(ApplyVoteCommand.VoteDirection direction) 
        {
            var applyVoteCommand = new ApplyVoteCommand(new ApplyVoteCommand.Parameters()
            {
                Id = FullName,
                Direction = direction
            });

            await Client.ExecuteCommandAsync(applyVoteCommand).ConfigureAwait(false);           
        }         
    }
}
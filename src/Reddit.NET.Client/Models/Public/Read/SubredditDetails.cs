using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Interactions;
using System.Threading.Tasks;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a subreddit.
    /// </summary>
    public class SubredditDetails : IToInteractor<SubredditInteractor>, IReloadable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetails" /> class.
        /// </summary>
        /// <param name="thing">A <see cref="Thing{TData}" /> containing a subreddit's data.</param>
        internal SubredditDetails(IThing<Subreddit.Details> thing)
        {
            Name = thing.Data.DisplayName;
            Title = thing.Data.Title;
            Description = thing.Data.Description;
            Subscribers = thing.Data.Subscribers;
            Url = thing.Data.Url;
            IsSubscribed = thing.Data.IsSubscribed;
        }

        /// <summary>
        /// Gets the name of the subreddit.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the title of the subreddit.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the description of the subreddit.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the number of users subreddit to the subreddit.
        /// </summary>
        public long Subscribers { get; private set; }

        /// <summary>
        /// Gets the relative URL of the subreddit e.g. <c>"/r/pics/".</c>
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the currently authenticated user is subscribed to the subreddit.
        /// </summary>        
        public bool IsSubscribed { get; private set; }        

        /// <inheritdoc />
        public SubredditInteractor Interact(RedditClient client) => client.Subreddit(Name);

        /// <inheritdoc />
        public async Task ReloadAsync(RedditClient client)
        {
            var details = await Interact(client).GetDetailsAsync();

            Name = details.Name;
            Title = details.Title;
            Description = details.Description;
            Subscribers = details.Subscribers;
            Url = details.Url;
            IsSubscribed = details.IsSubscribed;
        }

        /// <inheritdoc />
        public override string ToString() => $"Subreddit [Name = {Name}, Title = {Title}]";
    }
}
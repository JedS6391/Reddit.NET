namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Defines the base attributes shared by all reddit API objects.
    /// </summary>
    /// <remarks>
    /// Each <see cref="IThing{TData}" /> has a kind which will define the type of data the thing represents 
    /// (e.g. a comment 'thing' will have different data than a subreddit 'thing').
    /// 
    /// This interface allows thing implementations to be cast in a number of ways, due to the covariant type parameter.
    /// </remarks>
    /// <see href="https://github.com/reddit-archive/reddit/wiki/JSON#thing-reddit-base-class" />
    /// <typeparam name="TData">The type of data this kind of thing contains.</typeparam>
    public interface IThing<out TData>
    {
        /// <summary>
        /// Gets the identifier of the thing.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the full name of the thing.
        /// </summary>
        /// <remarks>
        /// Full names are a combination of a thing's type and its identifier. For example, <c>t1_h0zsb4o</c>.
        /// </remarks>        
        string Name { get; }

        /// <summary>
        /// Gets the kind of the thing.
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// Gets the data of the thing.
        /// </summary>
        TData Data { get; }
    }
}
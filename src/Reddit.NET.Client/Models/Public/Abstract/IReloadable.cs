using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reddit.NET.Client.Models.Public.Abstract
{
    /// <summary>
    /// Defines the ability to reload a read-only model.
    /// </summary>
    public interface IReloadable
    {
        /// <summary>
        /// Gets the date and time when the model was last loaded.
        /// </summary>
        DateTimeOffset LastLoadedAtUtc { get; }

        /// <summary>
        /// Reloads the model.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to reload the model.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ReloadAsync(RedditClient client, CancellationToken cancellationToken = default);
    }
}

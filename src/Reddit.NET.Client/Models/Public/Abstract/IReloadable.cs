using System.Threading.Tasks;

namespace Reddit.NET.Client.Models.Public.Abstract
{
    /// <summary>
    /// Defines the ability to reload a read-only model.
    /// </summary>
    public interface IReloadable
    {
        /// <summary>
        /// Reloads the model.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to reload the model.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ReloadAsync(RedditClient client);
    }
}
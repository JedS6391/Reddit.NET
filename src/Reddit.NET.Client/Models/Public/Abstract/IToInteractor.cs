using Reddit.NET.Client.Interactions.Abstract;

namespace Reddit.NET.Client.Models.Public.Abstract
{
    /// <summary>
    /// Defines the ability to convert of a read-only model to an <see cref="IInteractor" />
    /// </summary>
    /// <typeparam name="TInteractor">The type of interactor to convert to.</typeparam>
    public interface IToInteractor<TInteractor>
        where TInteractor : IInteractor
    {
        /// <summary>
        /// Converts the model to an interactor of type <typeparamref name="TInteractor" />.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used by the interactor.</param>
        /// <returns>An interactor instance.</returns>
        TInteractor Interact(RedditClient client);
    }
}
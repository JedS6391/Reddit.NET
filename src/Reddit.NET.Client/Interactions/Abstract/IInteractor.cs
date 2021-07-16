namespace Reddit.NET.Client.Interactions.Abstract
{
    /// <summary>
    /// Represents a set of interactions with reddit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each interactor is responsible for a specific high-level concept e.g. subreddits, users, etc.
    /// </para>
    /// <para>
    /// Interactors are intended to be lightweight objects that don't require any network requests to create.
    /// </para>
    /// <para>
    /// Interactor instances are managed by the <see cref="RedditClient" /> class and cannot be directly instantiated.
    /// </para>
    /// </remarks>
    public interface IInteractor
    {
    }
}

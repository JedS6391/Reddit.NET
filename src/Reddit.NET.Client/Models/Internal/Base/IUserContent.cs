namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Represents content created by a user.
    /// </summary>
    public interface IUserContent : IVoteable, ICreated, ISaveable
    {
        /// <summary>
        /// Gets the author of the content.
        /// </summary>
        string Author { get; }
    }
}
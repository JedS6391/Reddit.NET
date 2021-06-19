namespace Reddit.NET.Core.Client.Command.Models.Internal.Base
{
    /// <summary>
    /// Represents content created by a user.
    /// </summary>
    public interface IUserContent : IVoteable, ICreated
    {
        /// <summary>
        /// Gets the author of the content.
        /// </summary>
        string Author { get; }
    }
}
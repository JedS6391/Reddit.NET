namespace Reddit.NET.Core.Client.Command.Abstract
{
    /// <summary>
    /// Represents an action that can be executed to interact with reddit.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets an identifier for the command.
        /// </summary>
        string Id { get; }
    }
}
using System.Net.Http;

namespace Reddit.NET.Client.Command
{
    /// <summary>
    /// Represents a command to execute against reddit.
    /// </summary>
    /// <remarks>
    /// A <see cref="ClientCommand" /> instance describes an HTTP request that can be executed by a <see cref="CommandExecutor" />.
    /// 
    /// Each HTTP interaction can be defined as a command, to allow components to operate in terms of 
    /// commands rather than HTTP requests.
    /// </remarks>
    public abstract class ClientCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommand" /> class.
        /// </summary>
        protected ClientCommand()
        {
        }

        /// <summary>
        /// Gets an identifier for the command.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Builds the <see cref="HttpRequestMessage" /> this command represents. 
        /// </summary>
        /// <returns>A <see cref="HttpRequestMessage" /> instance.</returns>
        public abstract HttpRequestMessage BuildRequest();
    }
}
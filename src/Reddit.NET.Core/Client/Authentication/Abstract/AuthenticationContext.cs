using System.Linq;
using Reddit.NET.Core.Client.Command.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Authentication.Abstract
{
    /// <summary>
    /// Represents a context which can be used when performing operations that require authentication.
    /// </summary>
    public abstract class AuthenticationContext
    {
        private readonly string[] _supportedCommandIds;

        /// <summary>
        /// Gets an identifier for the context.
        /// </summary>
        public abstract string Id { get; }


        /// <summary>
        /// Gets the authentication token associated with this context.
        /// </summary>
        public abstract Token Token { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationContext" /> class.
        /// </summary>
        /// <param name="supportedCommandIds">A list of <see cref="ICommand" /> identifiers that are supported by this context.</param>
        protected AuthenticationContext(string[] supportedCommandIds)
        {
            _supportedCommandIds = supportedCommandIds;
        }

        /// <summary>
        /// Determines whether the provided command can be executed in this context.
        /// </summary>
        /// <param name="command">A command to determine the execution rules for.</param>
        /// <returns><see langword="true" /> if the context supports the provided command; <see langword="false" /> otherwise.</returns>
        public bool CanExecute(ICommand command) => _supportedCommandIds.Any(id => id == command.Id);
    }
}
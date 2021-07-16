using System.Threading.Tasks;
using Microsoft;
using Reddit.NET.Client.Command;

namespace Reddit.NET.Client.Authentication.Abstract
{
    /// <summary>
    /// Provides base functionality for <see cref="IAuthenticator" /> implementations.
    /// </summary>
    public abstract class BaseAuthenticator : IAuthenticator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAuthenticator" /> class.
        /// </summary>
        /// <param name="commandExecutor">An <see cref="CommandExecutor" /> instance used to execute commands against reddit.</param>
        protected BaseAuthenticator(CommandExecutor commandExecutor)
        {
            CommandExecutor = Requires.NotNull(commandExecutor, nameof(commandExecutor));
        }

        /// <summary>
        /// Gets a <see cref="Command.CommandExecutor" /> instance used to execute commands against reddit.
        /// </summary>
        protected CommandExecutor CommandExecutor { get; }

        /// <inheritdoc />
        public abstract Task<AuthenticationContext> GetAuthenticationContextAsync();
    }
}

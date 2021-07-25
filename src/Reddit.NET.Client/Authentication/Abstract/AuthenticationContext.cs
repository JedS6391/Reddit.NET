using System;
using System.Linq;
using System.Reflection;
using Reddit.NET.Client.Authentication.Context;
using Reddit.NET.Client.Command;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication.Abstract
{
    /// <summary>
    /// Represents a context for performing operations that require authentication.
    /// </summary>
    public abstract class AuthenticationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationContext" /> class.
        /// </summary>
        protected AuthenticationContext()
        {
        }

        /// <summary>
        /// Gets an identifier for the context.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Gets the authentication token associated with this context.
        /// </summary>
        public abstract Token Token { get; }

        /// <summary>
        /// Determines whether the provided command can be executed in this context.
        /// </summary>
        /// <remarks>
        /// Whether a command is supported or not is determined by which <see cref="SupportedAuthenticationContextAttribute" /> attributes are applied.
        /// </remarks>
        /// <param name="command">A command to determine the execution rules for.</param>
        /// <returns><see langword="true" /> if the context supports the provided command; <see langword="false" /> otherwise.</returns>
        public bool CanExecute(ClientCommand command)
        {
            var supportedAuthenticationContextAttributes = command
                .GetType()
                .GetCustomAttributes<SupportedAuthenticationContextAttribute>();

            if (!supportedAuthenticationContextAttributes.Any())
            {
                throw new ArgumentException($"'{command.Id}' does not have any supported authentication context types.");
            }

            var contextType = GetType();

            return supportedAuthenticationContextAttributes.Any(a => a.Types.Contains(contextType));
        }
    }
}

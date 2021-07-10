using System;
using System.Linq;
using Microsoft;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Command;

namespace Reddit.NET.Client.Authentication.Context
{
    /// <summary>
    /// Used to indicate the supported <see cref="AuthenticationContext" /> types for a <see cref="ClientCommand" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class SupportedAuthenticationContextAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedAuthenticationContextAttribute" /> class.
        /// </summary>
        /// <param name="types">The <see cref="AuthenticationContext" /> types supported.</param>
        protected SupportedAuthenticationContextAttribute(params Type[] types)
        {
            Requires.NotNull(types, nameof(types));
            Requires.Argument(
                types.All(t => t.IsSubclassOf(typeof(AuthenticationContext))),
                nameof(types),
                "Types must be all be derived from AuthenticationContext.");

            Types = types;
        }

        /// <summary>
        /// Gets the supported <see cref="AuthenticationContext" /> types.
        /// </summary>
        public Type[] Types { get; }
    }
}

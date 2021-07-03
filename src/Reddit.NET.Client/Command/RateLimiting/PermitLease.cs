using System;

namespace Reddit.NET.Client.Command.RateLimiting
{
    /// <summary>
    /// Represents a lease for a permit to a rate limited resource.
    /// </summary>
    internal abstract class PermitLease : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the permit was acquired or not.
        /// </summary>
        public abstract bool IsAcquired { get; }

        /// <inheritdoc />        
        public abstract void Dispose();
    }
}
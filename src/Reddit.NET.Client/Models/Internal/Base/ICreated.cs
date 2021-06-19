using System;

namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Represents an object that has been created.
    /// </summary>
    /// <see href="https://github.com/reddit-archive/reddit/wiki/JSON#created-implementation" />
    public interface ICreated
    {
        /// <summary>
        /// Gets the time of creation in UTC.
        /// </summary>
        DateTimeOffset CreatedAtUtc { get; }
    }
}
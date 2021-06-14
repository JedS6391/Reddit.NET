using System;

namespace Reddit.NET.Core.Client.Command.Models.Internal.Base
{
    /// <summary>
    /// Represents an object that has been created.
    /// </summary>
    /// <see href="https://github.com/reddit-archive/reddit/wiki/JSON#created-implementation" />
    internal interface ICreated
    {
        /// <summary>
        /// Gets the time of creation in UTC.
        /// </summary>
        DateTimeOffset CreatedAtUtc { get; }
    }
}
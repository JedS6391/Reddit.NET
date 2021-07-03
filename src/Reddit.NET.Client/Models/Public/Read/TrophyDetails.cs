using System;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a trophy.
    /// </summary>
    public class TrophyDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrophyDetails" /> class.
        /// </summary>
        /// <param name="trophy">The TrophyDetails data.</param>
        internal TrophyDetails(Trophy trophy)
        {
            Name = trophy.Data.Name;
            Description = trophy.Data.Description;
            Icon40Url = trophy.Data.Icon40Url;
            Icon70Url = trophy.Data.Icon70Url;
            AwardedAtUtc = trophy.Data.AwardedAtUtc;
        }

        /// <summary>
        /// Gets the name of the trophy.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of the trophy.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the URL of the 41x41px icon for the trophy.
        /// </summary>
        public string Icon40Url { get; }

        /// <summary>
        /// Gets the URL of the 71x71px icon for the trophy.
        /// </summary>
        public string Icon70Url { get; }

        /// <summary>
        /// Gets the time the trophy was awarded in UTC.
        /// </summary>
        public DateTimeOffset? AwardedAtUtc { get; } 

        /// <inheritdoc />
        public override string ToString() => 
            $"Trophy [Name = {Name}, AwardedAtUtc = {AwardedAtUtc}]";
    }
}
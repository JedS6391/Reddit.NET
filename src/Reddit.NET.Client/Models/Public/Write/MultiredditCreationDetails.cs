using System.Collections.Generic;
using Microsoft;

namespace Reddit.NET.Client.Models.Public.Write
{
    /// <summary>
    /// Represents the details to create a multireddit.
    /// </summary>
    public class MultiredditCreationDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiredditCreationDetails" /> class.
        /// </summary>
        /// <param name="name">The name of the multireddit to create.</param>
        /// <param name="subreddits">The names of the subreddits comprising the multireddit to create.</param>
        public MultiredditCreationDetails(string name, string[] subreddits)
        {
            Requires.NotNullOrWhiteSpace(name, nameof(name));
            Requires.NotNullOrEmpty(subreddits, nameof(subreddits));

            Name = name;
            Subreddits = subreddits;
        }

        /// <summary>
        /// Gets the name of the multireddit to create.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the names of the subreddits comprising the multireddit to create.
        /// </summary>
        public IReadOnlyList<string> Subreddits { get; }
    }
}

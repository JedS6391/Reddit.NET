using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft;
using Reddit.NET.Client.Models.Internal.Base;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of an error.
    /// </summary>
    /// <remarks>
    /// See <a href="https://github.com/reddit-archive/reddit/blob/master/r2/r2/lib/errors.py" /> for more on the errors returned by reddit.
    /// </remarks>
    public class ErrorDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDetails" /> class.
        /// </summary>
        /// <param name="type">The unique type identifier of the error.</param>
        /// <param name="message">The message associated with the error.</param>
        /// <param name="fields">The input fields associated with the error, if any.</param>
        internal ErrorDetails(string type, string message, IReadOnlyList<string> fields)
        {
            Type = Requires.NotNull(type, nameof(type));
            Message = Requires.NotNull(message, nameof(message));
            Fields = fields ?? Array.Empty<string>();
        }

        /// <summary>
        /// Gets the unique type identifier of the error.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets the message associated with the error.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the input fields associated with the error, if any.
        /// </summary>
        public IReadOnlyList<string> Fields { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="ErrorDetails" /> class from the provided response object.
        /// </summary>
        /// <typeparam name="T">The type of the response data.</typeparam>
        /// <param name="response">The response object.</param>
        /// <returns>An <see cref="ErrorDetails" /> instance.</returns>
        internal static ErrorDetails FromResponse<T>(JsonDataResponse<T> response)
        {
            Requires.NotNull(response, nameof(response));

            if (response.Json.Errors == null || !response.Json.Errors.Any())
            {
                return null;
            }

            // We only care about the first error
            var error = response.Json.Errors.First();
            var fields = error.Count > 2 ?
                new string[] { error[2] } :
                Array.Empty<string>();

            return new ErrorDetails(
                type: error[0],
                message: error[1],
                fields: fields);
        }
    }
}

namespace Reddit.NET.WebApi.Services.Interfaces
{
    /// <summary>
    /// Provides the ability to manage session data.
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Gets the value associated with the provided key.
        /// </summary>
        /// <param name="key">The key of the value to retrieve from the session data.</param>
        /// <returns>The value associated with the provided key.</returns>
        string Get(string key);

        /// <summary>
        /// Stores the provided value in session data with the given key.
        /// </summary>
        /// <param name="key">The key to store the value against.</param>
        /// <param name="value">The value to store in session data.</param>
        void Store(string key, string value);

        /// <summary>
        /// Removes the value associated with the provided key.
        /// </summary>
        /// <param name="key">The key of the value to remove from the ession data.</param>
        void Remove(string key);
    }
}
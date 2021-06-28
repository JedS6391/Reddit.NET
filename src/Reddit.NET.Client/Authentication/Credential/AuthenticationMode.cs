namespace Reddit.NET.Client.Authentication.Credential
{
    /// <summary>
    /// Defines the modes of authentication supported by the client.
    /// </summary>
    /// <remarks>
    /// Based on the details from <see href="https://github.com/reddit-archive/reddit/wiki/OAuth2-App-Types" />.
    /// </remarks>
    public enum AuthenticationMode
    {
        /// <summary>
        /// The app runs as the backend of a web server, on a server that only YOU have access to.
        /// </summary>
        /// <remarks>
        /// Web applications are able to keep a secret and will use the authorization code grant type
        /// to obtain an access token after the user has approved access on their behalf.
        /// </remarks>
        WebApp,

        /// <summary>
        /// An app installed on a computer that you don't own or control.
        /// </summary>
        /// <remarks>
        /// Installed applications are not able to keep a secret and will use the authorization code grant type
        /// to obtain an access token after the user has approved access on their behalf.
        /// </remarks>
        InstalledApp,

        /// <summary>
        /// A single-user script.
        /// </summary>
        /// <remarks>
        /// Scripts are able to keep a secret and only have access to the account the client ID and secret is for.
        /// </remarks>
        Script,

        /// <summary>
        /// Similar to <see cref="WebApp" /> and <see cref="Script" />, 
        /// but only able to access non-authenticated functionality.
        /// </summary>
        /// <remarks>
        /// Should be used in contexts where a secret can be kept (i.e. script or web-app)
        /// </remarks>
        ReadOnly,

        /// <summary>
        /// Similar to <see cref="ReadOnly" /> but for app contexts.
        /// </summary>
        /// <remarks>
        /// Should be used in contexts where a secret cannot be kept (i.e. installed app, mobile device)
        /// </remarks>
        ReadOnlyInstalledApp,
    }
}
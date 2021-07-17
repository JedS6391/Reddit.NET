using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Subreddits
{
    /// <summary>
    /// Defines a command to update the subscription to a particular subreddit.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class UpdateSubredditSubscriptionCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSubredditSubscriptionCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public UpdateSubredditSubscriptionCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(UpdateSubredditSubscriptionCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "sr_name", _parameters.SubredditName },
                { "action", MapSubscriptionAction(_parameters.Action) }
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.Subreddit.Subscription),
                Content = new FormUrlEncodedContent(requestParameters)
            };

            return request;
        }

        private static string MapSubscriptionAction(SubscriptionAction action) =>
            action switch
            {
                SubscriptionAction.Subscribe => "sub",
                SubscriptionAction.Unubscribe => "unsub",
                _ => throw new ArgumentException($"Unsupported subscription action '{action}'."),
            };

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets or sets the name of the subreddit to update the subscription.
            /// </summary>
            public string SubredditName { get; set; }

            /// <summary>
            /// Gets or sets the subscription action to perform.
            /// </summary>
            public SubscriptionAction Action { get; set; }
        }

        /// <summary>
        /// Defines the subscription actions.
        /// </summary>
        public enum SubscriptionAction
        {
            /// <summary>
            /// Subscribes to the subreddit.
            /// </summary>
            Subscribe,

            /// <summary>
            /// Unsubscribes from the subreddit
            /// </summary>
            Unubscribe
        }
    }
}

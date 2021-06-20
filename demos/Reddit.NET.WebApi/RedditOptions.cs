using System;
using Microsoft.Extensions.Options;

namespace Reddit.NET.WebApi
{
    public class RedditOptions : IOptions<RedditOptions>
    {
        public RedditOptions Value => this;
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Uri RedirectUri { get; set; }
    }
}
using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Command.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;

namespace Reddit.NET.Core.Client.Command.Users
{
    public class GetUserDetailsCommand : AuthenticatedCommand<GetUserDetailsCommand.Parameters, GetUserDetailsCommand.Result, User>
    {
        public GetUserDetailsCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
            : base(httpClientFactory, loggerFactory)
        {
        }

        public override string Id => nameof(GetUserDetailsCommand);

        protected override HttpRequestMessage BuildRequest(Parameters parameters)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://oauth.reddit.com/api/v1/me")
            };

            return request;
        }

        protected override Result MapToResult(User response) => new Result()
        {
            Details = new UserDetails()
            {                
                Name = response.Name
            }
        };

        public class Parameters 
        {
        }

        public class Result 
        {
            public UserDetails Details { get; internal set; }
        }
    }
}
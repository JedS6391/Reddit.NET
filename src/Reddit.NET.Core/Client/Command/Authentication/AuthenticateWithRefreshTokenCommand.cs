using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Command.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Command.Authentication
{
    public class AuthenticateWithRefreshTokenCommand 
        : UnauthenticatedCommand<AuthenticateWithRefreshTokenCommand.Parameters, AuthenticateWithRefreshTokenCommand.Result, Token>
    {
        public AuthenticateWithRefreshTokenCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
            : base(httpClientFactory, loggerFactory)
        {
        }

        public override string Id => nameof(AuthenticateWithUsernamePasswordCommand);

        protected override HttpRequestMessage BuildRequest(Parameters parameters)
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", parameters.RefreshToken },                
                { "duration", "permanent" }
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://www.reddit.com/api/v1/access_token"),
                Content = new FormUrlEncodedContent(requestParameters)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(
                    $"{parameters.ClientId}:{parameters.ClientSecret}")));

            return request;            
        }

        protected override Result MapResponse(Token response) => new Result() 
        {
            Token = response
        };

        public class Parameters 
        {
            public string RefreshToken { get; set; }            
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }

        public class Result 
        {
            public Token Token { get; internal set; }
        }
    }
}
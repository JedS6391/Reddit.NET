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
    public class AuthenticateWithAuthorizationCodeCommand 
        : UnauthenticatedCommand<AuthenticateWithAuthorizationCodeCommand.Parameters, AuthenticateWithAuthorizationCodeCommand.Result, Token>
    {
        public AuthenticateWithAuthorizationCodeCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
            : base(httpClientFactory, loggerFactory)
        {
        }

        public override string Id => nameof(AuthenticateWithAuthorizationCodeCommand);

        protected override HttpRequestMessage BuildRequest(Parameters parameters)
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code", parameters.Code },   
                { "redirect_uri", parameters.RedirectUri },             
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

        protected override Result MapToResult(Token response) => new Result() 
        {
            Token = response
        };

        public class Parameters 
        {
            public string Code { get; set; }     
            public string RedirectUri { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }

        public class Result 
        {
            public Token Token { get; internal set; }
        }
    }
}
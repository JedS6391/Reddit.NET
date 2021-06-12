using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Reddit.NET.Core.Client.Command.Authentication
{
    public class AuthenticateWithClientCredentialsCommand : ClientCommand
    {
        private readonly AuthenticateWithClientCredentialsCommand.Parameters _parameters;

        public AuthenticateWithClientCredentialsCommand(AuthenticateWithClientCredentialsCommand.Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        public override string Id => nameof(AuthenticateWithUsernamePasswordCommand);

        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "grant_type", "client_credentials" },
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
                    $"{_parameters.ClientId}:{_parameters.ClientSecret}")));

            return request;            
        }

        public class Parameters 
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}
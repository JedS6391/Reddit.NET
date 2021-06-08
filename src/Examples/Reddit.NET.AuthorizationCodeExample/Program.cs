using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.AuthorizationCodeExample
{
    public class Program
    {
        const string RedirectUri = "http://127.0.0.1:8080/redirect";

        private static readonly HttpClient Http = new HttpClient();

        public static void Main(string[] args)
        {
            Console.WriteLine("Please follow the steps to retrieve an access token and refresh token you can use with the Reddit.NET client.");
            Console.WriteLine();

            var clientId = Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("REDDIT_CLIENT_SECRET");
            var state = Guid.NewGuid().ToString();

            Console.WriteLine();
            Console.WriteLine("1. Open the following link in your browser to complete the authorization process.");
            Console.WriteLine();

            var authorizationUri = GetAuthorizationUri(clientId, state);

            Console.WriteLine($"{authorizationUri}");
            Console.WriteLine();
            
            Console.WriteLine("2. Once you've completed authorization in the browser, copy the code returned and enter it below.");
            Console.WriteLine();

            var code = PromptForValue("Code");

            var token = RedeemCode(clientId, clientSecret, code);

            Console.WriteLine();            
            Console.WriteLine($"Access token: {token.AccessToken}");
            Console.WriteLine($"Refresh token: {token.RefreshToken}");            
        }

        private static Token RedeemCode(string clientId, string clientSecret, string code)
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", RedirectUri }                
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
                    $"{clientId}:{clientSecret}")));

            var response = Http.SendAsync(request).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Request failed.");
            }

            return response
                .Content
                .ReadFromJsonAsync<Token>()
                .GetAwaiter()
                .GetResult();
        }


        private static string PromptForValue(string valueName) 
        {
            string value = null;

            while (string.IsNullOrEmpty(value)) 
            {
                Console.Write($"{valueName}: ");

                value = Console.ReadLine();
            }

            return value;
        }

        private static string GetAuthorizationUri(string clientId, string state) =>
            $"https://www.reddit.com/api/v1/authorize?client_id={clientId}&response_type=code&state={state}&redirect_uri={RedirectUri}&duration=permanent&scope=creddits%20modcontributors%20modmail%20modconfig%20subscribe%20structuredstyles%20vote%20wikiedit%20mysubreddits%20submit%20modlog%20modposts%20modflair%20save%20modothers%20read%20privatemessages%20report%20identity%20livemanage%20account%20modtraffic%20wikiread%20edit%20modwiki%20modself%20history%20flair";
    }
}

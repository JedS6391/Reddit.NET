using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Reddit.NET.Core.Client.Command.Abstract
{
    public abstract class BaseCommand<TParameters, TResponse> : ICommand
    {
        const string ClientName = "Reddit.NET client";

        protected IHttpClientFactory HttpClientFactory { get; }
        protected ILogger<BaseCommand<TParameters, TResponse>> Logger { get; }
        
        public abstract string Id { get; }

        protected BaseCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            HttpClientFactory = httpClientFactory;
            Logger = loggerFactory.CreateLogger<BaseCommand<TParameters, TResponse>>();
        }

        protected async Task<TResponse> ExecuteRequest(HttpRequestMessage request)
        {            
            AddDefaultHeaders(request);

            var client = HttpClientFactory.CreateClient();

            Logger.LogDebug("Executing '{Id}' command", Id);

            var response = await client
                .SendAsync(request)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("'{Id}' command request failed [{StatusCode}]", Id, response.StatusCode);

                // TODO: Exception type
                throw new Exception();
            }                     

            return await response
                .Content
                .ReadFromJsonAsync<TResponse>()
                .ConfigureAwait(false);
        }

        private void AddDefaultHeaders(HttpRequestMessage request)
        {
            Logger.LogDebug("Adding default headers for '{Id}' command", Id);

            request.Headers.Add("User-Agent", ClientName);
        }
    }
}
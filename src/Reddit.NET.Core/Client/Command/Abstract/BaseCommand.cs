using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Command.Exceptions;

namespace Reddit.NET.Core.Client.Command.Abstract
{
    /// <summary>
    /// Represents a command which can execute an HTTP request and generate a response of a specific type.
    /// </summary>
    /// <typeparam name="TResponse">The type of response to generate.</typeparam>
    public abstract class BaseCommand<TResponse> : ICommand
    {
        const string ClientName = "Reddit.NET client";

        /// <summary>
        /// Gets an <see cref="IHttpClientFactory" /> instance used to create clients when executing requests.
        /// </summary>
        protected IHttpClientFactory HttpClientFactory { get; }

        /// <summary>
        /// Gets an <see cref="ILogger" /> instance used to write log messages.
        /// </summary>
        protected ILogger<BaseCommand<TResponse>> Logger { get; }
        
        /// <inheritdoc />
        public abstract string Id { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommand{TResponse}" /> class.
        /// </summary>
        /// <param name="httpClientFactory">An <see cref="IHttpClientFactory" /> instance used to create clients when executing requests</param>
        /// <param name="loggerFactory">An <see cref="ILoggerFactory" /> instance used create a logger for writing log messages.</param>
        protected BaseCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            HttpClientFactory = httpClientFactory;
            Logger = loggerFactory.CreateLogger<BaseCommand<TResponse>>();
        }

        /// <summary>
        /// Executes the provided HTTP request.
        /// </summary>
        /// <param name="request">An HTTP request.</param>
        /// <returns>The result of the HTTP request as an instance of type <typeparamref name="TResponse"/>.</returns>
        /// <exception cref="CommandExecutionFailedException">Thrown when the HTTP request results in a response with a non-successful HTTP status code.</exception>
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
                
                throw new CommandExecutionFailedException($"'{Id}' command request failed [{response.StatusCode}]");
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
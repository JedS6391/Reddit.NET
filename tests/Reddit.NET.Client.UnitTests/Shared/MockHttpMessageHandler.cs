using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Reddit.NET.Client.UnitTests.Shared
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        public MockHttpMessageHandler()
        {
        }

        public int RequestCount => SeenRequests.Count;

        public List<HttpRequestMessage> SeenRequests { get; } = new List<HttpRequestMessage>();

        public Func<HttpRequestMessage, Task<HttpResponseMessage>> RequestFunc { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            SeenRequests.Add(request);

            var response = await RequestFunc.Invoke(request);

            response.RequestMessage = request;

            return response;
        }
    }
}

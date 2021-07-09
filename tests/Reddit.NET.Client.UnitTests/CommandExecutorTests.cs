using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Reddit.NET.Client.Command;
using Reddit.NET.Client.Command.RateLimiting;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.UnitTests.Shared;

namespace Reddit.NET.Client.UnitTests
{
    [SuppressMessage("Reliability", "CA2012", Justification = "ValueTask result is not used in test assertions.")]
    public class CommandExecutorTests
    {
        private ILogger<CommandExecutor> _logger;
        private IHttpClientFactory _httpClientFactory;
        private IRateLimiter _rateLimiter;
        private MockHttpMessageHandler _httpMessageHandler;
        private CommandExecutor _commandExecutor;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger<CommandExecutor>>();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _rateLimiter = Substitute.For<IRateLimiter>();
            _httpMessageHandler = new MockHttpMessageHandler();

            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

            _httpClientFactory
                .CreateClient(Constants.HttpClientName)
                .Returns(new HttpClient(_httpMessageHandler));

            var successfulLease = SuccessfulLease();

            _rateLimiter
                .AcquireAsync(Arg.Any<int>())
                .Returns(successfulLease);

            _commandExecutor = new CommandExecutor(
                _logger,
                _httpClientFactory,
                _rateLimiter);
        }

        [Test]
        public async Task ExecuteCommandAsync_200Response_ReturnsResponse()
        {
            var command = new MockCommand();

            var response = await _commandExecutor.ExecuteCommandAsync(command);

            Assert.IsNotNull(response);
            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            _ = _rateLimiter
                .Received(1)
                .AcquireAsync(permitCount: 1);

            _httpClientFactory
                .Received(1)
                .CreateClient(Constants.HttpClientName);
        }

        [Test]
        [TestCase(HttpStatusCode.InternalServerError)]
        [TestCase(HttpStatusCode.BadGateway)]
        [TestCase(HttpStatusCode.ServiceUnavailable)]
        [TestCase(HttpStatusCode.GatewayTimeout)]
        public void ExecuteCommandAsync_5xxResponse_ThrowsRedditClientResponseException(HttpStatusCode statusCode)
        {
            var command = new MockCommand();

            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(statusCode));

            var exception = Assert.ThrowsAsync<RedditClientResponseException>(async () =>
                await _commandExecutor.ExecuteCommandAsync(command));

            Assert.IsNotNull(exception);
            Assert.AreEqual(statusCode, exception.StatusCode);
            // Initial request + 3 retries
            Assert.AreEqual(4, _httpMessageHandler.RequestCount);

            _rateLimiter
                .Received(1)
                .AcquireAsync(permitCount: 1);

            _httpClientFactory
                .Received(1)
                .CreateClient(Constants.HttpClientName);
        }

        [Test]
        public void ExecuteCommandAsync_429Response_ThrowsRedditClientRateLimitException()
        {
            var command = new MockCommand();

            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.TooManyRequests));

            var exception = Assert.ThrowsAsync<RedditClientRateLimitException>(async () =>
                await _commandExecutor.ExecuteCommandAsync(command));

            Assert.IsNotNull(exception);
            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            _rateLimiter
                .Received(1)
                .AcquireAsync(permitCount: 1);

            _httpClientFactory
                .Received(1)
                .CreateClient(Constants.HttpClientName);
        }

        [Test]
        public void ExecuteCommandAsync_400ResponseWithErrorContent_ThrowsRedditClientApiException()
        {
            const string ResponseContent = @"
{
    ""fields"": [
        ""name""
    ],
    ""explanation"": ""This community name isn't recognizable. Check the spelling and try again."",
    ""message"": ""Bad Request"",
    ""reason"": ""BAD_SR_NAME""
}";

            var command = new MockCommand();

            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ResponseContent)
                });

            var exception = Assert.ThrowsAsync<RedditClientApiException>(async () =>
                await _commandExecutor.ExecuteCommandAsync(command));

            Assert.IsNotNull(exception);
            Assert.AreEqual("BAD_SR_NAME", exception.Details.Type);
            Assert.AreEqual("This community name isn't recognizable. Check the spelling and try again.", exception.Details.Message);
            CollectionAssert.AreEqual(new string[] { "name" }, exception.Details.Fields);
            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            _rateLimiter
                .Received(1)
                .AcquireAsync(permitCount: 1);

            _httpClientFactory
                .Received(1)
                .CreateClient(Constants.HttpClientName);
        }

        [Test]
        public void ExecuteCommandAsync_400ResponseWithoutErrorContent_ThrowsRedditClientResponseException()
        {
            var command = new MockCommand();

            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var exception = Assert.ThrowsAsync<RedditClientResponseException>(async () =>
                await _commandExecutor.ExecuteCommandAsync(command));

            Assert.IsNotNull(exception);
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            _rateLimiter
                .Received(1)
                .AcquireAsync(permitCount: 1);

            _httpClientFactory
                .Received(1)
                .CreateClient(Constants.HttpClientName);
        }

        [Test]
        [TestCase(HttpStatusCode.NotFound)]
        [TestCase(HttpStatusCode.NotImplemented)]
        public void ExecuteCommandAsync_UnexpectedErrorResponse_ThrowsRedditClientResponseException(HttpStatusCode statusCode)
        {
            var command = new MockCommand();

            _httpMessageHandler.RequestFunc = (request) =>
                Task.FromResult(new HttpResponseMessage(statusCode));

            var exception = Assert.ThrowsAsync<RedditClientResponseException>(async () =>
                await _commandExecutor.ExecuteCommandAsync(command));

            Assert.IsNotNull(exception);
            Assert.AreEqual(statusCode, exception.StatusCode);
            Assert.AreEqual(1, _httpMessageHandler.RequestCount);

            _rateLimiter
                .Received(1)
                .AcquireAsync(permitCount: 1);

            _httpClientFactory
                .Received(1)
                .CreateClient(Constants.HttpClientName);
        }

        [Test]
        public void ExecuteCommandAsync_FailedToAcquirePermitLease_ThrowsRedditClientRateLimitException()
        {
            var command = new MockCommand();

            var failedLease = FailedLease();

            _rateLimiter
                .AcquireAsync(Arg.Any<int>())
                .Returns(failedLease);

            var exception = Assert.ThrowsAsync<RedditClientRateLimitException>(async () =>
                await _commandExecutor.ExecuteCommandAsync(command));

            Assert.IsNotNull(exception);
            Assert.AreEqual(0, _httpMessageHandler.RequestCount);

            _rateLimiter
                .Received(1)
                .AcquireAsync(permitCount: 1);

            _httpClientFactory
                .DidNotReceive()
                .CreateClient(Constants.HttpClientName);
        }

        private static ValueTask<PermitLease> SuccessfulLease()
        {
            var lease = Substitute.For<PermitLease>();

            lease.IsAcquired.Returns(true);

            return ValueTask.FromResult(lease);
        }

        private static ValueTask<PermitLease> FailedLease()
        {
            var lease = Substitute.For<PermitLease>();

            lease.IsAcquired.Returns(false);

            return ValueTask.FromResult(lease);
        }

        private class MockCommand : ClientCommand
        {
            public override string Id => nameof(MockCommand);

            public override HttpRequestMessage BuildRequest() =>
                new HttpRequestMessage(HttpMethod.Get, $"http://localhost/{Guid.NewGuid()}");
        }
    }
}

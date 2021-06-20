using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reddit.NET.WebApi.Services.Interfaces;

namespace Reddit.NET.WebApi.Controllers
{
    [ApiController]
    [Route("v1/api/reddit/authentication")]
    public class RedditAuthenticationController : ControllerBase
    {
        private readonly ILogger<RedditAuthenticationController> _logger;
        private readonly IRedditService _redditService;

        public RedditAuthenticationController(
            ILogger<RedditAuthenticationController> logger,
            IRedditService redditService)
        {
            _logger = logger;
            _redditService = redditService;
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            var authorizationUri = _redditService.GenerateAuthorizationUri();

            return Redirect(authorizationUri.AbsoluteUri);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout([Required, FromQuery] Guid sessionId)
        {
            await _redditService.EndSessionAsync(sessionId);

            return Ok();
        }

        [HttpGet]
        [Route("callback")]
        public async Task<IActionResult> Callback(
            [Required, FromQuery] string state, 
            [Required, FromQuery] string code)
        {
            var sessionId = await _redditService.CompleteAuthorizationAsync(state, code);

            return Ok(new 
            {
                SessionId = sessionId
            });
        }
    }
}

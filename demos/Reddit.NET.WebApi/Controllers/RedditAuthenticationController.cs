using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reddit.NET.WebApi.Services.Interfaces;

namespace Reddit.NET.WebApi.Controllers
{
    /// <summary>
    /// An API controller for reddit authentication.
    /// </summary>
    [ApiController]
    [Route("v1/api/reddit/authentication")]
    public class RedditAuthenticationController : ControllerBase
    {
        private readonly IRedditService _redditService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditAuthenticationController" /> class.
        /// </summary>
        /// <param name="redditService">A service for interacting with reddit.</param>
        public RedditAuthenticationController(IRedditService redditService)
        {
            _redditService = redditService;
        }

        /// <summary>
        /// Starts the reddit OAuth login process.
        /// </summary>
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            var authorizationUri = _redditService.GenerateAuthorizationUri();

            return Redirect(authorizationUri.AbsoluteUri);
        }

        /// <summary>
        /// Completes the reddit OAuth login process.
        /// </summary>
        /// <remarks>
        /// The session ID returned allows the user to perform further authenticated actions.
        /// </remarks>
        /// <param name="state">The OAuth state parameter.</param>
        /// <param name="code">The OAuth authorization code parameter.</param>
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

        /// <summary>
        /// Ends the reddit session.
        /// </summary>
        /// <param name="sessionId">The session identifier obtained upon completion of the login process.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout([Required, FromQuery] Guid sessionId)
        {
            await _redditService.EndSessionAsync(sessionId);

            return Ok();
        }
    }
}

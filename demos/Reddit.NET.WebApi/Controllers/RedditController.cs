using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reddit.NET.WebApi.Services.Interfaces;

namespace Reddit.NET.WebApi.Controllers
{
    /// <summary>
    /// An API controller for reddit.
    /// </summary>
    [ApiController]
    [Route("v1/api/reddit/")]
    public class RedditController : ControllerBase
    {
        private readonly IRedditService _redditService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditController" /> class.
        /// </summary>
        /// <param name="redditService">A service for interacting with reddit.</param>
        public RedditController(IRedditService redditService)
        {
            _redditService = redditService;
        }

        /// <summary>
        /// Gets the details of the user associated with the provided session.
        /// </summary>
        /// <param name="sessionId">The session identifier obtained upon completion of the login process.</param>
        [HttpGet]
        [Route("user/details")]
        public async Task<IActionResult> GetUserDetails([Required, FromQuery] Guid sessionId)
        {
            var client = await _redditService.GetClientAsync(sessionId);

            var userDetails = await client.Me().GetDetailsAsync();

            return Ok(new
            {
                Username = userDetails.Name
            });
        }


        /// <summary>
        /// Gets the subscribed subreddits of the user associated with the provided session.
        /// </summary>
        /// <param name="sessionId">The session identifier obtained upon completion of the login process.</param>
        [HttpGet]
        [Route("user/subreddits")]
        public async Task<IActionResult> GetUserSubreddits([Required, FromQuery] Guid sessionId)
        {
            var client = await _redditService.GetClientAsync(sessionId);

            var userSubreddits = await client
                .Me()
                .GetSubredditsAsync()
                .ToListAsync();

            return Ok(userSubreddits);
        }
    }
}

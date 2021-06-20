using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reddit.NET.WebApi.Services.Interfaces;

namespace Reddit.NET.WebApi.Controllers
{
    [ApiController]
    [Route("v1/api/reddit/")]
    public class RedditController : ControllerBase
    {
        private readonly ILogger<RedditController> _logger;
        private readonly IRedditService _redditService;

        public RedditController(
            ILogger<RedditController> logger,
            IRedditService redditService)
        {
            _logger = logger;
            _redditService = redditService;
        }

        [HttpGet]
        [Route("user-details")]
        public async Task<IActionResult> GetUserDetails([Required, FromQuery] Guid sessionId)
        {
            var client = await _redditService.GetClientAsync(sessionId);

            var userDetails = await client.Me().GetDetailsAsync();

            return Ok(new
            {
                Username = userDetails.Name
            });
        }
    }
}
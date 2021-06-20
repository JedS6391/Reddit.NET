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

        [HttpGet]
        [Route("callback")]
        public async Task<IActionResult> Callback([Required] string state, [Required] string code)
        {
            await _redditService.CompleteAuthorizationAsync(state, code);

            return Ok();
        }
    }
}

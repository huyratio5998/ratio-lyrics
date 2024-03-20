using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ratio_Lyrics.Web.Services.Abstraction;

namespace Ratio_Lyrics.Api.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class SiteSettingController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICacheService _cacheService;                

        public SiteSettingController(ILogger<SiteSettingController> logger, ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ClearCache([FromBody] string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return BadRequest();

            await _cacheService.ClearCacheAsync(key);

            return Ok();
        }
    }
}

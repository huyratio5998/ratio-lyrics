using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Services.Abstraction;
using Ratio_Lyrics.Web.Services.Implements;

namespace Ratio_Lyrics.Api.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class SongController : ControllerBase
    {
        private readonly ILogger<SongController> _logger;
        private readonly ISongService _songService;        
        private readonly IServiceProvider _serviceProvider;
        private RunUpdateViewsBackgroundTask _updateViewTask;
        private const int SearchNumberDefault = 6;
        private const int MaxOrderTimeExecuteMinutes = 5;

        public SongController(ILogger<SongController> logger, ISongService songService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _songService = songService;
            _serviceProvider = serviceProvider;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();

            var request = new BaseQueryParams
            {
                SearchText = name,
                PageSize = SearchNumberDefault
            };
            var song = await _songService.GetSuggestSongsAsync(request);
            if (song == null) return NotFound();

            return Ok(song);
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] int id)
        {
            if(id <= 0) return BadRequest();

            var song = await _songService.GetSongAsync(id, false);            
            if (song == null) return NotFound();

            await UpdateViewsInBackground(id, _serviceProvider, _logger);

            return Ok(song);
        }

        private async Task UpdateViewsInBackground(int id, IServiceProvider provider, ILogger logger)
        {
            var timeoutCts = new CancellationTokenSource();
            timeoutCts.CancelAfter(TimeSpan.FromMinutes(MaxOrderTimeExecuteMinutes));
            _updateViewTask = new RunUpdateViewsBackgroundTask(id, provider, logger);
            await _updateViewTask.StartAsync(timeoutCts.Token);            
        }
    }
}

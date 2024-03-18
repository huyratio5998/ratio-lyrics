﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Services.Abstraction;
using Ratio_Lyrics.Web.Services.Implements;
using System;

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
        private const int SearchNumberDefault = 5;
        private const int MaxOrderTimeExecuteMinutes = 5;

        public SongController(ILogger<SongController> logger, ISongService songService, IHostedService hostedService)
        {
            _logger = logger;
            _songService = songService;            
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
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
        public async Task<IActionResult> Get([FromQuery] int id)
        {
            if(id <= 0) return BadRequest();

            var song = await _songService.GetSongAsync(id, false);            
            if (song == null) return NotFound();

            //var hostedService = new RunUpdateViewsBackgroundTask(song.Id, _songService, _logger);
            //await hostedService.StartAsync(timeoutCts.Token);            

            //woud run in background queue. => ...
            var timeoutCts = new CancellationTokenSource();
            timeoutCts.CancelAfter(TimeSpan.FromMinutes(MaxOrderTimeExecuteMinutes));
            await _songService.UpdateViewsAsync(id, timeoutCts.Token);

            return Ok(song);
        }
    }
}

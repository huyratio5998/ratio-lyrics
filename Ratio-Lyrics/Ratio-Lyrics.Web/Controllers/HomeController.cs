using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Services.Abstraction;
using System.Diagnostics;

namespace Ratio_Lyrics.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISongService _songService;
        private readonly IMediaPlatformService _mediaPlatformService;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, ISongService songService, IMediaPlatformService mediaPlatformService, IMapper mapper)
        {
            _logger = logger;
            _songService = songService;
            _mediaPlatformService = mediaPlatformService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                var medias = await Task.Run(() => _mediaPlatformService.GetMediaPlatformsAsync());
                var model = new SongViewModel
                {
                    MediaPlatformLinks = _mapper.Map<List<SongMediaPlatformViewModel>>(medias),
                };
                return View(model);
            }

            var song = await _songService.GetSongAsync(text, false);

            return View(song);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SongViewModel newSong)
        {
            newSong.SearchKey = _songService.BuildSearchKey(newSong);
            var result = await _songService.CreateSongAsync(newSong);
            if (result <= 0) return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Index), new { text = newSong.Name });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

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

        public HomeController(ILogger<HomeController> logger, ISongService songService)
        {
            _logger = logger;
            _songService = songService;
        }

        //public async Task<IActionResult> Index(int id)
        //{
        //    if (id == 0) return View();

        //    var song = await _songService.GetSongAsync(id);
        //    return View(song);
        //}

        public async Task<IActionResult> Index(string text)
        {
            if(string.IsNullOrEmpty(text)) return View();

            var song = await _songService.GetSongAsync(text);
            return View(song);
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

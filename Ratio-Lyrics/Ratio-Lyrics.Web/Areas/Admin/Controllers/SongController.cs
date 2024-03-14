using Microsoft.AspNetCore.Mvc;

namespace Ratio_Lyrics.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SongController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

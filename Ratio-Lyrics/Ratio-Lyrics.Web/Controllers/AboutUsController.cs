using Microsoft.AspNetCore.Mvc;

namespace Ratio_Lyrics.Web.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ratio_Lyrics.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager,SuperAdmin,ContentEditor, Employee")]    
    public class HomeController : Controller
    {
        [Route("/admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

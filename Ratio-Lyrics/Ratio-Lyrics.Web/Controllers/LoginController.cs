using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ratio_Lyrics.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult ExternalLoginFailure(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}

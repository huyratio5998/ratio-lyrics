using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ratio_Lyrics.Web.Constants;
using Ratio_Lyrics.Web.Services.Abstraction;

namespace Ratio_Lyrics.Api.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;

        public AccountController(ILogger<AccountController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }       

        [AllowAnonymous]
        [HttpPost]
        [Route("externalLogin")]
        public IActionResult ExternalLogin([FromForm] string provider = CommonConstant.GoogleProvider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            return _userService.ExternalLogin(provider, redirectUrl);
        }

        [AllowAnonymous]
        [Route("externalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var result = await _userService.ExternalLoginCallback();            

            if (result.Status.Equals("Success")) return LocalRedirect(returnUrl);
            return RedirectToAction("ExternalLoginFailure", "Login", new { returnUrl });
        }

        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            var logoutResponse = await _userService.UserLogout();
            //if (logoutResponse) Response.Cookies.Delete(CookieKeys.CartId);

            return Ok(logoutResponse);
        }

        [Route("authenticated")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAuthenticated()
        {
            try
            {
                var result = true;
                if (User == null || !User.Identity.IsAuthenticated) result = false;

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }
    }
}

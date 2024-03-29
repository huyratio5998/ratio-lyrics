using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ratio_Lyrics.Web.Constants;
using Ratio_Lyrics.Web.Services.Abstraction;

namespace Ratio_Lyrics.Web.Features.Authentication
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAuthenticationService _authenticationService;

        public AccountController(ILogger<AccountController> logger, IAuthenticationService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("externalLogin")]
        public IActionResult ExternalLogin([FromForm] string provider = CommonConstant.GoogleProvider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            return _authenticationService.ExternalLogin(provider, redirectUrl);
        }

        [AllowAnonymous]
        [Route("externalLoginCallback")]
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var result = await _authenticationService.ExternalLoginCallback();

            if (result.Status.Equals("Success")) return LocalRedirect(returnUrl);
            return RedirectToAction("ExternalLoginFailure", "Login", new { returnUrl });
        }

        [Authorize]
        [Route("logout")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var logoutResponse = await _authenticationService.UserLogout();
            //if (logoutResponse) Response.Cookies.Delete(CookieKeys.CartId);

            return Ok(logoutResponse);
        }

        [Route("authenticated")]
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Models.Enums;
using Ratio_Lyrics.Web.Services.Abstraction;
using System.Security.Claims;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<RatioLyricUsers> _signInManager;
        private readonly UserManager<RatioLyricUsers> _userManager;
        private readonly ILogger _logger;
        private readonly IUserService _userService;

        public AuthenticationService(SignInManager<RatioLyricUsers> signInManager
            , UserManager<RatioLyricUsers> userManager, ILogger logger, IUserService userService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _userService = userService;
        }

        public ChallengeResult ExternalLogin(string provider, string redirectUrl)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<LoginResponseViewModel> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return new LoginResponseViewModel { Status = "Failure" };
            }

            // check can login yet?
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return new LoginResponseViewModel { Status = "Success", UserName = info.Principal.Identity.Name };
            }

            if (signInResult.IsLockedOut)
            {
                return new LoginResponseViewModel { Status = "Failure", Message = "Account being locked!" };
            }
            else
            {
                var user = new RatioLyricUsers()
                {
                    DisplayName = info.Principal.FindFirstValue(ClaimTypes.Name),
                    IsClientUser = true,
                };
                string phoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone);

                var result = await _userService.CreateExternalUser(user, info);
                if (result.Succeeded)
                {
                    await _userService.UpdatePhoneNumber(user, phoneNumber);
                    await _userManager.AddToRoleAsync(user, UserRole.Client.ToString());
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: true, info.LoginProvider);
                        return new LoginResponseViewModel { Status = "Success", UserName = user.UserName };
                    }
                }
            }

            return new LoginResponseViewModel { Status = "Failure", Message = $"External authentication by {info.LoginProvider} failure!" };
        }

        public async Task<bool> UserLogout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception logout error: {ex}");
                return false;
            }
        }
    }
}

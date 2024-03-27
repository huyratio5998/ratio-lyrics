using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Services.Abstraction;
using System.Security.Claims;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<RatioLyricUsers> _signInManager;
        private readonly UserManager<RatioLyricUsers> _userManager;
        private readonly IUserStore<RatioLyricUsers> _userStore;
        private readonly IUserEmailStore<RatioLyricUsers> _emailStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;                        
        private readonly IUserService _userService;

        public AuthenticationService(SignInManager<RatioLyricUsers> signInManager
            , UserManager<RatioLyricUsers> userManager, IUserStore<RatioLyricUsers> userStore
            , IUserEmailStore<RatioLyricUsers> emailStore, RoleManager<IdentityRole> roleManager
            , ILogger logger, IUserService userService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _roleManager = roleManager;
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
                    DisplayName = info.Principal.FindFirstValue(ClaimTypes.Name)
                };
                string email = info.Principal.FindFirstValue(ClaimTypes.Email);
                string phoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone);

                // check username exist
                if (await _userRepository.GetAll().AsQueryable().FirstOrDefaultAsync(x => x.UserName.Equals(email)) != null)
                {
                    email = $"{email}.{info.LoginProvider}".ToLower();
                }

                await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await _userService.UpdatePhoneNumber(user, phoneNumber);
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

        

        private IUserEmailStore<RatioLyricUsers> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<RatioLyricUsers>)_userStore;
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Repositories.Abstracts;
using Ratio_Lyrics.Web.Services.Abstraction;
using Ratio_Lyrics.Web.Services.Abstractions;
using System.Security.Claims;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly UserManager<RatioLyricUsers> _userManager;
        private readonly SignInManager<RatioLyricUsers> _signInManager;
        private readonly IUserStore<RatioLyricUsers> _userStore;
        private readonly IUserEmailStore<RatioLyricUsers> _emailStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;
        private readonly IUserRepository _userRepository;

        public UserService(SignInManager<RatioLyricUsers> signInManager,
            UserManager<RatioLyricUsers> userManager,
            IUserStore<RatioLyricUsers> userStore, 
            IUserEmailStore<RatioLyricUsers> emailStore,
            RoleManager<IdentityRole> roleManager, 
            ILogger logger, IMapper mapper, 
            IUnitOfWork unitOfWork, 
            ICommonService commonService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = emailStore;
            _roleManager = roleManager;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _commonService = commonService;
            _userRepository = unitOfWork.UserRepository;
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
                    await UpdatePhoneNumber(user, phoneNumber);
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

        private async Task<bool> UpdatePhoneNumber(RatioLyricUsers user, string newPhoneNumber)
        {
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (newPhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, newPhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return false;
                }
            }
            return true;
        }

        //admin page
        public async Task<RegisterResponseViewModel> RegisterAdminUser(RegisterRequestViewModel newUser)
        {
            if (newUser == null || string.IsNullOrWhiteSpace(newUser.UserName) || string.IsNullOrEmpty(newUser.Password)) return new RegisterResponseViewModel { Status = "Failure" };

            var user = new RatioLyricUsers
            {
                DisplayName = newUser.DisplayName,
                PasswordHash = _commonService.HashPasword(newUser.Password, out var salt),
                HashSalt = Convert.ToHexString(salt)
            };

            await _userStore.SetUserNameAsync(user, newUser.UserName, CancellationToken.None);            
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await UpdatePhoneNumber(user, newUser.PhoneNumber);
                await _userManager.AddToRoleAsync(user, Constants.CommonConstant.Admin);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return new RegisterResponseViewModel
                {
                    UserName = user.UserName,
                    Status = "Success"
                };
            }

            return new RegisterResponseViewModel { Status = "Failure" };
        }

        public async Task<LoginResponseViewModel> AdminUserLogin(LoginRequestViewModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password)) return new LoginResponseViewModel { Status = "Failure" };

            var userInfo = await _userRepository.GetAll().AsQueryable().FirstOrDefaultAsync(x => x.UserName.Equals(request.UserName));
            if (userInfo == null || string.IsNullOrEmpty(userInfo.PasswordHash) || string.IsNullOrEmpty(userInfo.HashSalt)) return new LoginResponseViewModel { Status = "Failure" };

            var result = _commonService.VerifyPassword(request.Password, userInfo.PasswordHash, Convert.FromHexString(userInfo.HashSalt));
            if (!result) return new LoginResponseViewModel { Status = "Failure" };

            await _signInManager.SignInAsync(userInfo, isPersistent: false);
            return new LoginResponseViewModel
            {
                UserName = request.UserName,
                Status = "Success"
            };
        }
    }
}

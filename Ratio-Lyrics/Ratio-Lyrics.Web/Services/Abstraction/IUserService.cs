using Microsoft.AspNetCore.Mvc;
using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Services.Abstraction
{
    public interface IUserService
    {
        ChallengeResult ExternalLogin(string provider, string redirectUrl);
        Task<LoginResponseViewModel> ExternalLoginCallback();
        Task<UserViewModel?> GetUserById(string id);
        Task<bool> UserLogout();
    }
}

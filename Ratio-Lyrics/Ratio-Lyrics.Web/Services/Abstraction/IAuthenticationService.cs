using Microsoft.AspNetCore.Mvc;
using Ratio_Lyrics.Web.Models;
using System.Security.Claims;

namespace Ratio_Lyrics.Web.Services.Abstraction
{
    public interface IAuthenticationService
    {
        ChallengeResult ExternalLogin(string provider, string redirectUrl);
        Task<LoginResponseViewModel> ExternalLoginCallback();        
        Task<bool> UserLogout();
        Task<List<string>?> GetCurrentUserRoles(ClaimsPrincipal currentUser);
    }
}

using Microsoft.AspNetCore.Identity;
using Ratio_Lyrics.Web.Areas.Admin.Models;
using Ratio_Lyrics.Web.Areas.Admin.Models.User;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Services.Abstraction
{
    public interface IUserService
    {
        Task<ListUsersViewModel<UserViewModel>> Gets(BaseSearchArgs args);
        Task<UserViewModel?> Get(string id);
        Task<RegisterResponseViewModel> CreateEmployee(UserViewModel newUser);
        Task<IdentityResult> CreateExternalUser(RatioLyricUsers user, ExternalLoginInfo? info);
        Task<bool> UpdateEmployee(UserViewModel userModel);
        Task<bool> UpdatePhoneNumber(RatioLyricUsers user, string newPhoneNumber);
        Task<bool> Delete(string id);

        Task<List<string>>? GetUserRoles(RatioLyricUsers user);
        Task<List<IdentityRole>> GetRoles();
        Task<LoginResponseViewModel> AdminUserLogin(LoginRequestViewModel request);


    }
}

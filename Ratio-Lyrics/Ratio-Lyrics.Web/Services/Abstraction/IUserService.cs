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
        Task<string> Create(UserViewModel user);
        Task<bool> Update(UserViewModel user);
        Task<bool> Delete(string id);

        //
        Task<int> CreateExternalUser(RatioLyricUsers user, ExternalLoginInfo? info);
        Task<bool> CheckUserNameExist(string userName);
        Task<bool> UpdatePhoneNumber(RatioLyricUsers user, string newPhoneNumber);        
        Task<List<IdentityRole>> GetRoles();
        Task<List<string>>? GetUserRoles(RatioLyricUsers user);                                
        Task<ListUsersViewModel<UserViewModel>> GetListEmployees(BaseSearchRequest args);                
        Task<bool> CreateEmployee(UserViewModel user);
        Task<bool> UpdateEmployee(UserViewModel user);
        Task<RegisterResponseViewModel> RegisterAdminUser(UserViewModel newUser);
        Task<LoginResponseViewModel> AdminUserLogin(LoginRequestViewModel request);        
    }
}

using Microsoft.AspNetCore.Identity;
using Ratio_Lyrics.Web.Entities;

namespace Ratio_Lyrics.Web.Helpers
{
    public static class ApplicationExtensions
    {
        public static async Task<WebApplication> CreateRolesAsync(this WebApplication app, IConfiguration configuration)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));
            var roles = configuration.GetSection("Roles").Get<List<string>>();

            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // create user
            var ratioUser = new RatioLyricUsers()
            {
                UserName = configuration["RatioSettings:UserName"],
                Email = configuration["RatioSettings:UserEmail"],
            };
            string ratioPWD = configuration["RatioSettings:UserPassword"];

            var userManager = (UserManager<RatioLyricUsers>)scope.ServiceProvider.GetService(typeof(UserManager<RatioLyricUsers>));
            await CreateUser(userManager, ratioUser, ratioPWD, "SuperAdmin");

            return app;
        }
        private static async Task CreateUser(UserManager<RatioLyricUsers>? userManager, RatioLyricUsers user, string userPWD, string role)
        {
            if (userManager == null) return;

            var _user = await userManager.FindByEmailAsync(user.Email);
            if (_user == null)
            {
                var createPowerUser = await userManager.CreateAsync(user, userPWD);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}

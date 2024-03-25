using Microsoft.AspNetCore.Identity;

namespace Ratio_Lyrics.Web.Entities
{
    public class RatioLyricUsers : IdentityUser
    {
        public string? DisplayName { get; set; }
        public string? HashSalt { get; set; }
    }
}

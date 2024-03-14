using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ratio_Lyrics.Web.Data
{
    public class RatioLyricsDBContext : IdentityDbContext
    {
        public RatioLyricsDBContext(DbContextOptions<RatioLyricsDBContext> options)
            : base(options)
        {
        }
    }
}

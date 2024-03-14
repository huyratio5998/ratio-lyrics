using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Entities;

namespace Ratio_Lyrics.Web.Data
{
    public class RatioLyricsDBContext : IdentityDbContext
    {
        public RatioLyricsDBContext(DbContextOptions<RatioLyricsDBContext> options)
            : base(options)
        {
        }

        public DbSet<Song> Songs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<MediaPlatform> MediaPlatforms { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }
        public DbSet<SongLyric> SongLyrics { get; set; }
        public DbSet<SongMediaPlatform> SongMediaPlatforms { get; set; }
        public DbSet<SongArtist> SongArtists { get; set; }
    }
}

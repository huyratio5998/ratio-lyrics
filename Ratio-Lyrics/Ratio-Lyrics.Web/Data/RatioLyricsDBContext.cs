using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Constants;
using Ratio_Lyrics.Web.Entities;

namespace Ratio_Lyrics.Web.Data
{
    public class RatioLyricsDBContext : IdentityDbContext
    {
        public RatioLyricsDBContext(DbContextOptions<RatioLyricsDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SongArtist>().HasKey(x => new { x.SongId, x.ArtistId });
            builder.Entity<SongMediaPlatform>().HasKey(x => new { x.SongId, x.MediaPlatformId });

            builder.Entity<MediaPlatform>().HasData(
                new MediaPlatform { Id = 1, Name = CommonConstant.Spotify, Image = "/images/logos/spotify.png" },
                new MediaPlatform { Id = 2, Name = CommonConstant.Youtube, Image = "/images/logos/youtube.png" },
                new MediaPlatform { Id = 3, Name = CommonConstant.AppleMusic, Image = "/images/logos/apple-music.png" });

            base.OnModelCreating(builder);
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

namespace Ratio_Lyrics.Web.Entities
{
    public class Song : BaseEntity
    {                        
        public string Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Background { get; set; }
        public DateTime ReleaseDate { get; set; }                
        public string? SearchKey { get; set; }

        public SongLyric Lyric { get; set; }
        public ICollection<SongMediaPlatform> MediaPlatformLinks { get; set; }
        public ICollection<SongArtist> SongArtists { get; set; }        
    }
}

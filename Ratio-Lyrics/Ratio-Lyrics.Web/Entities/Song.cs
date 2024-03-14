namespace Ratio_Lyrics.Web.Entities
{
    public class Song : BaseEntity
    {
        //SEO
        public string? Thumbnail { get; set; }
        public string Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string Author { get; set; }
        public string? MediaPlatformLinks { get; set; }
        public string? Image { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Country { get; set; }
        public string? Tags { get; set; }

        public ICollection<SongLyric> Lyrics { get; set; }
    }
}

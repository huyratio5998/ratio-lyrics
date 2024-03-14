namespace Ratio_Lyrics.Web.Entities
{
    public class MediaPlatform : BaseEntity
    {
        public string Name { get; set; }
        public string? Image { get; set; }

        public ICollection<SongMediaPlatform> songMediaPlatforms { get; set; }
    }
}

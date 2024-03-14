namespace Ratio_Lyrics.Web.Entities
{
    public class SongMediaPlatform : BaseEntity
    {
        public string Link { get; set; }
        public int SongId { get; set; }
        public int MediaPlatformId { get; set; }

        public Song Song { get; set; }
        public MediaPlatform MediaPlatform { get; set; }
    }
}

namespace Ratio_Lyrics.Web.Entities
{
    public class SongArtist : BaseEntity
    {
        public int SongId { get; set; }
        public Song Song { get; set; }
        public int ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}

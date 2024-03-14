namespace Ratio_Lyrics.Web.Entities
{
    public class SongLyric : BaseEntity
    {
        public string Lyric { get; set; }
        public decimal Views { get; set; }
        public int SongId { get; set; }
        public Song Songs { get; set; }
    }
}

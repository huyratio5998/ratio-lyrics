namespace Ratio_Lyrics.Web.Entities
{
    public class SongLyric : BaseEntity
    {
        public int SongId { get; set; }
        public Song Songs { get; set; }        
    }
}

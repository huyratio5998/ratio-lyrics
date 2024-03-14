namespace Ratio_Lyrics.Web.Entities
{
    public class Artist : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public ArtistRole Role { get; set; }

        public ICollection<SongArtist> SongArtists { get; set; }
    }

    public enum ArtistRole
    {
        Singer = 0,
        Producer = 1,
        Author = 2,
        Actor = 3
    }
}

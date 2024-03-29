using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Ratio_Lyrics.Web.Models
{
    public class SongViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        //Main properties
        public string Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public string? Background { get; set; }
        [DataType(DataType.Date)]        
        public DateTime ReleaseDate { get; set; } =  DateTime.Now;
        public string? ReleaseDateDisplay { get; set; }
        public string? SearchKey { get; set; }

        //Form create
        public string? YoutubeLink { get; set; }
        public string? SpotifyLink { get; set; }
        public string? AppleMusicLink { get; set; }
        public string? MediaLinksForm { get; set; }
        public string? ArtistForm { get; set; }
        
        //Lyrics
        public string Lyric { get; set; }
        public decimal Views { get; set; }
        public string? ContributedBy { get; set; }
        public List<SongMediaPlatformViewModel>? MediaPlatformLinks { get; set; }
        public List<ArtistViewModel>? Artists { get; set; }
    }

    public class SongViewModelValidator : AbstractValidator<SongViewModel>
    {
        public SongViewModelValidator()
        {            
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Lyric).NotNull().NotEmpty();            
        }
    }
}

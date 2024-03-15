using FluentValidation;

namespace Ratio_Lyrics.Web.Models
{
    public class SongMediaPlatformViewModel
    {
        public int MediaPlatformId { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public string? Link { get; set; }
    }
    public class SongMediaPlatformViewModelValidator : AbstractValidator<SongMediaPlatformViewModel>
    {
        public SongMediaPlatformViewModelValidator()
        {                        
        }
    }
}

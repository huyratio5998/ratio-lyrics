using FluentValidation;

namespace Ratio_Lyrics.Web.Models
{
    public class MediaPlatformViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public class MediaPlatformViewModelValidator : AbstractValidator<MediaPlatformViewModel>
    {
        public MediaPlatformViewModelValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}
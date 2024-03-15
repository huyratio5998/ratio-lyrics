using FluentValidation;
using Ratio_Lyrics.Web.Entities;

namespace Ratio_Lyrics.Web.Models
{
    public class ArtistViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ArtistRole Role { get; set; }
    }
    public class ArtistViewModelValidator : AbstractValidator<ArtistViewModel>
    {
        public ArtistViewModelValidator()
        {                       
            RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}

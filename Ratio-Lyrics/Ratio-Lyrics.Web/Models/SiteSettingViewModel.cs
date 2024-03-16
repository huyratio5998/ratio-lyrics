using FluentValidation;
using Ratio_Lyrics.Web.Entities;

namespace Ratio_Lyrics.Web.Models
{
    public class SiteSettingViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Value { get; set; }
        public SettingType SettingType { get; set; }
        public bool IsActive { get; set; }
    }

    public class SiteSettingViewModelValidator : AbstractValidator<SiteSettingViewModel>
    {
        public SiteSettingViewModelValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}

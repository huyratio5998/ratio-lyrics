using Ratio_Lyrics.Web.Models.Enums;

namespace Ratio_Lyrics.Web.Areas.Admin.Models
{
    public interface IBaseSort
    {
        SortingType SortType { get; set; }
    }
}

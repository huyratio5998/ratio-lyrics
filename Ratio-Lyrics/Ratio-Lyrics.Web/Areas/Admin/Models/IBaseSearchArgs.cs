using Ratio_Lyrics.Web.Models.Enums;

namespace Ratio_Lyrics.Web.Areas.Admin.Models
{
    public interface IBaseSearchArgs
    {
        public string? FilterItems { get; set; }
        public SortingType SortType { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool IsSelectPreviousItems { get; set; }
    }
}

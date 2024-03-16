using Microsoft.AspNetCore.Mvc;

namespace Ratio_Lyrics.Web.Areas.Admin.Models
{
    [BindProperties]
    public class BaseSearchArgs
    {
        [BindProperty(Name = "filterItems")]
        public string? FilterItems { get; set; }

        [BindProperty(Name = "sortType")]
        public SortingEnum SortType { get; set; }

        [BindProperty(Name = "isSelectPreviousItems")]
        public bool IsSelectPreviousItems { get; set; }

        [BindProperty(Name = "page")]
        public int PageIndex { get; set; }

        [BindProperty(Name = "pageSize")]
        public int PageSize { get; set; }
    }

    public enum SortingEnum
    {
        Default,
        Oldest,
        RecentUpdate,
        Name,
        LowtoHeigh,
        HeightoLow
    }
}

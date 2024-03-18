using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Areas.Admin.Models
{
    public class BaseAdminPagingViewModel
    {
        public string? SearchText { get; set; } = string.Empty;
        public OrderType? OrderBy { get; set; } = OrderType.Asc;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public bool IsSelectPreviousItems { get; set; }
    }

}

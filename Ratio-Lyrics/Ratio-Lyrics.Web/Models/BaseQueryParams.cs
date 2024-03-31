using Microsoft.AspNetCore.Mvc;

namespace Ratio_Lyrics.Web.Models
{
    [BindProperties]
    public class BaseQueryParams
    {
        public string? SearchText { get; set; } = string.Empty;
        public OrderType? OrderBy { get; set; } = OrderType.Desc;
        [BindProperty(Name ="page")]
        public int PageNumber { get; set; }
        public int PageSize { get; set; }        
    }

    public enum OrderType
    {
        Asc = 0,
        Desc = 1,
    }
}

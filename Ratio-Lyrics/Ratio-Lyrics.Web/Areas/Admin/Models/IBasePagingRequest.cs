namespace Ratio_Lyrics.Web.Areas.Admin.Models
{
    public interface IBasePagingRequest
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }        
        bool IsSelectPreviousItems { get; set; }
    }
}

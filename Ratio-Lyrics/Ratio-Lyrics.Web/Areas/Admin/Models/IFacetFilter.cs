namespace Ratio_Lyrics.Web.Areas.Admin.Models
{
    public interface IFacetFilter
    {
        IEnumerable<FacetFilterItem> FilterItems { get; set; }
    }
}

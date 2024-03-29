namespace Ratio_Lyrics.Web.Areas.Admin.Models.User
{
    public class ListUsersViewModel<T> : BaseListingPageViewModel where T : UserViewModel
    {
        public ListUsersViewModel()
        {
            if (Users == null) Users = new List<T>();
            AvailableFilters = new List<FacetFilterItem>
            {
                new FacetFilterItem{FieldName="Name"},
                new FacetFilterItem{FieldName="Email"},
                new FacetFilterItem{FieldName="PhoneNumber"},
            };
        }
        public List<T> Users { get; set; }
        public List<FacetFilterItem> AvailableFilters { get; set; }
    }
}

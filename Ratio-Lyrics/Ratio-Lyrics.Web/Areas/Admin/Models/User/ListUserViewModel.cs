namespace Ratio_Lyrics.Web.Areas.Admin.Models.User
{
    public class ListUsersViewModel<T> : BaseListingPageViewModel where T : UserViewModel
    {
        public ListUsersViewModel()
        {
            if (Users == null) Users = new List<T>();
        }
        public List<T> Users { get; set; }
    }
}

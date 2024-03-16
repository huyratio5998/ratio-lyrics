using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Areas.Admin.Models
{
    public class ListSongsAdminViewModel : BaseAdminPagingViewModel
    {
        public List<SongViewModel>? Items { get; set; }
    }
}

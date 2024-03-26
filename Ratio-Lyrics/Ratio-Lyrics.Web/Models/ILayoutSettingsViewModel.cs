using Ratio_Lyrics.Web.Entities;

namespace Ratio_Lyrics.Web.Models
{
    public interface ILayoutSettingsViewModel
    {
        public string PublicApiUrl { get; }        
        public string CurrentPath();        
    }
}

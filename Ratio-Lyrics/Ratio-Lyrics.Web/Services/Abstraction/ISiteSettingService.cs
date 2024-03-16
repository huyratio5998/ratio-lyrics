using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Services.Abstraction
{
    public interface ISiteSettingService
    {
        Task<SiteSettingViewModel?> Get(int id, bool isTracking = true);
        Task<List<SiteSettingViewModel>> Gets();        
        Task<int> Create(SiteSettingViewModel setting);
        Task<bool> Delete(int id);
        Task<bool> Update(SiteSettingViewModel setting);
    }
}

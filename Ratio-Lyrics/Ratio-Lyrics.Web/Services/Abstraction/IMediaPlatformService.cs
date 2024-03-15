using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Services.Abstraction
{
    public interface IMediaPlatformService
    {
        Task<MediaPlatformViewModel?> GetMediaPlatform(int mediaPlatformId, bool isTracking = true);
        Task<MediaPlatformViewModel?> GetMediaPlatform(string name, bool isTracking = true);
        List<MediaPlatformViewModel> GetMediaPlatformsAsync();
        Task<int> CreateMediaPlatformAsync(MediaPlatformViewModel newMediaPlatform);
        Task<bool> DeleteMediaPlatformAsync(int mediaPlatformId);
        Task<bool> UpdateMediaPlatformAsync(MediaPlatformViewModel newMediaPlatform);
    }
}

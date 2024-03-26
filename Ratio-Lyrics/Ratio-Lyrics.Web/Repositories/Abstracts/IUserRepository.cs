using Ratio_Lyrics.Web.Entities;

namespace Ratio_Lyrics.Web.Repositories.Abstracts
{
    public interface IUserRepository
    {
        IEnumerable<RatioLyricUsers> GetAll(bool isTracking = false);
        Task<RatioLyricUsers?> GetShopUser(string id);
    }
}

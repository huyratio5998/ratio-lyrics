using Ratio_Lyrics.Web.Entities;

namespace Ratio_Lyrics.Web.Repositories.Abstracts
{
    public interface IUserRepository
    {
        IEnumerable<RatioLyricUsers> GetAll(bool isTracking = false);        
        Task<RatioLyricUsers?> Get(string id, bool isTracking = true);
        Task<RatioLyricUsers> CreateAsync(RatioLyricUsers entity);
        bool Update(RatioLyricUsers entity);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteAsync(RatioLyricUsers? entity);
        Task<bool> DeleteRangeAsync(List<RatioLyricUsers> entities);
    }
}

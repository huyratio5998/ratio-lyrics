using Ratio_Lyrics.Web.Entities;

namespace Ratio_Lyrics.Web
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll(bool isTracking = false);
        Task<T?> GetByIdAsync(int id, bool isTracking = true);
        Task<T> CreateAsync(T entity);
        bool Update(T entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAsync(T? entity);
        Task<bool> DeleteRangeAsync(List<T> entities);
    }
}
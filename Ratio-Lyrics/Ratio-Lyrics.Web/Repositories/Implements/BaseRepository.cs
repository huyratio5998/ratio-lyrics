using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Data;
using Ratio_Lyrics.Web.Entities;

namespace Ratio_Lyrics.Web.Repositories.Implements
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly RatioLyricsDBContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(RatioLyricsDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);            
            return await DeleteAsync(entity);
        }

        public async Task<bool> DeleteAsync(T? entity)
        {
            if (entity == null) return false;

            _dbSet.Remove(entity);

            return true;
        }

        public async Task<bool> DeleteRangeAsync(List<T> entities)
        {
            if(entities == null) return true;            

            _dbSet.RemoveRange(entities);

            return true;
        }                      

        public IEnumerable<T> GetAll(bool isTracking = false)
        {
            return isTracking ? _dbSet : _dbSet.AsNoTracking();
        }

        public async Task<T?> GetByIdAsync(int id, bool isTracking = true)
        {
            if (id == 0) return null!;

            var entity = isTracking ? await _dbSet.FirstOrDefaultAsync(s => s.Id == id)
                : await _dbSet.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null) return null!;

            return entity;
        }        

        public bool Update(T entity)
        {
            if (entity == null) return false;

            entity.ModifiedDate = DateTime.UtcNow;
            _dbSet.Update(entity);
            return true;
        }
    }
}

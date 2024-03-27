using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Data;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Repositories.Abstracts;

namespace Ratio_Lyrics.Web.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly RatioLyricsDBContext _context;
        private readonly DbSet<RatioLyricUsers> _dbSet;

        public UserRepository(RatioLyricsDBContext context)
        {
            _context = context;
            _dbSet = context.Set<RatioLyricUsers>();
        }
        public IEnumerable<RatioLyricUsers> GetAll(bool isTracking = false)
        {
            return isTracking ? _dbSet : _dbSet.AsNoTracking();
        }        

        public async Task<RatioLyricUsers> CreateAsync(RatioLyricUsers entity)
        {            
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await Get(id);
            return await DeleteAsync(entity);
        }

        public async Task<bool> DeleteAsync(RatioLyricUsers? entity)
        {
            if (entity == null) return false;

            _dbSet.Remove(entity);

            return true;
        }

        public async Task<bool> DeleteRangeAsync(List<RatioLyricUsers> entities)
        {
            if (entities == null) return true;

            _dbSet.RemoveRange(entities);

            return true;
        }        

        public async Task<RatioLyricUsers?> Get(string id, bool isTracking = true)
        {
            if (string.IsNullOrEmpty(id)) return null!;

            var entity = isTracking ? await _dbSet.FirstOrDefaultAsync(s => s.Id.Equals(id))
                : await _dbSet.AsNoTracking().FirstOrDefaultAsync(s => s.Id.Equals(id));
            if (entity == null) return null!;

            return entity;
        }

        public bool Update(RatioLyricUsers entity)
        {
            if (entity == null) return false;
            
            _dbSet.Update(entity);
            return true;
        }
    }
}

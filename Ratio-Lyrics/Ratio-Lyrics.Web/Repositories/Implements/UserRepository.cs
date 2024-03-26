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
        public async Task<RatioLyricUsers?> GetShopUser(string id)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(id));
        }
    }
}

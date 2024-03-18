using Microsoft.EntityFrameworkCore.Storage;
using Ratio_Lyrics.Web.Data;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Repositories.Abstracts;

namespace Ratio_Lyrics.Web.Repositories.Implements
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly RatioLyricsDBContext _context;
        private Dictionary<Type, object> _repositories;
        private IDbContextTransaction _transaction;
        private bool disposedValue = false;

        public UnitOfWork(RatioLyricsDBContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public async Task CreateTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync(CancellationToken token)
        {
            await _context.SaveChangesAsync(token);
        }

        public async Task CommitAsync()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null!;
            }
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }

        public IBaseRepository<T> GetRepository<T>() where T : BaseEntity
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return _repositories[typeof(T)] as BaseRepository<T>;
            }

            var repository = new BaseRepository<T>(_context);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

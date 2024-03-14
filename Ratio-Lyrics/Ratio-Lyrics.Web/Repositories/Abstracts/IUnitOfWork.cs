using Ratio_Lyrics.Web.Entities;

namespace Ratio_Lyrics.Web.Repositories.Abstracts
{
    public interface IUnitOfWork
    {
        Task CreateTransactionAsync();
        Task SaveAsync();
        Task CommitAsync();
        Task RollbackAsync();

        IBaseRepository<T> GetRepository<T>() where T : BaseEntity;
    }
}

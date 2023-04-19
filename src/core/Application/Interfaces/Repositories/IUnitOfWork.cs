using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryAsync<T> Repository<T>() where T : class;
        void CreateTransaction();
        Task CreateTransactionAsync();
        void Commit();
        Task CommitAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        void SaveChanges();
        void Rollback();
        Task RollbackAsync();
    }
}

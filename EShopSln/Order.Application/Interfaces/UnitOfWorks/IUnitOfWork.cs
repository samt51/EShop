using Order.Application.Interfaces.Repositories;

namespace Order.Application.Interfaces.UnitOfWorks;

public interface IUnitOfWork: IAsyncDisposable
{
    IReadRepository<T> GetReadRepository<T>() where T : class, new();
    IWriteRepository<T> GetWriteRepository<T>() where T : class, new();
    void OpenTransaction();
    Task OpenTransactionAsync(CancellationToken cancellationToken);
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollBackAsync(CancellationToken cancellationToken = default);

}
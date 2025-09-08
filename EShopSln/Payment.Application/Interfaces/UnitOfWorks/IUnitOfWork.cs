using EShop.Shared.Dtos.Common;
using Payment.Application.Interfaces.Repositories;

namespace Payment.Application.Interfaces.UnitOfWorks;

public interface IUnitOfWork: IAsyncDisposable
{
    IReadRepository<T> GetReadRepository<T>() where T : class, IBaseEntity, new();
    IWriteRepository<T> GetWriteRepository<T>() where T : class, IBaseEntity, new();
    Task OpenTransactionAsync(CancellationToken? cancellationToken = null);
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollBackAsync(CancellationToken cancellationToken = default);

}
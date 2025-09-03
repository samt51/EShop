using Catalog.Application.Interfaces.Repositories;
using EShop.Shared.Dtos.Common;

namespace Catalog.Application.Interfaces.UnitOfWorks;

public interface IUnitOfWork: IAsyncDisposable
{
    IReadRepository<T> GetReadRepository<T>() where T : class, IBaseEntity, new();
    IWriteRepository<T> GetWriteRepository<T>() where T : class, IBaseEntity, new();
    void OpenTransaction();
    Task<int> SaveAsync();
    Task CommitAsync();
    void RollBack();

}
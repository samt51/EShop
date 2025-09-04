using Order.Application.Interfaces.Repositories;

namespace Order.Application.Interfaces.UnitOfWorks;

public interface IUnitOfWork: IAsyncDisposable
{
    IReadRepository<T> GetReadRepository<T>() where T : class, new();
    IWriteRepository<T> GetWriteRepository<T>() where T : class, new();
    void OpenTransaction();
    Task<int> SaveAsync();
    Task CommitAsync();
    void RollBack();

}
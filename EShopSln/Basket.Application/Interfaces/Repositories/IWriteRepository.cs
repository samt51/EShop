using EShop.Shared.Dtos.Common;

namespace Basket.Application.Interfaces.Repositories;

public interface IWriteRepository<T> where T : class, IBaseEntity, new()
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IList<T> entities);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
    Task HardDeleteRangeAsync(IList<T> entity);
}
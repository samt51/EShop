using System.Linq.Expressions;
using EShop.Shared.Dtos.Common;
using Microsoft.EntityFrameworkCore.Query;

namespace Basket.Application.Interfaces.Repositories;

public interface IReadRepository<T> where T : class, IBaseEntity, new()
{
    Task<IList<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, T>>? selector = null,
        bool enableTracking = false,
        CancellationToken ct = default);

    Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool enableTracking = false, int currentPage = 1, int pageSize = 3);

    /// <summary>
    /// Exception GetFirst
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="include"></param>
    /// <param name="enableTracking"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    Task<T> GetAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool enableTracking = false);


    /// <summary>
    /// Not Exception Controll
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="include"></param>
    /// <param name="enableTracking"></param>
    /// <returns></returns>
    Task<T> FindAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool enableTracking = false);

    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}
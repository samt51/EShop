using Microsoft.EntityFrameworkCore;
using Order.Application.Interfaces.Repositories;

namespace Order.Infrastructure.Concrete.Repositories;

public class WriteRepository<T> : IWriteRepository<T> where T : class, new()
{
    private readonly DbContext dbContext;

    public WriteRepository(DbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    private DbSet<T> Table { get => dbContext.Set<T>(); }

    public async Task<T> AddAsync(T entity,CancellationToken cancellationToken = default)
    {
        var tab = await Table.AddAsync(entity);
        return tab.Entity;
    }

    

    public async Task AddRangeAsync(IList<T> entities)
    {
        await Table.AddRangeAsync(entities);
    }
    public async Task<T> UpdateAsync(T entity)
    {
        await Task.Run(() => Table.Update(entity));
        return entity;
    }

    public async Task HardDeleteRangeAsync(IList<T> entity)
    {
        await Task.Run(() => Table.RemoveRange(entity));
    }

    public async Task SoftDeleteAsync(T entity)
    {
        await Task.Run(() => Table.Update(entity));
    }

    public async Task<T> DeleteAsync(T entity)
    {
        await Task.Run(() => Table.Update(entity));
        return entity;
    }
}

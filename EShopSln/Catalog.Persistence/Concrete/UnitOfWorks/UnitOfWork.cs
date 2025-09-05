using Catalog.Application.Interfaces.Repositories;
using Catalog.Application.Interfaces.UnitOfWorks;
using Catalog.Persistence.Concrete.Repositories;
using Catalog.Persistence.Context;

namespace Catalog.Persistence.Concrete.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext dbContext;
    public UnitOfWork(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollBackAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.RollbackTransactionAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync() => await dbContext.DisposeAsync();

    public async Task OpenTransactionAsync(CancellationToken? cancellationToken = null)
    {
       await dbContext.Database.BeginTransactionAsync();
    }
    
    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {

        try
        {
            var result = await dbContext.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        { 
            await RollBackAsync(cancellationToken);
            throw new Exception(ex.Message);

        }
    }
    IReadRepository<T> IUnitOfWork.GetReadRepository<T>() => new ReadRepository<T>(dbContext);
    IWriteRepository<T> IUnitOfWork.GetWriteRepository<T>() => new WriteRepository<T>(dbContext);

}

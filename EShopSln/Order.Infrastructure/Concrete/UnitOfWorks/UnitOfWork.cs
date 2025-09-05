using Order.Application.Interfaces.Repositories;
using Order.Application.Interfaces.UnitOfWorks;
using Order.Infrastructure.Concrete.Repositories;
using Order.Infrastructure.Context;

namespace Order.Infrastructure.Concrete.UnitOfWorks;

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

    public async ValueTask DisposeAsync() => await dbContext.DisposeAsync();

    public void OpenTransaction()
    {
        dbContext.Database.BeginTransactionAsync();
    }

    public async Task OpenTransactionAsync(CancellationToken cancellationToken)
    {
        await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public void OpenTransaction(CancellationToken cancellationToken)
    {
        dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task RollBackAsync(CancellationToken cancellationToken = default)
    {
     await dbContext.Database.RollbackTransactionAsync(cancellationToken);
    }

    public int Save() => dbContext.SaveChanges();
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

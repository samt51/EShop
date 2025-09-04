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
    public async Task CommitAsync()
    {
        await dbContext.Database.CommitTransactionAsync();
    }

    public async ValueTask DisposeAsync() => await dbContext.DisposeAsync();

    public void OpenTransaction()
    {
        dbContext.Database.BeginTransactionAsync();
    }
        

    public void RollBack()
    {
        dbContext.Database.RollbackTransaction();
    }

    public int Save() => dbContext.SaveChanges();
    public async Task<int> SaveAsync()
    {

        try
        {
            var result = await dbContext.SaveChangesAsync();
            return result;
        }
        catch (Exception ex)
        {
            RollBack();
            throw new Exception(ex.Message);

        }
    }
    IReadRepository<T> IUnitOfWork.GetReadRepository<T>() => new ReadRepository<T>(dbContext);
    IWriteRepository<T> IUnitOfWork.GetWriteRepository<T>() => new WriteRepository<T>(dbContext);

}

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

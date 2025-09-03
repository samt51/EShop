using Catalog.Application.Interfaces.Mapping;
using Catalog.Application.Interfaces.Repositories;
using Catalog.Application.Interfaces.UnitOfWorks;
using Catalog.Persistence.Concrete.Repositories;
using Catalog.Persistence.Concrete.UnitOfWorks;
using Catalog.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Persistence;

public static class Registration
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

        services.AddSingleton<Catalog.Application.Interfaces.Mapping.IMapper, Mapper>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();


    }
}
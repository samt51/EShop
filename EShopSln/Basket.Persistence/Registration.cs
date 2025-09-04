using Basket.Application.Interfaces.Repositories;
using Basket.Application.Interfaces.UnitOfWorks;
using Basket.Persistence.Concrete.Repositories;
using Basket.Persistence.Concrete.UnitOfWorks;
using Basket.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basket.Persistence;

public static class Registration
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

        services.AddSingleton<Basket.Application.Interfaces.Mapping.IMapper, Mapper>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();


    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Interfaces.Mapping;
using Order.Application.Interfaces.Repositories;
using Order.Application.Interfaces.UnitOfWorks;
using Order.Infrastructure.Concrete.Repositories;
using Order.Infrastructure.Concrete.UnitOfWorks;
using Order.Infrastructure.Context;
namespace Order.Infrastructure;

public static class Registration
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

        services.AddSingleton<IMapper, Mapper>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
    }
}
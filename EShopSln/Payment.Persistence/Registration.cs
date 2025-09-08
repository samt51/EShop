using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Payment.Application.Interfaces.Repositories;
using Payment.Application.Interfaces.UnitOfWorks;
using Payment.Persistence.Concrete.Repositories;
using Payment.Persistence.Concrete.UnitOfWorks;
using Payment.Persistence.Context;

namespace Payment.Persistence;

public static class Registration
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

        services.AddSingleton<Payment.Application.Interfaces.Mapping.IMapper, Mapper>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();


    }
}
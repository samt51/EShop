using Basket.Application.Interfaces.Repositories;
using Basket.Persistence.Concrete.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Basket.Persistence;

public static class Registration
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {

        
      
        services.AddScoped<IBasketRepository, RedisBasketRepository>();


        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var cfg = configuration.GetSection("Redis")["Configuration"];
            return ConnectionMultiplexer.Connect(cfg);
        });
        
        services.AddSingleton<Basket.Application.Interfaces.Mapping.IMapper, Mapper>();

  


    }
}
using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Consumers;
using Payment.Application.Middleware.Exceptions;

namespace Payment.Application;


public static class Registration
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        
        var assembly = Assembly.GetExecutingAssembly();

    

        services.AddTransient<ExceptionMiddleware>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
      

        services.AddMassTransit(x =>
        {
            var rabbitHost = configuration["RabbitMQ:Host"] ?? "localhost";
            var vhost = configuration["RabbitMQ:VirtualHost"] ?? "/";
            var rabbitUser = configuration["RabbitMQ:Username"] ?? "guest";
            var rabbitPass = configuration["RabbitMQ:Password"] ?? "guest";
            x.AddConsumer<RefundPaymentConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(rabbitHost, vhost, h => { h.Username(rabbitUser); h.Password(rabbitPass); });
                
                cfg.ReceiveEndpoint("payment-refund", e =>
                {
                    e.UseInMemoryOutbox();
                    e.PrefetchCount = 16;
                    e.ConcurrentMessageLimit = 8;
                });
            });
        });

        return services;
    }
}
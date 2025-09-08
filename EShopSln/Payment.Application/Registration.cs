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
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQUrl"], "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("payment-refund", e =>
                {
                    e.UseInMemoryOutbox();
                    e.PrefetchCount = 16;
                    e.ConcurrentMessageLimit = 8;
                    e.ConfigureConsumer<RefundPaymentConsumer>(ctx);
                });
            });
        });

        return services;
    }
}
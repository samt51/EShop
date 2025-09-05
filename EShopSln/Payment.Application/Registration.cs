using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Consumers.OrderConsumer;
using Payment.Application.Middleware.Exceptions;
using MediatR;
using Payment.Application.Consumers.InventoryConsumers;

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
            x.AddConsumer<OrderCreatedConsumer>(c => c.ConcurrentMessageLimit = 8);
            x.AddConsumer<RefundPaymentConsumer>(c => c.ConcurrentMessageLimit = 8);
            x.AddConsumer<InventoryReservedConsumer>();
            x.AddConsumer<RefundPaymentConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQUrl"], "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // Global dayanıklılık politikası
                cfg.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));
                cfg.UseInMemoryOutbox(); // EF Outbox ekleyene kadar faydalı

                // OrderCreated => event (Payment dinler)
                cfg.ReceiveEndpoint("payment-order-created", e =>
                {
                    e.ConfigureConsumer<OrderCreatedConsumer>(ctx);
                    e.PrefetchCount = 16;
                });

                // RefundPayment => komut (Payment dinler)
                cfg.ReceiveEndpoint("payment-refund", e =>
                {
                    e.ConfigureConsumer<RefundPaymentConsumer>(ctx);
                    e.PrefetchCount = 16;
                });
            });
        });

        services.AddSingleton<Payment.Application.Interfaces.Mapping.IMapper, Mapper>();
        return services;
    }
}
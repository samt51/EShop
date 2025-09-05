
using EShop.Shared.Messages.Commands.Payments;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Consumers;


namespace Order.Application;

 public static class Registration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddMassTransit(x =>
            {
                x.AddConsumer<PaymentAuthorizedConsumer>(c => c.ConcurrentMessageLimit = 8);


                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQUrl"], "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));
                    cfg.UseInMemoryOutbox(); // EF Outbox yoksa iyi bir güvenlik

                 
                    cfg.ReceiveEndpoint("order-payment-authorized", e =>
                    {
                        e.ConfigureConsumer<PaymentAuthorizedConsumer>(ctx);
                        e.PrefetchCount = 16;
                    });
                });

                EndpointConvention.Map<RefundPaymentCommand>(new Uri("queue:payment-refund"));
            });

            return services;
        }
    }

using System.Reflection;
using EShop.Shared.Messages.Commands.Payments;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Consumers;
using Order.Application.Middleware.Exceptions;


namespace Order.Application;

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
                x.AddConsumer<PaymentAuthorizedConsumer>(c => c.ConcurrentMessageLimit = 8);

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(rabbitHost, vhost, h => { h.Username(rabbitUser); h.Password(rabbitPass); });
                    cfg.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));
                    cfg.UseInMemoryOutbox(); 
                    cfg.ReceiveEndpoint("order-payment-authorized", e =>
                    {
                        e.PrefetchCount = 16;        
                        e.ConcurrentMessageLimit = 8;  
                        e.UseInMemoryOutbox();
                        e.ConfigureConsumer<PaymentAuthorizedConsumer>(ctx);
                    });
                });

                EndpointConvention.Map<RefundPaymentCommand>(new Uri("queue:payment-refund"));
            });

            return services;
        }
    }
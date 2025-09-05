using System.Reflection;
using Basket.Application.Middleware.Exceptions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Consumers;

namespace Basket.Application;

 public static class Registration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddTransient<ExceptionMiddleware>();
            
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderCreatedConsumer>();


                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQUrl"], "/", h => { h.Username("guest"); h.Password("guest"); });
                    cfg.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));
                    cfg.ConfigureEndpoints(ctx);
                });
            });
            
            return services;

        }
        
    }

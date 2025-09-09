using System.Reflection;
using Catalog.Application.Middleware.Exceptions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

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
                var rabbitUser = configuration["RabbitMQ:Username"] ?? "eshop";
                var rabbitPass = configuration["RabbitMQ:Password"] ?? "eshop";
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    var mq = configuration.GetSection("RabbitMq");
                    cfg.Host(rabbitHost, "/", h => { h.Username(rabbitUser); h.Password(rabbitPass); });
                    cfg.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));
                    cfg.UseInMemoryOutbox(); 

                    cfg.ConfigureEndpoints(ctx); 
                });
            });
            
            return services;

        }
        
    }

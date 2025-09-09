using System.Reflection;
using Basket.Application.Middleware.Exceptions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


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
                var rabbitHost = configuration["RabbitMQ:Host"] ?? "localhost";
                var vhost = configuration["RabbitMQ:VirtualHost"] ?? "/";
                var rabbitUser = configuration["RabbitMQ:Username"] ?? "eshop";
                var rabbitPass = configuration["RabbitMQ:Password"] ?? "eshop";
                
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(rabbitHost, vhost, h => { h.Username(rabbitUser); h.Password(rabbitPass); });
                    cfg.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));
                    cfg.ConfigureEndpoints(ctx);
                    
                });
            });
            
            services.AddMassTransitHostedService(true);
            services.Configure<MassTransitHostOptions>(o =>
            {
                o.WaitUntilStarted = true;
                o.StartTimeout     = TimeSpan.FromSeconds(30);
                o.StopTimeout      = TimeSpan.FromSeconds(30);
            });

            
            return services;

        }
        
    }

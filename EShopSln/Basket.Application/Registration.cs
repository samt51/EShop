using System.Reflection;
using Basket.Application.Middleware.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Basket.Application;

 public static class Registration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddTransient<ExceptionMiddleware>();
            
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            
            return services;

        }
        
    }

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Middleware.Exceptions;

namespace Order.Application;

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

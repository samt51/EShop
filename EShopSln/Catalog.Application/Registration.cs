using System.Reflection;
using Catalog.Application.Middleware.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

 public static class Registration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddTransient<ExceptionMiddleware>();
            
            return services;

        }
    }

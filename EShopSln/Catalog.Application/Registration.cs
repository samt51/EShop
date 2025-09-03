using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

 public static class Registration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            //services.AddTransient<ExceptionMiddleware>();


            //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            //services.AddRulesFromAssemblyContaining(assembly, typeof(BaseRules));




         






            return services;

        }
        private static IServiceCollection AddRulesFromAssemblyContaining(
          this IServiceCollection services,
          Assembly assembly,
          Type type)
        {
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
            foreach (var item in types)
                services.AddTransient(item);

            return services;
        }

    }

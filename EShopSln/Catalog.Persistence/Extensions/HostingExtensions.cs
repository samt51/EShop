using Catalog.Domain.Entities;
using Catalog.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Catalog.Persistence.Extensions;

 public static class HostingExtensions
    {
        public static async Task MigrateDevAndSeedAsync<TContext>(
            this IHost host,
            Func<TContext, IServiceProvider, Task>? devSeed = null)
            where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var sp  = scope.ServiceProvider;
            var env = sp.GetRequiredService<IHostEnvironment>();
            if (!env.IsDevelopment()) return;

            var db     = sp.GetRequiredService<TContext>();
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("EF.Migration");

            await db.Database.MigrateAsync();
            if (devSeed is not null) await devSeed(db, sp);
            logger.LogInformation("Development migrate & seed completed.");
        }
        
        public static class DevSeeder
        {
            public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
            {
                if (!await db.Category.AnyAsync(ct))
                {
                    await db.Category.AddRangeAsync(
                        new Category(1,"Pantolon"),
                        new Category(2,"T-Shirt"));
                    await db.SaveChangesAsync(ct);
                }
                
                if (!await db.Product.AnyAsync(ct))
                {
                    await db.Product.AddRangeAsync(
                        new Product { Id = 1, Name = "Kot", Price = 12.50m, Stock = 100, CategoryId = 1 },
                        new Product { Id = 2, Name = "Baskılı T-Shirt",  Price = 14.90m, Stock =  80, CategoryId = 2 });
                    await db.SaveChangesAsync(ct);
                }
            }
        }
        
    }

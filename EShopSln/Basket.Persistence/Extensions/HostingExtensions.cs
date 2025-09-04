using Basket.Domain.Entities;
using Basket.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Basket.Persistence.Extensions;

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
                if (!await db.Baskets.AnyAsync(ct))
                {
                    var d = new List<BasketItem>();
                    d.Add(new BasketItem
                    {
                        Quantity = 1,BasketId = 1,ProductId = 1,Price = 12.50m
                        ,ProductName = "Kot",ImageUrl = "",
                    });
                    await db.Baskets.AddRangeAsync(
                        new Domain.Entities.Basket(1,1,d));
                    await db.SaveChangesAsync(ct);
                }
            }
        }
        
    }

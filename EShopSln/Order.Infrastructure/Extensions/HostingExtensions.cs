using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Order.Domain.OrderAggregate;
using Order.Infrastructure.Context;

namespace Order.Infrastructure.Extensions;

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
                if (!await db.Orders.AnyAsync(ct))
                {
                    await db.Orders.AddRangeAsync(
                        new Domain.OrderAggregate.Order(1,1,
                            new Address("İstanbul", "Bağcılar", "2200.Sokak", "34200", "")));
                    await db.SaveChangesAsync(ct);
                }
                
                if (!await db.OrderItems.AnyAsync(ct))
                {
                    await db.OrderItems.AddRangeAsync(
                        new OrderItem(1,1,"Pantolon","ımageUrl",25.5m,1));
                    await db.SaveChangesAsync(ct);
                }
            }
        }
    }
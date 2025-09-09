using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payment.Persistence.Context;

namespace Payment.Persistence.Extensions;

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
                if (!await db.Payments.AnyAsync(ct))
                {
                    await db.Payments.AddRangeAsync(
                        new Domain.Entities.Payment(1, 1, Guid.NewGuid(), 10.5m));
                    await db.SaveChangesAsync(ct);
                }
            }
        }
    }
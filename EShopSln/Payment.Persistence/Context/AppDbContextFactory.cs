using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Payment.Persistence.Context;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var conn =
            "Host=localhost;Port=5432;Database=PaymentDb;Username=sametbaglan;Password=1425369As;Ssl Mode=Prefer;Trust Server Certificate=true";
                   

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(conn, x => x.MigrationsAssembly("Payment.Persistence"))
            .Options;

        return new AppDbContext(options);
    }
}
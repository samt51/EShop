using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;


namespace Payment.Persistence.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        b.Entity<Domain.Entities.Payment>(b =>
        {
            b.ToTable("payments");  
            b.HasKey(x => x.Id);
        });

    }
    public DbSet<Domain.Entities.Payment> Payments { get; set; }

}
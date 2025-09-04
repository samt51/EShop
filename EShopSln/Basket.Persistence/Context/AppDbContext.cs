using Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Basket.Persistence.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public DbSet<Domain.Entities.Basket> Baskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
}
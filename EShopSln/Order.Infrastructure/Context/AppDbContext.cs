using Microsoft.EntityFrameworkCore;
using Order.Domain.OrderAggregate;

namespace Order.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public const string DEFAULT_SCHEMA = "ordering";
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order.Domain.OrderAggregate.Order>().ToTable("Orders", DEFAULT_SCHEMA);
        modelBuilder.Entity<OrderItem>().ToTable("OrderItems", DEFAULT_SCHEMA);

        modelBuilder.Entity<OrderItem>().Property(x => x.Price).HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order.Domain.OrderAggregate.Order>().OwnsOne(o => o.Address).WithOwner();

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Order.Domain.OrderAggregate.Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}
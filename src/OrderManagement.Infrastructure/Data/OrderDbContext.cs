using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Data;

/// <summary>
/// EF Core DbContext for Order management.
/// </summary>
public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.OrderNumber)
                .IsUnique();

            entity.Property(e => e.CustomerEmail)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(e => e.CustomerEmail);

            entity.Property(e => e.OrderDate)
                .IsRequired();

            entity.Property(e => e.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.Property(e => e.Status)
                .IsRequired();

            entity.HasIndex(e => e.Status);
        });
    }
}

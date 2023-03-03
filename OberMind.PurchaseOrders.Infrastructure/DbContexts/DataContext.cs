using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OberMind.PurchaseOrders.Domain.Entities;

namespace OberMind.PurchaseOrders.Infrastructure.DbContexts;

public class DataContext: DbContext 
{ 
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<LineItem> LineItems { get; set; }
    public DbSet<User> Users { get; set; }

    public DataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseOrder>()
            .HasMany(p => p.LineItems)
            .WithOne(l => l.PurchaseOrder)
            .HasForeignKey(l => l.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PurchaseOrder>()
            .Property(p => p.Name)
            .HasDefaultValueSql("NEWID()");

        modelBuilder.Entity<PurchaseOrder>()
            .Property(p => p.Status)
            .HasDefaultValue(PurchaseOrderStatus.DRAFT);

        modelBuilder.Entity<PurchaseOrder>()
            .Property(p => p.CreatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        modelBuilder.Entity<LineItem>()
            .Property(l => l.Amount)
            .HasColumnType("decimal(18,2)");

        base.OnModelCreating(modelBuilder);
    }
}
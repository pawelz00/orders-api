using Microsoft.EntityFrameworkCore;
using OrdersApi.Application.Interfaces.Infrastructure;
using OrdersApi.Domain.Models;

namespace OrdersApi.Infrastructure.Data
{
    public class AppDbContext : DbContext, IApplicationDbContext // Implement interface
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<OrderItem>()
                .HasKey(orderItem => new { orderItem.OrderId, orderItem.ProductId });

            modelBuilder.Entity<OrderItem>()
                .HasOne(orderItem => orderItem.Order)
                .WithMany(order => order.OrderItems)
                .HasForeignKey(orderItem => orderItem.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(orderItem => orderItem.Product)
                .WithMany(product => product.OrderItems)
                .HasForeignKey(orderItem => orderItem.ProductId);

            modelBuilder.Entity<Product>()
               .Property(p => p.Price)
               .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
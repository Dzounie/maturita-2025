using Eshop.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Api.Data;


//DbContextOptions<EshopContext> options říká: použij "options" (connstring, typ databáze...), které jsou nastaveny v program.cs pro EshopContext
public class EshopContext(DbContextOptions<EshopContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CartItem> Carts => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade); //když smažu uživatele, smažou se i jeho objednávky

        modelBuilder.Entity<OrderStatus>().HasData(
            new { Id = 1, StatusName = "Vytvořeno" },
            new { Id = 2, StatusName = "Zpracovává se" },
            new { Id = 3, StatusName = "Odesláno" },
            new { Id = 4, StatusName = "Doručeno" }
        );

        modelBuilder.Entity<User>().
           Property(u => u.Role).HasDefaultValue("User");
    }
}

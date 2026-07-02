using MatrixInc.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixInc.DataAccess;

public class MatrixDbContext : DbContext
{
    public MatrixDbContext(DbContextOptions<MatrixDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuratie
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.HasMany(e => e.OrderItems)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Customer configuratie
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasOne(e => e.Address)
                .WithMany()
                .HasForeignKey(e => e.AddressId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.Orders)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Address configuratie
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // Order configuratie
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.HasMany(e => e.OrderItems)
                .WithOne(e => e.Order)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderItem configuratie
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Tandwiel A200", Description = "Hoogwaardig stalen tandwiel voor industriële toepassingen", Price = 45.99m, StockQuantity = 150, Category = "Tandwielen", IsActive = true },
            new Product { Id = 2, Name = "Lager B350", Description = "Kogellager met hoge belastingscapaciteit", Price = 32.50m, StockQuantity = 200, Category = "Lagers", IsActive = true },
            new Product { Id = 3, Name = "Moer M12", Description = "Zeskant moer M12 verzinkt", Price = 0.75m, StockQuantity = 5000, Category = "Bevestigingsmiddelen", IsActive = true },
            new Product { Id = 4, Name = "Bout M12x50", Description = "Zeskantbout M12x50 verzinkt", Price = 1.25m, StockQuantity = 3000, Category = "Bevestigingsmiddelen", IsActive = true },
            new Product { Id = 5, Name = "Koppeling C450", Description = "Flexibele koppeling voor asverbindingen", Price = 125.00m, StockQuantity = 75, Category = "Koppelingen", IsActive = true },
            new Product { Id = 6, Name = "Spindel D600", Description = "Precisie spindel voor CNC machines", Price = 850.00m, StockQuantity = 25, Category = "Spindels", IsActive = true },
            new Product { Id = 7, Name = "Riemschijf E100", Description = "V-snaar riemschijf 100mm diameter", Price = 28.75m, StockQuantity = 120, Category = "Aandrijvingen", IsActive = true },
            new Product { Id = 8, Name = "Pakking F25", Description = "Rubber pakking 25mm", Price = 3.50m, StockQuantity = 500, Category = "Pakkingen", IsActive = true }
        );

        // Seed Addresses
        modelBuilder.Entity<Address>().HasData(
            new Address { Id = 1, Street = "Hoofdstraat", HouseNumber = "1", PostalCode = "1234AB", City = "Amsterdam", Country = "Nederland" },
            new Address { Id = 2, Street = "Dorpsweg", HouseNumber = "25", PostalCode = "5678CD", City = "Rotterdam", Country = "Nederland" },
            new Address { Id = 3, Street = "Kerkstraat", HouseNumber = "42", ApartmentNumber = "2B", PostalCode = "3456EF", City = "Utrecht", Country = "Nederland" }
        );

        // Seed Customers met nieuwe Address relatie
        modelBuilder.Entity<Customer>().HasData(
            new Customer 
            { 
                Id = 1, 
                Name = "Jan Bakker", 
                Email = "jan.bakker@example.com", 
                PhoneNumber = "0612345678", 
                AddressId = 1,
                AddressOld = "Hoofdstraat 1, 1234AB Amsterdam", // Backwards compatibility
                CreatedDate = new DateTime(2024, 6, 1, 12, 0, 0)
            },
            new Customer 
            { 
                Id = 2, 
                Name = "Marie de Vries", 
                Email = "marie.devries@example.com", 
                PhoneNumber = "0687654321", 
                AddressId = 2,
                AddressOld = "Dorpsweg 25, 5678CD Rotterdam",
                CreatedDate = new DateTime(2024, 9, 1, 12, 0, 0)
            },
            new Customer 
            { 
                Id = 3, 
                Name = "Piet Jansen", 
                Email = "piet.jansen@example.com", 
                PhoneNumber = "0698765432", 
                AddressId = 3,
                AddressOld = "Kerkstraat 42-2B, 3456EF Utrecht",
                CreatedDate = new DateTime(2024, 11, 1, 12, 0, 0)
            }
        );
    }
}

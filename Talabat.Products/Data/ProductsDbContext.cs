using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Talabat.Products.Domain;

namespace Talabat.Products.Data;

public class ProductsDbContext : DbContext
{
    private Guid product1Id = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5f");
    private Guid product2Id = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5c");
    private Guid merchantId = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5c");

    internal DbSet<Product> Products { get; set; }

    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Products");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // @TODO: Remove Seed Data After Testing
        modelBuilder.Entity<Product>().HasData(new Product(merchantId, "Product 1", 40, product1Id));
        modelBuilder.Entity<Product>().HasData(new Product(merchantId, "Product 2", 80, product2Id));

        base.OnModelCreating(modelBuilder);
    }
}
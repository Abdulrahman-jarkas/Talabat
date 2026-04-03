using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Talabat.Products.Domain;
using Talabat.SharedKernal;

namespace Talabat.Products.Data;

public class ProductsDbContext : DbContext
{
    private readonly IPublisher _publisher;

    private Guid product1Id = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5f");
    private Guid product2Id = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5c");
    private Guid merchantId = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5c");

    internal DbSet<Product> Products { get; set; }

    public ProductsDbContext(DbContextOptions<ProductsDbContext> options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Products");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // @TODO: Remove Seed Data After Testing
        modelBuilder.Entity<Product>().HasData(
            new Product(merchantId, "Product 1", 40, 100, product1Id),
            new Product(merchantId, "Product 2", 80, 100, product2Id));

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .SelectMany(e => e.Entity.PopDomainEvents())
            .ToList();

        foreach (var domainEvent in domainEvents)
            await _publisher.Publish(domainEvent, cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }
}
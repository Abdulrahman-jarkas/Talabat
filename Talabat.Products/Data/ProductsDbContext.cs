using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Talabat.Products.Domain;
using Talabat.SharedKernal;

namespace Talabat.Products.Data;

public class ProductsDbContext : DbContext
{
    private readonly IPublisher _publisher;

    internal DbSet<Product> Products { get; set; }
    internal DbSet<Shop> Shops { get; set; }

    public ProductsDbContext(DbContextOptions<ProductsDbContext> options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Products");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

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
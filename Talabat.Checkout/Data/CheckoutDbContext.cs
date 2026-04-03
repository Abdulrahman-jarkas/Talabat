using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Talabat.Checkout.Domain.CheckoutSessionAggregate;
using Talabat.Checkout.Domain.ProductAggregate;
using Talabat.SharedKernal;

namespace Talabat.Checkout.Data;

public class CheckoutDbContext : DbContext
{
	private readonly IPublisher _publisher;

	internal DbSet<CheckoutSession> CheckoutSessions { get; set; }
	internal DbSet<CheckoutItem> CheckoutItems { get; set; }
	internal DbSet<Product> Products { get; set; }

	public CheckoutDbContext(DbContextOptions<CheckoutDbContext> options, IPublisher publisher) : base(options)
	{
		_publisher = publisher;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("Checkout");

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

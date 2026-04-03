using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Talabat.Orders.Domain.CheckoutSessionAggregate;
using Talabat.Orders.Domain.OrderAggregate;
using Talabat.SharedKernal;

namespace Talabat.Orders.Data;

public class OrdersDbContext : DbContext
{
	private readonly IPublisher _publisher;

	internal DbSet<Order> Orders { get; set; }
	internal DbSet<CheckoutSession> CheckoutSessions { get; set; }
	internal DbSet<CheckoutItem> CheckoutItems { get; set; }

	public OrdersDbContext(DbContextOptions<OrdersDbContext> options, IPublisher publisher) : base(options)
	{
		_publisher = publisher;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("Orders");

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
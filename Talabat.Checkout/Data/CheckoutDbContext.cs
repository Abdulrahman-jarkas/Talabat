using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Talabat.Checkout.Domain.CheckoutSessionAggregate;

namespace Talabat.Checkout.Data;

public class CheckoutDbContext : DbContext
{
	internal DbSet<CheckoutSession> CheckoutSessions { get; set; }
	internal DbSet<CheckoutItem> CheckoutItems { get; set; }

	public CheckoutDbContext(DbContextOptions<CheckoutDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("Checkout");

		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}

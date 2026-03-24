using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
using Talabat.Users.Domain.MerchantAggregate;

namespace Talabat.Users.Data;

public class UsersDbContext : DbContext
{
	internal DbSet<Merchant> Merchants { get; set; }
	internal DbSet<Customer> Customers { get; set; }
	internal DbSet<CheckoutSession> CheckoutSessions { get; set; }
	internal DbSet<CheckoutItem> CheckoutItems { get; set; }

	public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("Users");

		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}
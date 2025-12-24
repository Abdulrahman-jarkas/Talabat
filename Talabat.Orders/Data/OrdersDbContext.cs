using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Talabat.Orders.Domain.OrderAggregate;

namespace Talabat.Orders.Data;

public class OrdersDbContext : DbContext
{
	internal DbSet<Order> Orders { get; set; }
	public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("Orders");

		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}
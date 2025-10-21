using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Talabat.OrderProcessing.Domain.OrderAggregate;

namespace Talabat.OrderProcessing.Data;

public class OrderProcessingDbContext : DbContext
{
	public DbSet<Order> Orders { get; set; }

	public OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}

using Microsoft.EntityFrameworkCore;
using ProductsManagement.Domain.ProductAggregate;
using System.Reflection;

namespace ProductsManagement.Infrastructure.Persistence;

public class ProductsManagementDbContext : DbContext
{
	public DbSet<Product> Products { get; set; }

	public ProductsManagementDbContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}

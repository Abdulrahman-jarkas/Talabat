using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Infrastructure.Persistence;

public class ProductsManagementDbContext : DbContext
{
	public DbSet<Product> Products { get; set; }
	public DbSet<Modifier> Modifiers { get; set; }
	//public DbSet<ModifierGroup> ModifierGroups { get; set; }

	public ProductsManagementDbContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}

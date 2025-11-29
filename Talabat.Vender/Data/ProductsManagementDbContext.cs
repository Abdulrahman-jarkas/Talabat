using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Domain.ProductAggregate.Entities;
using Talabat.Vender.Domain.VendorAggregate;

namespace Talabat.Vender.Infrastructure.Persistence;

public class ProductsManagementDbContext : DbContext
{
	public DbSet<VendorEntity> Venders { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Modifier> Modifiers { get; set; }

	public ProductsManagementDbContext(DbContextOptions<ProductsManagementDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}

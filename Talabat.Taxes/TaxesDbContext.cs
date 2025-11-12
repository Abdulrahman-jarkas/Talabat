using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Talabat.Taxes;

public class TaxesDbContext : DbContext
{
	public DbSet<TaxPolicy> TaxPolicies { get; set; }
	public DbSet<TaxCategory> TaxCategories { get; set; }

	public TaxesDbContext(DbContextOptions<TaxesDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}

using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Talabat.Taxes.Domain;

namespace Talabat.Taxes.Configuration;

public class TaxesDbContext : DbContext
{
	public DbSet<Tax> Taxes { get; set; }
	public DbSet<Country> Countries { get; set; }

	public TaxesDbContext(DbContextOptions<TaxesDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}

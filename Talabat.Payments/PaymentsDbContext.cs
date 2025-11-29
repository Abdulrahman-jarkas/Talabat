using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Talabat.Payments;

public class PaymentsDbContext : DbContext
{
	public DbSet<Payment> Payments { get; set; }

	public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}

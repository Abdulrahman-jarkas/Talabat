using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Data;

public class UsersDbContext : DbContext
{
	internal DbSet<Customer> Customers { get; set; }

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
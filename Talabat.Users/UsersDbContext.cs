using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Talabat.Users;

public class UsersDbContext : DbContext
{
	private Guid customer1Id = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5f");
	private Guid merchantId = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5c");

	internal DbSet<Merchant> Merchants { get; set; }
	internal DbSet<Customer> Customers { get; set; }

	public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("Users");

		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		// @TODO: Remove Seed Data After Testing
		modelBuilder.Entity<Merchant>().HasData(new Merchant("merchant1@gamil.com", merchantId));
		modelBuilder.Entity<Customer>().HasData(new Customer("customer@gamil.com", customer1Id));

		base.OnModelCreating(modelBuilder);
	}
}
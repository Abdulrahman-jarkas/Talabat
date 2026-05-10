using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Talabat.Invoices;

public class InvoicesDbContext : DbContext
{
	private Guid customer1Id = Guid.Parse("1fb673f4-6974-478b-b4eb-b9882dd13c5f");

	internal DbSet<Invoice> Invoices { get; set; }
	internal DbSet<Customer> Customers { get; set; }


	public InvoicesDbContext(DbContextOptions<InvoicesDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("Invoices");

		//@TODO: we need to listen on CustomerCreated event to create a customer when a new user is created
		modelBuilder.Entity<Customer>().HasData(new Customer(customer1Id));

		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}
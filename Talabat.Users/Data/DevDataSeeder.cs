using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Data;

public static class DevDataSeeder
{
	// Customer IDs are the same as Account IDs (customers are linked by account)
	public static readonly Guid AdminCustomerId = Guid.Parse("20000000-0000-0000-0000-000000000004");
	public static readonly Guid OwnerCustomerId = Guid.Parse("20000000-0000-0000-0000-000000000005");
	public static readonly Guid StaffCustomerId = Guid.Parse("20000000-0000-0000-0000-000000000008");

	public static async Task SeedUsersDataAsync(this IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

		if (await db.Customers.AnyAsync())
			return;

		var adminCustomer = new Customer(AdminCustomerId, "admin@talabat.com");
		adminCustomer.AddAddress("456 King Fahd Road, Al Olaya, Riyadh 12211");
		adminCustomer.AddAddress("12 Prince Sultan Street, Al Rawdah, Jeddah 23432");

		var ownerCustomer = new Customer(OwnerCustomerId, "owner@talabat.com");
		ownerCustomer.AddAddress("89 Tahlia Street, Al Sulaimaniyah, Riyadh 12214");

		var staffCustomer = new Customer(StaffCustomerId, "staff@talabat.com");
		staffCustomer.AddAddress("100 Main Street, Al Malaz, Riyadh 12836");

		db.Customers.AddRange(adminCustomer, ownerCustomer, staffCustomer);

		await db.SaveChangesAsync();
	}
}

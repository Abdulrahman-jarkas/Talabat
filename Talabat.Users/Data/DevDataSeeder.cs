using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Data;

public static class DevDataSeeder
{
	public static readonly Guid AhmedCustomerId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012");
	public static readonly Guid SaraCustomerId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123");

	public static async Task SeedUsersDataAsync(this IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

		if (await db.Customers.AnyAsync())
			return;

		var ahmed = new Customer("ahmed.ali@gmail.com", AhmedCustomerId);
		ahmed.AddAddress("456 King Fahd Road, Al Olaya, Riyadh 12211");
		ahmed.AddAddress("12 Prince Sultan Street, Al Rawdah, Jeddah 23432");

		var sara = new Customer("sara.mohammed@outlook.com", SaraCustomerId);
		sara.AddAddress("89 Tahlia Street, Al Sulaimaniyah, Riyadh 12214");

		db.Customers.AddRange(ahmed, sara);

		await db.SaveChangesAsync();
	}
}

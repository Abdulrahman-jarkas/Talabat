using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.MerchantAggregate;

namespace Talabat.Users.Data;

public static class DevDataSeeder
{
	public static readonly Guid AlBaikMerchantId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
	public static readonly Guid ShawarmaHouseMerchantId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");

	public static readonly Guid AhmedCustomerId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012");
	public static readonly Guid SaraCustomerId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123");

	public static async Task SeedUsersDataAsync(this IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

		if (await db.Merchants.AnyAsync())
			return;

		var alBaik = new Merchant("contact@albaik.com", AlBaikMerchantId);
		var shawarmaHouse = new Merchant("info@shawarmahouse.sa", ShawarmaHouseMerchantId);

		db.Merchants.AddRange(alBaik, shawarmaHouse);

		var ahmed = new Customer("ahmed.ali@gmail.com", AhmedCustomerId);
		ahmed.AddAddress("456 King Fahd Road, Al Olaya, Riyadh 12211");
		ahmed.AddAddress("12 Prince Sultan Street, Al Rawdah, Jeddah 23432");

		var sara = new Customer("sara.mohammed@outlook.com", SaraCustomerId);
		sara.AddAddress("89 Tahlia Street, Al Sulaimaniyah, Riyadh 12214");

		db.Customers.AddRange(ahmed, sara);

		await db.SaveChangesAsync();
	}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Products.Domain;
using Talabat.SharedKernal;

namespace Talabat.Products.Data;

public static class DevDataSeeder
{
	/// <summary>
	/// Fixed Shop IDs for testing purposes.
	/// </summary>
	public static class TestShopIds
	{
		public static readonly Guid AlBaik = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
		public static readonly Guid ShawarmaHouse = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");
	}

	public static async Task SeedProductsDataAsync(this IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

		await SeedShopsAsync(db);
		await SeedProductsAsync(db);
	}

	private static async Task SeedShopsAsync(ProductsDbContext db)
	{
		if (await db.Shops.AnyAsync())
			return;

		db.Shops.AddRange(
			new Shop(TestShopIds.AlBaik, "Al Baik"),
			new Shop(TestShopIds.ShawarmaHouse, "Shawarma House")
		);

		await db.SaveChangesAsync();
	}

	private static async Task SeedProductsAsync(ProductsDbContext db)
	{
		if (await db.Products.AnyAsync())
			return;

		db.Products.AddRange(
			new Product(TestShopIds.AlBaik, "Broasted Chicken Meal", 45.00m, 100,
				Guid.Parse("11111111-1111-1111-1111-111111111111")),
			new Product(TestShopIds.AlBaik, "Chicken Nuggets (10 pcs)", 25.00m, 200,
				Guid.Parse("22222222-2222-2222-2222-222222222222")),
			new Product(TestShopIds.AlBaik, "Fish Fillet Meal", 40.00m, 80,
				Guid.Parse("33333333-3333-3333-3333-333333333333")),

			new Product(TestShopIds.ShawarmaHouse, "Chicken Shawarma Wrap", 18.00m, 150,
				Guid.Parse("44444444-4444-4444-4444-444444444444")),
			new Product(TestShopIds.ShawarmaHouse, "Beef Shawarma Plate", 35.00m, 120,
				Guid.Parse("55555555-5555-5555-5555-555555555555")),
			new Product(TestShopIds.ShawarmaHouse, "Falafel Wrap", 12.00m, 200,
				Guid.Parse("66666666-6666-6666-6666-666666666666"))
		);

		// Clear domain events raised by Product constructors to prevent publishing during seeding
		foreach (var entry in db.ChangeTracker.Entries<AggregateRoot>())
			entry.Entity.PopDomainEvents();

		await db.SaveChangesAsync();
	}
}

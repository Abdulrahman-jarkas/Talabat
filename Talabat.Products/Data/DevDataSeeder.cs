using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Products.Domain;
using Talabat.SharedKernal;

namespace Talabat.Products.Data;

public static class DevDataSeeder
{
	public static async Task SeedProductsDataAsync(this IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

		if (await db.Products.AnyAsync())
			return;

		var alBaikId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
		var shawarmaHouseId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");

		db.Products.AddRange(
			new Product(alBaikId, "Broasted Chicken Meal", 45.00m, 100,
				Guid.Parse("11111111-1111-1111-1111-111111111111")),
			new Product(alBaikId, "Chicken Nuggets (10 pcs)", 25.00m, 200,
				Guid.Parse("22222222-2222-2222-2222-222222222222")),
			new Product(alBaikId, "Fish Fillet Meal", 40.00m, 80,
				Guid.Parse("33333333-3333-3333-3333-333333333333")),

			new Product(shawarmaHouseId, "Chicken Shawarma Wrap", 18.00m, 150,
				Guid.Parse("44444444-4444-4444-4444-444444444444")),
			new Product(shawarmaHouseId, "Beef Shawarma Plate", 35.00m, 120,
				Guid.Parse("55555555-5555-5555-5555-555555555555")),
			new Product(shawarmaHouseId, "Falafel Wrap", 12.00m, 200,
				Guid.Parse("66666666-6666-6666-6666-666666666666"))
		);

		// Clear domain events raised by Product constructors to prevent publishing during seeding
		foreach (var entry in db.ChangeTracker.Entries<AggregateRoot>())
			entry.Entity.PopDomainEvents();

		await db.SaveChangesAsync();
	}
}

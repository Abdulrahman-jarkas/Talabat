using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Products.Domain;

namespace Talabat.Products.Data.Configuration;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Id)
			.IsRequired()
			.ValueGeneratedNever();

		builder.Property(p => p.ShopId).IsRequired();
		builder.Property(p => p.Title).IsRequired().HasMaxLength(256);
		builder.Property(p => p.BasePrice).HasPrecision(18, 2).IsRequired();
		builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);

		builder.OwnsOne(p => p.Stock, stockBuilder =>
		{
			stockBuilder.Property(s => s.Quantity)
				.HasColumnName("Quantity")
				.IsRequired();

			stockBuilder.Property(s => s.Reservations)
				.HasConversion(
					v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
					v => (IReadOnlyDictionary<Guid, Reservation>)(JsonSerializer.Deserialize<Dictionary<Guid, Reservation>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<Guid, Reservation>()))
				.HasColumnType("nvarchar(max)")
				.HasColumnName("Reservations");

			stockBuilder.Property(s => s.Reservations)
				.Metadata.SetValueComparer(new ValueComparer<IReadOnlyDictionary<Guid, Reservation>>(
					(a, b) => a != null && b != null && a.Count == b.Count
						&& a.All(kvp => b.ContainsKey(kvp.Key) && kvp.Value.Equals(b[kvp.Key])),
					c => c.Aggregate(0, (hash, kvp) => HashCode.Combine(hash, kvp.Key.GetHashCode(), kvp.Value.GetHashCode())),
					c => (IReadOnlyDictionary<Guid, Reservation>)new Dictionary<Guid, Reservation>(c)));

			stockBuilder.Ignore(s => s.EffectiveQuantity);
		});

		builder.Navigation(p => p.Stock).IsRequired();

		builder.HasQueryFilter(p => !p.IsDeleted);
	}
}

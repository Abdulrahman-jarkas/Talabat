using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Infrastructure.Persistence.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Title);
		builder.Property(p => p.BasePrice)
			.HasPrecision(18, 2);

		builder.HasMany(p => p.ModifierGroups)
			.WithMany()
			.UsingEntity<Dictionary<string, object>>(
				"ProductsModifierGroups",
				j => j
					.HasOne<ModifierGroup>()
					.WithMany()
					.HasForeignKey("ModifierGroupId")
					.OnDelete(DeleteBehavior.Cascade),
				j => j
					.HasOne<Product>()
					.WithMany()
					.HasForeignKey("ProductId")
					.OnDelete(DeleteBehavior.Cascade),
				j =>
				{
					j.HasKey("ProductId", "ModifierGroupId");
					j.ToTable("ProductsModifierGroups");
				});
	}
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsManagement.Domain.ProductAggregate;

namespace ProductsManagement.Infrastructure.Persistence.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.HasKey(p => p.Id);

		builder.HasMany(p => p.ModifierGroups)
			.WithMany()
			.UsingEntity(j => j.ToTable("ProductModifierGroups"));

		builder.Property(p => p.Title);
		builder.Property(p => p.BasePrice)
			.HasPrecision(18, 2);
	}
}
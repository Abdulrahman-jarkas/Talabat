using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Vender.Domain.ProductAggregate;

namespace Talabat.Vender.Infrastructure.Persistence.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.HasKey(p => p.Id);

		builder.HasMany(p => p.ModifierGroups)
			.WithMany();

		builder.Property(p => p.Title);
		builder.Property(p => p.BasePrice)
			.HasPrecision(18, 2);
	}
}
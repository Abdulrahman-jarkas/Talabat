using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Vender.Domain.ProductAggregate;

namespace Talabat.Vender.Infrastructure.Persistence.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Title);

		builder.Property(p => p.BasePrice)
			.HasPrecision(18, 2);

		builder.Property(p => p.TaxCategoryId);

		builder.OwnsOne(d => d.Customization, c =>
		{
			c.Property(c => c.ModifierGroups)
			  .HasColumnName("Data")
			  .HasValueJsonConverter();
		});
	}
}
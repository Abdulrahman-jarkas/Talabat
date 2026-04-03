using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Checkout.Domain.ProductAggregate;

namespace Talabat.Checkout.Data.Configuration;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.ToTable("Products");

		builder.HasKey(p => p.Id);

		builder.Property(p => p.Id)
			.IsRequired()
			.ValueGeneratedNever();

		builder.Property(p => p.Quantity).IsRequired();
		builder.Property(p => p.ReservedQuantity).IsRequired();
		builder.Property(p => p.BasePrice).HasPrecision(18, 2).IsRequired();
		builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);

		builder.Ignore(p => p.AvailableQuantity);
	}
}

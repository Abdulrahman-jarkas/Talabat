using Microsoft.EntityFrameworkCore;
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

		builder.Property(p => p.MerchantId).IsRequired();
		builder.Property(p => p.Title).IsRequired().HasMaxLength(256);
		builder.Property(p => p.BasePrice).HasPrecision(18, 2).IsRequired();

		builder.OwnsOne(p => p.Stock, stock =>
		{
			stock.Property(s => s.AvailableStock)
				.HasColumnName("AvailableStock")
				.IsRequired();

			stock.Property(s => s.ReservedStock)
				.HasColumnName("ReservedStock")
				.IsRequired();

			stock.Ignore(s => s.EffectiveStock);
		});
	}
}

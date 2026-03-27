using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Checkout.Domain.CheckoutSessionAggregate;

namespace Talabat.Checkout.Data.Configuration;

internal class CheckoutItemConfiguration : IEntityTypeConfiguration<CheckoutItem>
{
	public void Configure(EntityTypeBuilder<CheckoutItem> builder)
	{
		builder.ToTable("CheckoutItems");

		builder.HasKey(ci => ci.Id);

		builder.Property(ci => ci.Id)
			.IsRequired()
			.ValueGeneratedNever();

		builder.Property(ci => ci.ProductId)
			.HasColumnName("ProductId")
			.IsRequired();

		builder.Property(ci => ci.Quantity)
			.HasColumnName("Quantity")
			.IsRequired();

		builder.Property(ci => ci.Price)
			.HasColumnName("Price")
			.HasPrecision(18, 2)
			.IsRequired();

		builder.HasIndex("CheckoutSessionId")
			.HasDatabaseName("IX_CheckoutItems_CheckoutSessionId");
	}
}

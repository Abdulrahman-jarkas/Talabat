using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Users.Domain.CustomerAggregate.Checkout;

namespace Talabat.Users.Data.Configuration;

internal class CheckoutItemConfiguration : IEntityTypeConfiguration<CheckoutItem>
{
	public void Configure(EntityTypeBuilder<CheckoutItem> builder)
	{
		// Table Name
		builder.ToTable("CheckoutItems");

		// Primary Key
		builder.HasKey(ci => ci.Id);

		builder.Property(ci => ci.Id)
			.IsRequired()
			.ValueGeneratedNever(); // Guid is generated in code

		// ProductId
		builder.Property(ci => ci.ProductId)
			.HasColumnName("ProductId")
			.IsRequired();

		// Quantity
		builder.Property(ci => ci.Quantity)
			.HasColumnName("Quantity")
			.IsRequired();

		// BasePrice with decimal precision
		builder.Property(ci => ci.BasePrice)
			.HasColumnName("BasePrice")
			.HasPrecision(18, 2) // 2 decimal places for currency precision
			.IsRequired();

		// Index on CheckoutSessionId (shadow property) for performance
		builder.HasIndex("CheckoutSessionId")
			.HasDatabaseName("IX_CheckoutItems_CheckoutSessionId");
	}
}

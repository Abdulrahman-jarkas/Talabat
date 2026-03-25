using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Checkout;

namespace Talabat.Users.Data.Configuration;

internal class CheckoutSessionConfiguration : IEntityTypeConfiguration<CheckoutSession>
{
	public void Configure(EntityTypeBuilder<CheckoutSession> builder)
	{
		// Table Name
		builder.ToTable("CheckoutSessions");

		// Primary Key
		builder.HasKey(cs => cs.Id);

		builder.Property(cs => cs.Id)
			.IsRequired()
			.ValueGeneratedNever(); // Guid is generated in code

		// MerchantId
		builder.Property(cs => cs.MerchantId)
			.HasColumnName("MerchantId")
			.IsRequired();

		// AddressId (nullable)
		builder.Property(cs => cs.AddressId)
			.HasColumnName("AddressId")
			.IsRequired(false);

		// PaymentType (required)
		builder.Property(cs => cs.PaymentType)
			.HasColumnName("PaymentType")
			.HasConversion<int>()
			.IsRequired();

		// Status (required)
		builder.Property(cs => cs.Status)
			.HasColumnName("Status")
			.HasConversion<int>()
			.IsRequired();

		// PaymentId (nullable)
		builder.Property(cs => cs.PaymentId)
			.HasColumnName("PaymentId")
			.IsRequired(false);

		// OrderId (nullable)
		builder.Property(cs => cs.OrderId)
			.HasColumnName("OrderId")
			.IsRequired(false);

		// Relationship with Customer (One-to-Many)
		// CustomerId is managed as shadow property by EF Core
		builder.HasOne<Customer>()
			.WithMany()
			.HasForeignKey("CustomerId")
			.OnDelete(DeleteBehavior.Cascade);

		// Relationship with CheckoutItems (One-to-Many)
		// Map to the backing field _items
		builder.HasMany(cs => cs.Items)
			.WithOne()
			.HasForeignKey("CheckoutSessionId")
			.OnDelete(DeleteBehavior.Cascade);
	}
}

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

		// Relationship with Customer (One-to-One)
		// CustomerId is managed as shadow property by EF Core
		builder.HasOne<Customer>()
			.WithOne(c => c.ActiveCheckoutSession)
			.HasForeignKey<CheckoutSession>("CustomerId")
			.OnDelete(DeleteBehavior.Cascade);

		// Relationship with CheckoutItems (One-to-Many)
		builder.HasMany<CheckoutItem>()
			.WithOne()
			.HasForeignKey("CheckoutSessionId")
			.OnDelete(DeleteBehavior.Cascade);
	}
}

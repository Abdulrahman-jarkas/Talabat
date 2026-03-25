using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Checkout;

namespace Talabat.Users.Data.Configuration;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
	public void Configure(EntityTypeBuilder<Customer> builder)
	{
		// Table Name
		builder.ToTable("Customers");

		// Primary Key
		builder.HasKey(c => c.Id);

		builder.Property(c => c.Id)
			.IsRequired()
			.ValueGeneratedNever(); // Guid is generated in code

		// Email Property
		builder.Property(c => c.Email)
			.IsRequired()
			.HasMaxLength(256)
			.HasColumnName("Email");

		// Cart as single JSON column (entire value object stored as one JSON)
		builder.Property(c => c.Cart)
			.HasColumnName("Cart")
			.HasValueJsonConverter()
			.IsRequired(false); // Can be null when cart doesn't exist

		// CheckoutSessions Collection (One-to-Many relationship)
		builder.HasMany(c => c.CheckoutSessions)
			.WithOne()
			.HasForeignKey("CustomerId")
			.OnDelete(DeleteBehavior.Cascade);

		// Addresses Collection (One-to-Many relationship)
		builder.HasMany(a => a.Addresses)
			.WithOne()
			.HasForeignKey("CustomerId")
			.OnDelete(DeleteBehavior.Cascade);

		// Ignore ActiveCheckoutSession - it's a computed property
		builder.Ignore(c => c.ActiveCheckoutSession);
	}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Checkout.Domain.CheckoutSessionAggregate;

namespace Talabat.Checkout.Data.Configuration;

internal class CheckoutSessionConfiguration : IEntityTypeConfiguration<CheckoutSession>
{
	public void Configure(EntityTypeBuilder<CheckoutSession> builder)
	{
		builder.ToTable("CheckoutSessions");

		builder.HasKey(cs => cs.Id);

		builder.Property(cs => cs.Id)
			.IsRequired()
			.ValueGeneratedNever();

		builder.Property(cs => cs.CustomerId)
			.HasColumnName("CustomerId")
			.IsRequired();

		builder.Property(cs => cs.MerchantId)
			.HasColumnName("MerchantId")
			.IsRequired();

		builder.Property(cs => cs.AddressId)
			.HasColumnName("AddressId")
			.IsRequired();

		builder.OwnsOne(cs => cs.Lifetime, lifetime =>
		{
			lifetime.Property(l => l.StoredStatus)
				.HasColumnName("Status")
				.HasConversion<int>()
				.IsRequired();

			lifetime.Property(l => l.CreatedAt)
				.HasColumnName("CreatedAt")
				.IsRequired();

			lifetime.Property(l => l.ExpiresAt)
				.HasColumnName("ExpiresAt")
				.IsRequired();

			lifetime.Ignore(l => l.Status);
			lifetime.Ignore(l => l.IsActive);
			lifetime.Ignore(l => l.IsExpired);
		});

		builder.Property(cs => cs.PaymentId)
			.HasColumnName("PaymentId")
			.IsRequired(false);

		builder.Property(cs => cs.OrderId)
			.HasColumnName("OrderId")
			.IsRequired(false);

		builder.HasMany(cs => cs.Items)
			.WithOne()
			.HasForeignKey("CheckoutSessionId")
			.OnDelete(DeleteBehavior.Cascade);

		builder.Ignore(cs => cs.TotalPrice);

		builder.HasIndex(cs => cs.CustomerId)
			.HasDatabaseName("IX_CheckoutSessions_CustomerId");
	}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Talabat.Payments;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Status);
		builder.Property(p => p.Url).HasMaxLength(500);
		builder.Property(p => p.CustomerId);
		builder.Property(p => p.CheckoutSessionId);
		builder.Property(p => p.Amount).HasPrecision(18, 2);
	}
}

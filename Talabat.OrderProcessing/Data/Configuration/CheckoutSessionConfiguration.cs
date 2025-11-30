using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.OrderProcessing.Domain.CheckoutSessionAggregate;
using Talabat.Vender.OrderProcessing.Data.Configuration;

namespace Talabat.OrderProcessing.Data.Configuration;

public class CheckoutSessionConfiguration : IEntityTypeConfiguration<CheckoutSession>
{
	public void Configure(EntityTypeBuilder<CheckoutSession> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Items)
			.HasValueJsonConverter();
	}
}


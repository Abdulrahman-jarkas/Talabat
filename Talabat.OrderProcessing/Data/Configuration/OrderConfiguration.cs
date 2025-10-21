using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.OrderProcessing.Domain.OrderAggregate;
using Talabat.Vender.OrderProcessing.Data.Configuration;

namespace Talabat.OrderProcessing.Data.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Status)
			.HasConversion(
				v => v.CurrentStatus,
				v => OrderStatus.FromStatusValue(v))
			.HasColumnName("Status");

		builder.Property(c => c.PaymentMethod)
			  .HasColumnName("PaymentMethod");

		builder.Property(c => c.PaymentStatus)
		  .HasColumnName("PaymentStatus");

		builder.Property(p => p.Items)
			.HasValueJsonConverter();
	}
}

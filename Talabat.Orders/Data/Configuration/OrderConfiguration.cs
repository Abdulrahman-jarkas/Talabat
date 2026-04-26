using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Orders.Domain.OrderAggregate;

namespace Talabat.Orders.Data.Configuration;

internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
		builder.ToTable("Orders");

		builder.HasKey(o => o.Id);

		builder.Property(o => o.Id)
			.IsRequired()
			.ValueGeneratedNever();

		builder.Property<uint>("xmin")
			.HasColumnType("xid")
			.ValueGeneratedOnAddOrUpdate()
			.IsConcurrencyToken();

		builder.Property(o => o.CustomerId).IsRequired();
		builder.Property(o => o.ShopId).IsRequired();
		builder.Property(o => o.CheckoutSessionId).IsRequired();
		builder.Property(o => o.AddressId).IsRequired();

		builder.Property(o => o.Status)
			.HasConversion(
				s => (int)s.CurrentStatus,
				v => OrderStatus.FromStatusValue((OrderStatusValues)v))
			.HasColumnName("Status")
			.IsRequired();

		builder.OwnsOne(o => o.Payment, payment =>
		{
			payment.Property(p => p.PaymentId)
				.HasColumnName("PaymentId")
				.IsRequired();

			payment.Property(p => p.Status)
				.HasColumnName("PaymentStatus")
				.HasConversion<int>()
				.IsRequired();
		});

		builder.Property(o => o.Items)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
				v => (IReadOnlyCollection<OrderItem>)(JsonSerializer.Deserialize<List<OrderItem>>(v, (JsonSerializerOptions?)null) ?? new List<OrderItem>()))
			.HasColumnType("jsonb")
			.HasColumnName("Items");

		builder.Property(o => o.Items)
			.Metadata.SetValueComparer(new ValueComparer<IReadOnlyCollection<OrderItem>>(
				(a, b) => a != null && b != null && a.Count == b.Count && a.SequenceEqual(b),
				c => c.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
				c => (IReadOnlyCollection<OrderItem>)new List<OrderItem>(c)));

		// Shadow property for tracking creation time (used for daily order limits)
		builder.Property<DateTime>("CreatedAt")
			.HasDefaultValueSql("CURRENT_TIMESTAMP")
			.ValueGeneratedOnAdd();

		builder.HasIndex(o => o.CustomerId)
			.HasDatabaseName("IX_Orders_CustomerId");

		builder.HasIndex("ShopId", "CreatedAt")
			.HasDatabaseName("IX_Orders_ShopId_CreatedAt");
	}
}

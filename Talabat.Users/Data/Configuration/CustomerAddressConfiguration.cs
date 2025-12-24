using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Data.Configuration;

internal class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
	public void Configure(EntityTypeBuilder<CustomerAddress> builder)
	{
		// Table Name
		builder.ToTable("CustomerAddresses");

		// Primary Key
		builder.HasKey(ca => ca.Id);

		builder.Property(ca => ca.Id)
			.IsRequired()
			.ValueGeneratedNever() // Guid is generated in code
			.HasColumnName("Id");

		// Address Property
		builder.Property(ca => ca.Address)
			.IsRequired()
			.HasMaxLength(500)
			.HasColumnName("Address");

		// Foreign Key to Customer (shadow property)
		builder.Property<Guid>("CustomerId")
			.IsRequired()
			.HasColumnName("CustomerId");
	}
}

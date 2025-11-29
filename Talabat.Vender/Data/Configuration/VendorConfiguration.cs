using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Vender.Domain.VendorAggregate;

namespace Talabat.Vender.Infrastructure.Persistence.Configuration;

public class VendorConfiguration : IEntityTypeConfiguration<VendorEntity>
{

	public void Configure(EntityTypeBuilder<VendorEntity> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Name);
		builder.Property(p => p.Email);
	}
}
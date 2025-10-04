using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Infrastructure.Persistence.Configuration;

public class ModifierGroupConfiguration : IEntityTypeConfiguration<ModifierGroup>
{
	public void Configure(EntityTypeBuilder<ModifierGroup> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Title);
		builder.Property(p => p.Min);
		builder.Property(p => p.Max);

		builder.OwnsOne(d => d.Data, c =>
		{
			c.Property(c => c.ModifierGroupItems)
			.HasColumnName("Data")
			.HasValueJsonConverter();
		});
	}
}


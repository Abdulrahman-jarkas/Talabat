using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Infrastructure.Persistence.Configuration;

public class ModifierConfiguration : IEntityTypeConfiguration<Modifier>
{
	public void Configure(EntityTypeBuilder<Modifier> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Title);
		builder.Property(p => p.Price)
			.HasPrecision(18, 2);
	}
}


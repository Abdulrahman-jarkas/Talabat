using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsManagement.Domain.ProductAggregate.Entities;

namespace ProductsManagement.Infrastructure.Persistence.Configuration;

public class ModifierConfiguration : IEntityTypeConfiguration<Modifier>
{
	public void Configure(EntityTypeBuilder<Modifier> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Title);
		builder.Property(p => p.Price)
			.HasPrecision(18, 2);

		//@TODO: use custom converter so if we change the database type then we need only to change one place
		builder.Property(p => p.ModifierGroupIds)
			.HasColumnType("uuid[]"); // for postgres only
	}
}


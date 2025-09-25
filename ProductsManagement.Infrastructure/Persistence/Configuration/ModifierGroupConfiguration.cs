using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsManagement.Domain.ProductAggregate.Entities;

namespace ProductsManagement.Infrastructure.Persistence.Configuration;

public class ModifierGroupConfiguration : IEntityTypeConfiguration<ModifierGroup>
{
	public void Configure(EntityTypeBuilder<ModifierGroup> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Title);
		builder.Property(p => p.Min);
		builder.Property(p => p.Max);

		//@TODO: use custom converter so if we change the database type then we need only to change one place
		builder.Property(p => p.ModifierIds)
			.HasColumnType("uuid[]"); // for postgres only
	}
}


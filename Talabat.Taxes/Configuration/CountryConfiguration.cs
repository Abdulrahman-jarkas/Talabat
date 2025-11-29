using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Talabat.Taxes.Domain;
using Talabat.Taxes.Common;

namespace Talabat.Taxes.Configuration;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
	public void Configure(EntityTypeBuilder<Country> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(c => c.Name);
		builder.Property(c => c.Code);
		builder.Property(c => c.Currency);

		builder.Property(c => c.ServiceFees)
			.HasValueJsonConverter();
	}
}
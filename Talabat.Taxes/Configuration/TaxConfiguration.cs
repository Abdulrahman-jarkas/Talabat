using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Talabat.Taxes.Domain;

namespace Talabat.Taxes.Configuration;

public class TaxConfiguration : IEntityTypeConfiguration<Tax>
{

	public void Configure(EntityTypeBuilder<Tax> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(c => c.VatPercentage)
			.HasPrecision(18, 2);
			   
		builder.Property(c => c.Name);
		builder.Property(c => c.CountryId);
	}
}
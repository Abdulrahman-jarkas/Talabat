using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Talabat.Taxes;

public class TaxCategoryConfiguration : IEntityTypeConfiguration<TaxCategory>
{
	public void Configure(EntityTypeBuilder<TaxCategory> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(c => c.VatPercentage)
			.HasPrecision(18, 2);
			   

		builder.Property(c => c.Name);
	}
}

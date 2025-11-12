using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Talabat.Taxes;

public class TaxPolicyConfiguration : IEntityTypeConfiguration<TaxPolicy>
{
	public void Configure(EntityTypeBuilder<TaxPolicy> builder)
	{
		builder.HasKey(p => p.Id);

		builder.HasMany(c => c.Categories)
			   .WithOne()
			   .HasForeignKey("TaxPolicyId")
			   .IsRequired()
			   .OnDelete(DeleteBehavior.Cascade);

		builder.Property(c => c.CountryCode);
	}
}
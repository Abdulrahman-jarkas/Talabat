using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Accounts.Domain.RoleAggregate;

namespace Talabat.Accounts.Data.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name).HasMaxLength(256).IsRequired();
        builder.Property(r => r.Permissions)
            .HasColumnType("nvarchar(max)")
            .IsRequired()
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

        builder.OwnsOne(r => r.Tenant, tenant =>
        {
            tenant.Property(t => t.TenantId).HasColumnName("TenantId");
            tenant.Property(t => t.TenantType).HasColumnName("TenantType").HasConversion<string>().HasMaxLength(50).IsRequired();
        });

        builder.Property(r => r.CreatedBy).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.ModifiedBy);
        builder.Property(r => r.ModifiedAt);
        builder.Property(r => r.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}

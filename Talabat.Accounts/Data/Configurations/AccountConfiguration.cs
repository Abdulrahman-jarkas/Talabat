using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Accounts.Domain.AccountAggregate;

namespace Talabat.Accounts.Data.Configurations;

internal class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.OwnsOne(a => a.User, user =>
        {
            user.Property(u => u.UserId).HasColumnName("UserId").IsRequired();
            user.Property(u => u.Name).HasColumnName("UserName").HasMaxLength(256).IsRequired();
            user.Property(u => u.Email).HasColumnName("UserEmail").HasMaxLength(256).IsRequired();

            user.HasIndex(u => u.UserId);
        });

        builder.OwnsOne(a => a.Tenant, tenant =>
        {
            tenant.Property(t => t.TenantId).HasColumnName("TenantId");
            tenant.Property(t => t.TenantType).HasColumnName("TenantType").HasConversion<string>().HasMaxLength(50).IsRequired();
        });

        builder.Property(a => a.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(a => a.LastModifiedAt).IsRequired();
        builder.Property(a => a.Version).IsRowVersion();

        builder.Property(a => a.AccountRoles)
            .HasColumnName("AccountRoles")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<IReadOnlyCollection<AccountRole>>(v, (JsonSerializerOptions?)null) ?? new List<AccountRole>(),
                new ValueComparer<IReadOnlyCollection<AccountRole>>(
                    (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null),
                    c => JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),
                    c => JsonSerializer.Deserialize<IReadOnlyCollection<AccountRole>>(
                        JsonSerializer.Serialize(c, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!));

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

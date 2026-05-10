using Microsoft.EntityFrameworkCore;
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

        builder.Navigation(a => a.User)
            .IsRequired();

        builder.OwnsOne(a => a.Tenant, tenant =>
        {
            tenant.Property(t => t.TenantId).HasColumnName("TenantId");
            tenant.Property(t => t.TenantType).HasColumnName("TenantType").HasConversion<string>().HasMaxLength(50).IsRequired();
        });

        builder.Property(a => a.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(a => a.LastModifiedAt).IsRequired();
        builder.Property(a => a.Version).IsRowVersion();

        builder.OwnsMany(a => a.Assignments, assignment =>
        {
            assignment.ToTable("AccountAssignments", "Accounts");
            assignment.HasKey(a => a.Id);
            assignment.WithOwner().HasForeignKey("AccountId");
            assignment.Property<Guid>("AccountId").IsRequired();
            assignment.Property(a => a.RoleId).IsRequired();
            assignment.Property(a => a.AssignedBy).IsRequired();
            assignment.Property(a => a.AssignedAt).IsRequired();

            assignment.HasIndex(a => a.RoleId);
            assignment.HasIndex("AccountId");
        });

        builder.Navigation(a => a.Assignments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

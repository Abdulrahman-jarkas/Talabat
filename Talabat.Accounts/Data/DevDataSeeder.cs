using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Accounts.Domain.AccountAggregate;
using Talabat.Accounts.Domain.AccountAggregate.ValueObjects;
using Talabat.SharedKernal.Authorization;
using Talabat.Accounts.Domain.RoleAggregate;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Data;

public static class DevDataSeeder
{
    public static class TestRoleIds
    {
        public static readonly Guid Admin = Guid.Parse("20000000-0000-0000-0000-000000000001");
        public static readonly Guid ShopOwner = Guid.Parse("20000000-0000-0000-0000-000000000002");
        public static readonly Guid Customer = Guid.Parse("20000000-0000-0000-0000-000000000003");
    }

    public static class Users
    {
        public static readonly string Admin = "10000000-0000-0000-0000-000000000001";
        public static readonly string Owner = "10000000-0000-0000-0000-000000000002";
        public static readonly string Staff = "10000000-0000-0000-0000-000000000003";
    }

    public static class TestAccountIds
    {
        public static readonly Guid Admin = Guid.Parse("30000000-0000-0000-0000-000000000001");
        public static readonly Guid ShopOwner = Guid.Parse("30000000-0000-0000-0000-000000000002");
        public static readonly Guid AdminCustomer = Guid.Parse("30000000-0000-0000-0000-000000000003");
        public static readonly Guid OwnerCustomer = Guid.Parse("30000000-0000-0000-0000-000000000004");
        public static readonly Guid StaffCustomer = Guid.Parse("30000000-0000-0000-0000-000000000005");
    }

    public static async Task SeedAccountsDataAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();

        await SeedRolesAsync(db);
        await SeedAccountsAsync(db);
    }

    private static async Task SeedRolesAsync(AccountsDbContext db)
    {
        if (await db.Roles.AnyAsync())
            return;

        var allPermissions = new List<string>();
        allPermissions.AddRange(Contracts.AccountsPermissions.All);

        // Collect permissions from other modules via reflection-safe constants
        allPermissions.AddRange(new[]
        {
            "orders.create", "orders.read", "orders.update", "orders.cancel", "orders.ship", "orders.deliver",
            "products.create", "products.read", "products.update", "products.delete"
        });

        var adminRole = Role.Create(
            "Admin",
            allPermissions,
            null,
            TenantType.System,
            Guid.Empty).Value;

        var shopOwnerPermissions = new List<string>
        {
            "products.create", "products.read", "products.update", "products.delete",
            "orders.read", "orders.update", "orders.ship", "orders.deliver"
        };

        var shopOwnerRole = Role.Create(
            "ShopOwner",
            shopOwnerPermissions,
            null,
            TenantType.System,
            Guid.Empty).Value;

        var customerPermissions = new List<string>
        {
            "orders.create", "orders.read", "orders.cancel",
            "products.read"
        };

        var customerRole = Role.Create(
            "Customer",
            customerPermissions,
            null,
            TenantType.System,
            Guid.Empty).Value;

        db.Roles.AddRange(adminRole, shopOwnerRole, customerRole);

        // Clear domain events to prevent publishing during seeding
        foreach (var entry in db.ChangeTracker.Entries<AggregateRoot>())
            entry.Entity.PopDomainEvents();

        await db.SaveChangesAsync();
    }

    private static async Task SeedAccountsAsync(AccountsDbContext db)
    {
        if (await db.Accounts.AnyAsync())
            return;

        var adminRole = await db.Roles.FirstAsync(r => r.Name == "Admin");
        var shopOwnerRole = await db.Roles.FirstAsync(r => r.Name == "ShopOwner");
        var customerRole = await db.Roles.FirstAsync(r => r.Name == "Customer");

        var adminAccount = Account.Create(
            Guid.Parse(Users.Admin),
            "System Admin",
            "admin@talabat.com",
            null,
            TenantType.System);

        adminAccount.UpdateAssignments([adminRole.Id], Guid.Empty);

        var shopOwnerAccount = Account.Create(
            Guid.Parse(Users.Owner),
            "Shop Owner",
            "owner@talabat.com",
            Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), // AlBaik shop ID
            TenantType.Shop);

        shopOwnerAccount.UpdateAssignments([shopOwnerRole.Id], Guid.Empty);

        // Customer accounts for all users
        var adminCustomerAccount = Account.Create(
            Guid.Parse(Users.Admin),
            "System Admin",
            "admin@talabat.com",
            null,
            TenantType.System);

        adminCustomerAccount.UpdateAssignments([customerRole.Id], Guid.Empty);

        var ownerCustomerAccount = Account.Create(
            Guid.Parse(Users.Owner),
            "Shop Owner",
            "owner@talabat.com",
            null,
            TenantType.System);

        ownerCustomerAccount.UpdateAssignments([customerRole.Id], Guid.Empty);

        var staffCustomerAccount = Account.Create(
            Guid.Parse(Users.Staff),
            "Staff Member",
            "staff@talabat.com",
            null,
            TenantType.System);

        staffCustomerAccount.UpdateAssignments([customerRole.Id], Guid.Empty);


        db.Accounts.AddRange(adminAccount, shopOwnerAccount, adminCustomerAccount, ownerCustomerAccount, staffCustomerAccount);

        // Clear domain events to prevent publishing during seeding
        foreach (var entry in db.ChangeTracker.Entries<AggregateRoot>())
            entry.Entity.PopDomainEvents();

        await db.SaveChangesAsync();
    }
}

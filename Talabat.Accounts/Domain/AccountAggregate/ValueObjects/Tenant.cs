using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Domain.AccountAggregate.ValueObjects;

internal class Tenant : ValueObject
{
    public Guid? TenantId { get; private set; }
    public TenantType TenantType { get; private set; }

    internal Tenant(Guid? tenantId, TenantType tenantType)
    {
        TenantId = tenantId;
        TenantType = tenantType;
    }

    internal static Tenant Create(Guid? tenantId, TenantType tenantType)
    {
        return new Tenant(tenantId, tenantType);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return TenantId ?? Guid.Empty;
        yield return TenantType;
    }

    private Tenant() { }
}

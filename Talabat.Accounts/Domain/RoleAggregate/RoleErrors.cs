using ErrorOr;

namespace Talabat.Accounts.Domain.RoleAggregate;

internal static class RoleErrors
{
    public static Error NotFound =>
        Error.NotFound(
            code: "Role.NotFound",
            description: "Role was not found.");

    public static Error DuplicateName =>
        Error.Conflict(
            code: "Role.DuplicateName",
            description: "A role with this name already exists in this tenant.");

    public static Error InvalidTenantType =>
        Error.Validation(
            code: "Role.InvalidTenantType",
            description: "Role tenant type must be System or Shop.");

    public static Error InUse =>
        Error.Conflict(
            code: "Role.InUse",
            description: "Cannot delete a role that is assigned to accounts.");
}

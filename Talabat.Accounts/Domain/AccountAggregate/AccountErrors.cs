using ErrorOr;

namespace Talabat.Accounts.Domain.AccountAggregate;

internal static class AccountErrors
{
    public static Error NotFound =>
        Error.NotFound(
            code: "Account.NotFound",
            description: "Account was not found.");

    public static Error AlreadyDeleted =>
        Error.Conflict(
            code: "Account.AlreadyDeleted",
            description: "Account has already been removed.");

    }

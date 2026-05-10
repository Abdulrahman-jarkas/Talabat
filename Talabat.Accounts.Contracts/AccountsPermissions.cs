namespace Talabat.Accounts.Contracts;

public static class AccountsPermissions
{
    private const string Module = "accounts";

    public const string AddAccount = $"{Module}.add";
    public const string UpdateAccount = $"{Module}.update";

    public const string RemoveAccount = $"{Module}.remove";
    public const string ViewAccounts = $"{Module}.view";

    public const string AddRole = $"{Module}.roles.add";
    public const string EditRole = $"{Module}.roles.edit";
    public const string RemoveRole = $"{Module}.roles.remove";
    public const string ViewRoles = $"{Module}.roles.view";

    public static IReadOnlyList<string> All =>
        [AddAccount, UpdateAccount, RemoveAccount, ViewAccounts, AddRole, EditRole, RemoveRole, ViewRoles];
}

using ErrorOr;
using MediatR;

namespace Talabat.Accounts.Application.Account.Commands.UpdateAccount;

internal record UpdateAccountCommand(
    Guid AccountId,
    string Name,
    List<Guid> RoleIds,
    Guid AssignedBy,
    Guid? TenantId) : IRequest<ErrorOr<Success>>;

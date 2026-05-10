using ErrorOr;
using MediatR;

namespace Talabat.Accounts.Application.Account.Commands.UpdateAccount;

internal record UpdateAccountCommand(
    Guid AccountId,
    List<Guid> RoleIds,
    Guid AssignedBy,
    Guid? TenantId,
    string Version) : IRequest<ErrorOr<MutationResult>>;

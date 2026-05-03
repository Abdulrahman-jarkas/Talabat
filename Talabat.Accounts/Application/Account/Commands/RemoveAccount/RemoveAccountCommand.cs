using ErrorOr;
using MediatR;

namespace Talabat.Accounts.Application.Account.Commands.RemoveAccount;

internal record RemoveAccountCommand(Guid AccountId, Guid? TenantId) : IRequest<ErrorOr<Success>>;

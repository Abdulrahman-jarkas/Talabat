using ErrorOr;
using MediatR;

namespace Talabat.Accounts.Application.Account.Commands.RemoveAccount;

internal record RemoveAccountCommand(Guid AccountId, Guid? TenantId, string Version) : IRequest<ErrorOr<Success>>;

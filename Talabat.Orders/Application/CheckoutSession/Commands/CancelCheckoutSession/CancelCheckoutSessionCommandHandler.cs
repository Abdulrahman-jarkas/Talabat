using ErrorOr;
using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.CheckoutSession.Commands.CancelCheckoutSession;

internal class CancelCheckoutSessionCommandHandler(ICheckoutSessionRepository checkoutSessionRepository)
	: IRequestHandler<CancelCheckoutSessionCommand, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(CancelCheckoutSessionCommand command, CancellationToken cancellationToken)
	{
		var session = await checkoutSessionRepository.GetByIdAsync(command.CheckoutSessionId, cancellationToken);

		if (session is null)
			return CheckoutSessionErrors.NotFound;

		var result = session.Cancel();
		if (result.IsError)
			return result.Errors;

		using var scope = ModuleTransactionScope.Create();

		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);

		scope.Complete();

		return Result.Success;
	}
}

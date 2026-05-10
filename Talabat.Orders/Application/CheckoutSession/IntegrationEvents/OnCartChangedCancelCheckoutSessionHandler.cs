using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.SharedKernal;
using Talabat.SharedKernal.IntegrationEvents;

namespace Talabat.Orders.Application.CheckoutSession.IntegrationEvents;

internal class OnCartChangedCancelCheckoutSessionHandler(
    ICheckoutSessionRepository checkoutSessionRepository) : INotificationHandler<CartChangedIntegrationEvent>
{
    public async Task Handle(CartChangedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var activeSession = await checkoutSessionRepository.GetActiveByCustomerIdAsync(
            notification.CustomerId,
            cancellationToken);

        if (activeSession is null)
            return;

        var cancelResult = activeSession.Cancel();

        if (cancelResult.IsError)
            throw new EventualConsistencyException(
                EventualConsistencyError.From(
                    "CartChanged.SessionCancellationFailed",
                    $"Could not cancel session {activeSession.Id} for Customer {notification.CustomerId}."),
                cancelResult.Errors);

        using var scope = ModuleTransactionScope.Create();

        await checkoutSessionRepository.SaveChangesAsync(cancellationToken);

        scope.Complete();
    }
}

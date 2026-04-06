using MediatR;
using Microsoft.Extensions.Logging;
using Talabat.Orders.Data.Repositories;
using Talabat.Payments.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.CheckoutSession.IntegrationEvents;

internal class OnPaymentFailedCancelSessionHandler(
    ICheckoutSessionRepository checkoutSessionRepository,
    ILogger<OnPaymentFailedCancelSessionHandler> logger) : INotificationHandler<PaymentFailedEvent>
{
    public async Task Handle(PaymentFailedEvent notification, CancellationToken cancellationToken)
    {
        var session = await checkoutSessionRepository.GetByPaymentIdAsync(notification.PaymentId, cancellationToken);

        if (session is null)
        {
            logger.LogWarning(
                "[PaymentFailed] No checkout session found for PaymentId {PaymentId}.",
                notification.PaymentId);
            return;
        }

        // Idempotency: payment failed only applies to CheckedOut sessions — skip if already transitioned
        if (!session.Lifetime.IsCheckedOut)
        {
            logger.LogInformation(
                "[PaymentFailed] Session {SessionId} is already inactive. Skipping.",
                session.Id);
            return;
        }

        var closeResult = session.Close();
        if (closeResult.IsError)
            throw new EventualConsistencyException(
                EventualConsistencyError.From(
                    "PaymentFailed.CloseSessionFailed",
                    $"Failed to close session {session.Id} after payment failure for PaymentId {notification.PaymentId}."),
                closeResult.Errors);

        // Close raises CheckoutSessionClosedEvent ? removes reservations atomically
        using var scope = ModuleTransactionScope.Create();

        await checkoutSessionRepository.SaveChangesAsync(cancellationToken);

        scope.Complete();

        logger.LogInformation(
            "[PaymentFailed] Session {SessionId} closed for PaymentId {PaymentId}. Reason: {Reason}",
            session.Id,
            notification.PaymentId,
            notification.Reason);
    }
}

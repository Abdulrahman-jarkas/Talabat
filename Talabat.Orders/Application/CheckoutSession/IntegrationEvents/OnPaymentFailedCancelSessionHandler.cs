using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate;
using Talabat.Payments.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.CheckoutSession.IntegrationEvents;

internal class OnPaymentFailedCancelSessionHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	ILogger<OnPaymentFailedCancelSessionHandler> logger) : INotificationHandler<PaymentFailedEvent>
{
	public async Task Handle(PaymentFailedEvent notification, CancellationToken cancellationToken)
	{
		try
		{
			var checkoutSession = await checkoutSessionRepository.GetByPaymentIdAsync(
				notification.PaymentId,
				cancellationToken)
				?? throw new EventualConsistencyException(
					EventualConsistencyError.From(
						"PaymentFailed.SessionNotFound",
						$"No checkout session found for PaymentId {notification.PaymentId}."));

			// Idempotency: already cancelled, nothing to do
			if (checkoutSession.Lifetime.StoredStatus == CheckoutSessionStatusValues.Cancelled)
				return;

			var cancelResult = checkoutSession.Cancel();

			if (cancelResult.IsError)
				throw new EventualConsistencyException(
					EventualConsistencyError.From(
						"PaymentFailed.SessionCancellationFailed",
						$"Failed to cancel session {checkoutSession.Id} for PaymentId {notification.PaymentId}."),
					cancelResult.Errors);

			// Cancel() raises CheckoutSessionCancelledEvent, which is dispatched during SaveChangesAsync.
			// OnCheckoutSessionCancelledRemoveReservationsHandler then removes reservations via Products module.
			await checkoutSessionRepository.SaveChangesAsync(cancellationToken);
		}
		catch (DbUpdateConcurrencyException)
		{
			logger.LogInformation(
				"[PaymentFailed] Concurrency conflict for PaymentId {PaymentId}; another handler already processed it.",
				notification.PaymentId);
		}
	}
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Talabat.Checkout.Data.Repositories;
using Talabat.Checkout.Domain.CheckoutSessionAggregate;
using Talabat.Payments.Contracts;

namespace Talabat.Checkout.Integration;

internal class OnPaymentFailedEventHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	ILogger<OnPaymentFailedEventHandler> logger) : INotificationHandler<PaymentFailedEvent>
{
	public async Task Handle(PaymentFailedEvent notification, CancellationToken cancellationToken)
	{
		try
		{
			var checkoutSession = await checkoutSessionRepository.GetByPaymentIdAsync(
				notification.PaymentId,
				cancellationToken);

			if (checkoutSession is null)
			{
				logger.LogError(
					"[PaymentFailed] No checkout session found for PaymentId {PaymentId}.",
					notification.PaymentId);
				return;
			}

			// Idempotency: already cancelled, nothing to do
			if (checkoutSession.Lifetime.StoredStatus == CheckoutSessionStatusValues.Cancelled)
				return;

			var cancelResult = checkoutSession.Cancel();

			if (cancelResult.IsError)
			{
				logger.LogError(
					"[PaymentFailed] Failed to cancel session {SessionId} for PaymentId {PaymentId}. Errors: {Errors}",
					checkoutSession.Id,
					notification.PaymentId,
					string.Join(", ", cancelResult.Errors.Select(e => e.Description)));
				return;
			}

			// Cancel() raises CheckoutSessionCancelledEvent, which is dispatched during SaveChangesAsync.
			// OnCheckoutSessionCancelledEventHandler then releases reservations within the same transaction.
			await checkoutSessionRepository.SaveChangesAsync(cancellationToken);
		}
		catch (DbUpdateConcurrencyException)
		{
			logger.LogInformation(
				"[PaymentFailed] Session was modified concurrently for PaymentId {PaymentId}; skipping.",
				notification.PaymentId);
		}
		catch (Exception ex)
		{
			logger.LogError(ex,
				"[PaymentFailed] Unexpected error handling PaymentId {PaymentId}.",
				notification.PaymentId);
		}
	}
}

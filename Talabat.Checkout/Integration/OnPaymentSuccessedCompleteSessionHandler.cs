using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Talabat.Checkout.Data.Repositories;
using Talabat.Checkout.Domain.CheckoutSessionAggregate;
using Talabat.Payments.Contracts;

namespace Talabat.Checkout.Integration;

internal class OnPaymentSuccessedCompleteSessionHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	ILogger<OnPaymentSuccessedCompleteSessionHandler> logger) : INotificationHandler<PaymentSuccessedEvent>
{
	public async Task Handle(PaymentSuccessedEvent notification, CancellationToken cancellationToken)
	{
		try
		{
			var session = await checkoutSessionRepository.GetByPaymentIdAsync(
				notification.PaymentId,
				cancellationToken);

			if (session is null)
			{
				logger.LogError(
					"[CompleteSession] No checkout session found for PaymentId {PaymentId}.",
					notification.PaymentId);
				return;
			}

			// Idempotency: already completed, nothing to do
			if (session.Lifetime.StoredStatus == CheckoutSessionStatusValues.Completed)
				return;

			var result = session.Complete();

			if (result.IsError)
			{
				logger.LogError(
					"[CompleteSession] Failed to complete session {SessionId}. Errors: {Errors}",
					session.Id,
					string.Join(", ", result.Errors.Select(e => e.Description)));
				return;
			}

			await checkoutSessionRepository.SaveChangesAsync(cancellationToken);
		}
		catch (DbUpdateConcurrencyException)
		{
			// Another handler already processed this session — safe to skip
			logger.LogInformation(
				"[CompleteSession] Session was modified concurrently for PaymentId {PaymentId}; skipping.",
				notification.PaymentId);
		}
		// No catch(Exception) — let failures propagate so the webhook returns 500 and the gateway retries
	}
}

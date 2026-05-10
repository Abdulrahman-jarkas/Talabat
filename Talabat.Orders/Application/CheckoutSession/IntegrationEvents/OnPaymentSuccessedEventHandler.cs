using MediatR;
using Microsoft.Extensions.Logging;
using Talabat.Orders.Data.Repositories;
using Talabat.Payments.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.CheckoutSession.IntegrationEvents;

internal class OnPaymentSuccessedEventHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	ILogger<OnPaymentSuccessedEventHandler> logger) : INotificationHandler<PaymentSuccessedEvent>
{
	public async Task Handle(PaymentSuccessedEvent notification, CancellationToken cancellationToken)
	{
		var session = await checkoutSessionRepository.GetByPaymentIdAsync(
			notification.PaymentId,
			cancellationToken)
			?? throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"PaymentSuccess.SessionNotFound",
					$"No checkout session found for PaymentId {notification.PaymentId}."));

		var completeResult = session.Complete();
		if (completeResult.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"PaymentSuccess.CompletionFailed",
					$"Failed to complete checkout session for PaymentId {notification.PaymentId}."),
				completeResult.Errors);

		using var scope = ModuleTransactionScope.Create();

		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);

		scope.Complete();

		logger.LogInformation(
			"[PaymentSuccess] Session {SessionId} completed for PaymentId {PaymentId}.",
			session.Id,
			notification.PaymentId);
	}
}


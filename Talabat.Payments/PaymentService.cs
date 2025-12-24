using ErrorOr;
using MediatR;
using Talabat.Payments.Contracts;

namespace Talabat.Payments;

public class PaymentService(IPaymentsRepository paymentsRepository, ISender sender, IPublisher publisher) : IPaymentService
{
	public async Task<ErrorOr<CreatePaymentSessionResponse>> CreatePaymentSession(Guid customerId, Guid checkoutSessionId, decimal amount)
	{
		var payment = new Payment(Guid.NewGuid(), "", customerId, checkoutSessionId);

		await paymentsRepository.AddPaymentAsync(payment);
		await paymentsRepository.SaveChangesAsync();

		return new CreatePaymentSessionResponse() { PaymentId = payment.Id, PaymentUrl = payment.Url };
	}

	public async Task<ErrorOr<Success>> OnPaymentFailed(Guid paymentId)
	{
		var payment = await paymentsRepository.GetPaymentByIdAsync(paymentId);

		if (payment is null)
			return Error.Validation(
					code: "Payment.NotFound",
					description: $"Payment with id {paymentId} was not found."
				);

		if(payment.Status != PaymentStatus.Pending)
			return Error.Validation(
					code: "Payment.InvalidStatus",
					description: $"Payment with id {paymentId} has invalid status {payment.Status.ToString()}."
				);

		payment.SetStatus(PaymentStatus.Failed);

		await paymentsRepository.SaveChangesAsync();

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> OnPaymentRefundFailed(Guid paymentId)
	{
		var payment = await paymentsRepository.GetPaymentByIdAsync(paymentId);

		if (payment is null)
			return Error.Validation(
					code: "Payment.NotFound",
					description: $"Payment with id {paymentId} was not found."
				);

		if (payment.Status != PaymentStatus.Refunding)
			return Error.Validation(
					code: "Payment.InvalidStatus",
					description: $"Payment with id {paymentId} has invalid status {payment.Status.ToString()}."
				);

		payment.SetStatus(PaymentStatus.RefundFailed);
		await paymentsRepository.SaveChangesAsync();

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> OnPaymentRefundSuccess(Guid paymentId)
	{
		var payment = await paymentsRepository.GetPaymentByIdAsync(paymentId);

		if (payment is null)
			return Error.Validation(
					code: "Payment.NotFound",
					description: $"Payment with id {paymentId} was not found."
				);

		if (payment.Status != PaymentStatus.Refunding)
			return Error.Validation(
					code: "Payment.InvalidStatus",
					description: $"Payment with id {paymentId} has invalid status {payment.Status.ToString()}."
				);

		payment.SetStatus(PaymentStatus.Refunded);
		await paymentsRepository.SaveChangesAsync();

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> OnPaymentSuccess(Guid paymentId)
	{
		var payment = await paymentsRepository.GetPaymentByIdAsync(paymentId);

		if (payment is null)
			return Error.Validation(
					code: "Payment.NotFound",
					description: $"Payment with id {paymentId} was not found."
				);

		if (payment.Status != PaymentStatus.Pending)
			return Error.Validation(
					code: "Payment.InvalidStatus",
					description: $"Payment with id {paymentId} has invalid status {payment.Status.ToString()}.");

		payment.SetStatus(PaymentStatus.Paid);
		await paymentsRepository.SaveChangesAsync();

		await publisher.Publish(new PaymentSuccessedEvent(payment.Id, payment.CustomerId));

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> Refund(Guid paymentId)
	{
		var payment = await paymentsRepository.GetPaymentByIdAsync(paymentId);

		if (payment is null)
			return Error.Validation(
					code: "Payment.NotFound",
					description: $"Payment with id {paymentId} was not found."
				);

		if (payment.Status != PaymentStatus.Paid)
			return Error.Validation(
					code: "Payment.InvalidStatus",
					description: $"Payment with id {paymentId} has invalid status {payment.Status.ToString()}."
				);

		payment.SetStatus(PaymentStatus.Refunding);
		await paymentsRepository.SaveChangesAsync();

		return Result.Success;
	}
}

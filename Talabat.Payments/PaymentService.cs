
using ErrorOr;

namespace Talabat.Payments;

public class PaymentService(IPaymentsRepository paymentsRepository) : IPaymentService
{
	public async Task<ErrorOr<CreatePaymentSessionResponse>> CreatePaymentSession(int orderId, double amount)
	{
		await paymentsRepository.AddPaymentAsync(new Payment(Guid.NewGuid(), ""));

		return new CreatePaymentSessionResponse() { PaymentId = Guid.NewGuid(), PaymentUrl = "" };
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
					description: $"Payment with id {paymentId} has invalid status {payment.Status.ToString()}."
				);

		payment.SetStatus(PaymentStatus.Paid);
		await paymentsRepository.SaveChangesAsync();

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

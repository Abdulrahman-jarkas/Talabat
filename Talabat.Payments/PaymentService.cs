using ErrorOr;
using MediatR;
using Talabat.Payments.Contracts;

namespace Talabat.Payments;

public class PaymentService(IPaymentsRepository paymentsRepository, ISender sender, IPublisher publisher) : IPaymentService
{
	public async Task<ErrorOr<CreatePaymentSessionResponse>> CreatePaymentSession(Guid customerId, Guid checkoutSessionId, decimal amount)
	{
		var paymentId = Guid.NewGuid();
		var paymentUrl = $"https://payment-simulator.local/pay/{paymentId}";
		var payment = new Payment(paymentId, paymentUrl, customerId, checkoutSessionId, amount);

		await paymentsRepository.AddPaymentAsync(payment);
		await paymentsRepository.SaveChangesAsync();

		return new CreatePaymentSessionResponse() { PaymentId = payment.Id, PaymentUrl = payment.Url };
	}

	public async Task<ErrorOr<Success>> OnPaymentFailed(Guid paymentId)
	{
		var payment = await paymentsRepository.GetPaymentByIdAsync(paymentId);

		if (payment is null)
			return PaymentErrors.NotFound(paymentId);

		if(payment.Status != PaymentStatus.Pending)
			return PaymentErrors.InvalidStatus(paymentId, payment.Status);

		payment.SetStatus(PaymentStatus.Failed);

		await paymentsRepository.SaveChangesAsync();

		await publisher.Publish(new PaymentFailedEvent(payment.Id, payment.CustomerId, "Payment failed"));

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> OnPaymentRefundFailed(Guid paymentId)
	{
		var payment = await paymentsRepository.GetPaymentByIdAsync(paymentId);

		if (payment is null)
			return PaymentErrors.NotFound(paymentId);

		if (payment.Status != PaymentStatus.Refunding)
			return PaymentErrors.InvalidStatus(paymentId, payment.Status);

		payment.SetStatus(PaymentStatus.RefundFailed);
		await paymentsRepository.SaveChangesAsync();

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> OnPaymentRefundSuccess(Guid paymentId)
	{
		var payment = await paymentsRepository.GetPaymentByIdAsync(paymentId);

		if (payment is null)
			return PaymentErrors.NotFound(paymentId);

		if (payment.Status != PaymentStatus.Refunding)
			return PaymentErrors.InvalidStatus(paymentId, payment.Status);

		payment.SetStatus(PaymentStatus.Refunded);
		await paymentsRepository.SaveChangesAsync();

		await publisher.Publish(new PaymentRefundedEvent(payment.Id, payment.CustomerId));

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> OnPaymentSuccess(Guid paymentId)
	{
		var payment = await paymentsRepository.GetPaymentByIdAsync(paymentId);

		if (payment is null)
			return PaymentErrors.NotFound(paymentId);

		if (payment.Status != PaymentStatus.Pending)
			return PaymentErrors.InvalidStatus(paymentId, payment.Status);

		payment.SetStatus(PaymentStatus.Paid);
		await paymentsRepository.SaveChangesAsync();

		await publisher.Publish(new PaymentSuccessedEvent(payment.Id, payment.CustomerId));

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> Refund(Guid paymentId)
	{
		var payment = await paymentsRepository.GetPaymentByIdAsync(paymentId);

		if (payment is null)
			return PaymentErrors.NotFound(paymentId);

		if (payment.Status != PaymentStatus.Paid)
			return PaymentErrors.InvalidStatus(paymentId, payment.Status);

		payment.SetStatus(PaymentStatus.Refunding);
		await paymentsRepository.SaveChangesAsync();

		return Result.Success;
	}
}

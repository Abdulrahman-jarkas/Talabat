using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate;

internal class Payment : ValueObject
{
	public PaymentMethodValues Method { get; init; }
	public PaymentStatusValues Status { get; private set; }

	public Guid? PaymentId { get; private set; } = null;

	private Payment(
		PaymentMethodValues paymentMethod,
		PaymentStatusValues paymentStatus,
		Guid? paymentId)
	{
		Status = Guard.Against.EnumOutOfRange(paymentStatus);
		Method = Guard.Against.EnumOutOfRange(paymentMethod);
		PaymentId = paymentId;
	}

	public static Payment Card(Guid paymentId, PaymentStatusValues paymentStatus)
	{
		Guard.Against.Default(paymentId, nameof(paymentId));
		return new Payment(PaymentMethodValues.Card, paymentStatus, paymentId);
	}

	public static Payment Cash(PaymentStatusValues paymentStatus)
	{
		return new Payment(PaymentMethodValues.Cash, paymentStatus, null);
	}

	public ErrorOr<Payment> Pay()
	{
		if (Status != PaymentStatusValues.Paid)
			return new Payment(Method, PaymentStatusValues.Paid, PaymentId);

		return Error.Conflict("Payment.Invalid", "This operation is in valid");
	}


	public ErrorOr<Payment> Refund()
	{
		if (Status == PaymentStatusValues.Paid && Method == PaymentMethodValues.Card)
			return new Payment(Method, PaymentStatusValues.Refunded, PaymentId);

		return Error.Conflict("Payment.Invalid", "This operation is in valid");
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return Method;
		yield return Status;
		if (PaymentId is not null)
			yield return PaymentId;
	}
}

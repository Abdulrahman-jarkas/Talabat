using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate;

internal class Payment : ValueObject
{
	public PaymentStatusValues Status { get; private set; }

	public Guid PaymentId { get; private set; }

	private Payment(
		Guid paymentId,
		PaymentStatusValues paymentStatus)
	{
		PaymentId = Guard.Against.Default(paymentId, nameof(paymentId));
		Status = Guard.Against.EnumOutOfRange(paymentStatus);
	}

	public static Payment Create(Guid paymentId)
	{
		return new Payment(paymentId, PaymentStatusValues.Paid);
	}

	public ErrorOr<Payment> Refund()
	{
		if (Status == PaymentStatusValues.Paid)
			return new Payment(PaymentId, PaymentStatusValues.Refunded);

		return Error.Conflict("Payment.Invalid", "Only paid orders can be refunded.");
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return Status;
		yield return PaymentId;
	}
}

namespace Talabat.Payments;

public class Payment
{
	public Guid Id { get; init; }
	public string Url { get; init; } = string.Empty;
	public PaymentStatus Status { get; private set; }
	public Guid CustomerId { get; init; }
	public Guid CheckoutSessionId { get; init; }

	public Payment(Guid id, string url, Guid customerId, Guid checkoutSessionId)
	{
		Id = id;
		Url = url;
		CustomerId = customerId;
		CheckoutSessionId = checkoutSessionId;
		Status = PaymentStatus.Pending;
	}

	public void SetStatus(PaymentStatus status)
	{
		Status = status;
	}
}

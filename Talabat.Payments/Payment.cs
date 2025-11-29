namespace Talabat.Payments;

public class Payment
{
	public Guid Id { get; init; }
	public string Url { get; init; } = string.Empty;
	public PaymentStatus Status { get; private set; }

	public Payment(Guid id, string url)
	{
		Id = id;
		Url = url;
		Status = PaymentStatus.Pending;
	}

	public void SetStatus(PaymentStatus status)
	{
		Status = status;
	}
}

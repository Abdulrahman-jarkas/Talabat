namespace Talabat.Payments;

public class CreatePaymentSessionResponse
{
	public string PaymentUrl { get; set; } = string.Empty;
	public Guid PaymentId { get; set; }
}

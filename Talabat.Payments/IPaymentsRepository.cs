
namespace Talabat.Payments
{
	public interface IPaymentsRepository
	{
		Task AddPaymentAsync(Payment payment);
		Task<Payment?> GetPaymentByIdAsync(Guid paymentId);
		Task SaveChangesAsync();
	}
}
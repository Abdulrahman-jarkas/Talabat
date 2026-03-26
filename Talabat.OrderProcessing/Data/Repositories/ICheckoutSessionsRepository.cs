using Talabat.OrderProcessing.Domain.CheckoutSessionAggregate;

namespace Talabat.OrderProcessing.Data.Repositories
{
	public interface ICheckoutSessionsRepository
	{
		Task<CheckoutSession> CreateAsync(CheckoutSession session, CancellationToken cancellationToken = default);
		Task<CheckoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<CheckoutSession?> GetCheckoutSessionByPaymentIdAsync(Guid paymentId);
		Task SaveChangesAsync(CancellationToken cancellationToken = default);
		Task UpdateAsync(CheckoutSession session, CancellationToken cancellationToken = default);
	}
}
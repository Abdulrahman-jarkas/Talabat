using Talabat.Checkout.Domain.CheckoutSessionAggregate;

namespace Talabat.Checkout.Data.Repositories;

internal interface ICheckoutSessionRepository
{
	Task AddAsync(CheckoutSession checkoutSession, CancellationToken cancellationToken = default);
	Task<CheckoutSession?> GetByIdAsync(Guid checkoutSessionId, CancellationToken cancellationToken = default);
	Task<CheckoutSession?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
	Task<CheckoutSession?> GetActiveByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
	Task<List<CheckoutSession>> GetActiveSessionsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
	Task<List<CheckoutSession>> GetOverdueActiveSessionsAsync(CancellationToken cancellationToken = default);
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

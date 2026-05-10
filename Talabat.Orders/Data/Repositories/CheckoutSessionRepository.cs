using Microsoft.EntityFrameworkCore;
using Talabat.Orders.Domain.CheckoutSessionAggregate;

namespace Talabat.Orders.Data.Repositories;

internal class CheckoutSessionRepository(OrdersDbContext context) : ICheckoutSessionRepository
{
	public Task AddAsync(CheckoutSession checkoutSession, CancellationToken cancellationToken = default)
	{
		return context.CheckoutSessions.AddAsync(checkoutSession, cancellationToken).AsTask();
	}

	public Task<CheckoutSession?> GetByIdAsync(Guid checkoutSessionId, CancellationToken cancellationToken = default)
	{
		return context.CheckoutSessions
			.Include(cs => cs.Items)
			.FirstOrDefaultAsync(cs => cs.Id == checkoutSessionId, cancellationToken);
	}

	public Task<CheckoutSession?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
	{
		return context.CheckoutSessions
			.Include(cs => cs.Items)
			.FirstOrDefaultAsync(cs => cs.PaymentId == paymentId, cancellationToken);
	}

	public Task<CheckoutSession?> GetActiveByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
	{
		return context.CheckoutSessions
			.Include(cs => cs.Items)
			.FirstOrDefaultAsync(cs => cs.CustomerId == customerId
				&& cs.Lifetime.StoredStatus == CheckoutSessionStatusValues.Active, cancellationToken);
	}

	public Task<List<CheckoutSession>> GetActiveSessionsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
	{
		return context.CheckoutSessions
			.Include(cs => cs.Items)
			.Where(cs => cs.Lifetime.StoredStatus == CheckoutSessionStatusValues.Active
				&& cs.Items.Any(i => i.ProductId == productId))
			.ToListAsync(cancellationToken);
	}

	public Task<List<CheckoutSession>> GetOverdueActiveSessionsAsync(CancellationToken cancellationToken = default)
	{
		return context.CheckoutSessions
			.Include(cs => cs.Items)
			.Where(cs => cs.Lifetime.StoredStatus == CheckoutSessionStatusValues.Active
				&& cs.Lifetime.ExpiresAt <= DateTime.UtcNow)
			.ToListAsync(cancellationToken);
	}

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return context.SaveChangesAsync(cancellationToken);
	}
}

using Microsoft.EntityFrameworkCore;
using Talabat.Checkout.Domain.CheckoutSessionAggregate;

namespace Talabat.Checkout.Data.Repositories;

internal class CheckoutSessionRepository(CheckoutDbContext context) : ICheckoutSessionRepository
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

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return context.SaveChangesAsync(cancellationToken);
	}
}

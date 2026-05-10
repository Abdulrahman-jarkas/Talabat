using Microsoft.EntityFrameworkCore;
using Talabat.OrderProcessing.Domain.CheckoutSessionAggregate;

namespace Talabat.OrderProcessing.Data.Repositories;

public class CheckoutSessionsRepository(OrderProcessingDbContext context) : ICheckoutSessionsRepository
{

	public async Task<CheckoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await context.CheckoutSessions
			.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
	}

	public async Task<CheckoutSession> CreateAsync(CheckoutSession session, CancellationToken cancellationToken = default)
	{
	    await context.CheckoutSessions.AddAsync(session, cancellationToken);
		return session;
	}

	public async Task UpdateAsync(CheckoutSession session, CancellationToken cancellationToken = default)
	{
		context.CheckoutSessions.Update(session);
		await Task.CompletedTask;
	}

	public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		await context.SaveChangesAsync(cancellationToken);
	}

	public Task<CheckoutSession?> GetCheckoutSessionByPaymentIdAsync(Guid paymentId)
	{
		return context.CheckoutSessions
			.FirstOrDefaultAsync(cs => cs.PaymentId == paymentId);
	}
}
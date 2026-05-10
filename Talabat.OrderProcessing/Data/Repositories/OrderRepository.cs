using Microsoft.EntityFrameworkCore;
using Talabat.OrderProcessing.Domain.OrderAggregate;

namespace Talabat.OrderProcessing.Data.Repositories;

public class OrderRepository(OrderProcessingDbContext context) : IOrderRepository
{

	public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await context.Orders
			.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
	}

	public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
	{
		await context.Orders.AddAsync(order, cancellationToken);
		return order;
	}

	public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
	{
		context.Orders.Update(order);
		await Task.CompletedTask;
	}

	public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		await context.SaveChangesAsync(cancellationToken);
	}
}
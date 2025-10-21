using Talabat.OrderProcessing.Domain.OrderAggregate;

namespace Talabat.OrderProcessing.Data.Repositories
{
	public interface IOrderRepository
	{
		Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
		Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
		Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
		Task SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
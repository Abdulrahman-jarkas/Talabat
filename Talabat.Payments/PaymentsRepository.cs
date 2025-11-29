using Microsoft.EntityFrameworkCore;

namespace Talabat.Payments;

public class PaymentsRepository : IPaymentsRepository
{
	private readonly PaymentsDbContext _dbContext;
	public PaymentsRepository(PaymentsDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public Task AddPaymentAsync(Payment payment)
	{
		return _dbContext.Payments.AddAsync(payment).AsTask();
	}

	public Task<Payment?> GetPaymentByIdAsync(Guid paymentId)
	{
		return _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
	}

	public Task SaveChangesAsync()
	{
		return _dbContext.SaveChangesAsync();
	}
}

using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;
using Talabat.Users.Data;
using Testcontainers.MsSql;
using Xunit;

namespace Talabat.Users.IntegrationTests.Infrastructure;

public class UsersApiFactory : IAsyncLifetime
{
	private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
		.WithPassword("YourStrong@Passw0rd")
		.Build();

	public UsersDbContext DbContext { get; private set; } = null!;
	public ISender MockMediator { get; private set; } = null!;

	public async Task InitializeAsync()
	{
		await _dbContainer.StartAsync();

		var options = new DbContextOptionsBuilder<UsersDbContext>()
			.UseSqlServer(_dbContainer.GetConnectionString())
			.Options;

		DbContext = new UsersDbContext(options);

		await DbContext.Database.EnsureCreatedAsync();

		MockMediator = Substitute.For<ISender>();
	}

	public async Task DisposeAsync()
	{
		await DbContext.DisposeAsync();
		await _dbContainer.DisposeAsync();
	}

	public void SetupProductQuery(Guid productId, ProductResponse? response)
	{
		MockMediator.Send(
			Arg.Is<ProductQuery>(q => q.ProductId == productId),
			Arg.Any<CancellationToken>())
			.Returns(response);
	}

	public void SetupProductsQuery(List<Guid> productIds, List<ProductResponse>? response)
	{
		MockMediator.Send(
			Arg.Is<ProductsQuery>(q => q.ProductIds.SequenceEqual(productIds)),
			Arg.Any<CancellationToken>())
			.Returns(response);
	}

	public void SetupCreatePaymentSession(
		Guid customerId,
		Guid checkoutSessionId,
		decimal amount,
		CreatePaymentSessionResponseDto response)
	{
		MockMediator.Send(
			Arg.Any<CreatePaymentSessionRequest>(),
			Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<ErrorOr<CreatePaymentSessionResponseDto>>(response));
	}

	public void ResetMocks()
	{
		MockMediator.ClearReceivedCalls();
	}

	/// <summary>
	/// Resets the database to a clean state and seeds required test data.
	/// Called before each test to ensure isolation.
	/// </summary>
	public async Task ResetDatabaseAsync()
	{
		// Check if test customer exists and load all related data for proper cascade delete
		var existingCustomer = await DbContext.Customers
			.Include(c => c.Addresses)
			.FirstOrDefaultAsync(c => c.Id == TestConstants.Constants.Customer.Id);

		if (existingCustomer != null)
		{
			// Remove customer - cascade delete will handle all related entities
			// (checkout sessions, checkout items, addresses, etc.)
			// Cart is a JSON column so it's removed with the customer row
			DbContext.Customers.Remove(existingCustomer);
			await DbContext.SaveChangesAsync();
		}

		// Create a fresh customer in initial state
		var customer = new Talabat.Users.Domain.CustomerAggregate.Customer(
			TestConstants.Constants.Customer.Id,
			TestConstants.Constants.Customer.Email);

		DbContext.Customers.Add(customer);
		await DbContext.SaveChangesAsync();

		// Clear change tracker to ensure fresh state for tests
		DbContext.ChangeTracker.Clear();

		// Reset mocks to ensure clean state
		ResetMocks();
	}
}

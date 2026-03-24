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
using Testcontainers.PostgreSql;
using Xunit;

namespace Talabat.Users.IntegrationTests.Infrastructure;

public class UsersApiFactory : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("usersdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    public UsersDbContext DbContext { get; private set; } = null!;
    public ISender MockMediator { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
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
}

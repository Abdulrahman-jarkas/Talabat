using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Talabat.Checkout.Data.Repositories;

namespace Talabat.Checkout.Application.BackgroundServices;

internal class CheckoutSessionExpirationService(
	IServiceScopeFactory scopeFactory,
	ILogger<CheckoutSessionExpirationService> logger) : BackgroundService
{
	private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var timer = new PeriodicTimer(PollInterval);

		while (await timer.WaitForNextTickAsync(stoppingToken))
		{
			try
			{
				await ExpireOverdueSessionsAsync(stoppingToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error while expiring overdue checkout sessions.");
			}
		}
	}

	private async Task ExpireOverdueSessionsAsync(CancellationToken cancellationToken)
	{
		await using var scope = scopeFactory.CreateAsyncScope();

		var repository = scope.ServiceProvider.GetRequiredService<ICheckoutSessionRepository>();

		var overdueSessions = await repository.GetOverdueActiveSessionsAsync(cancellationToken);

		if (overdueSessions.Count == 0)
			return;

		logger.LogInformation("Expiring {Count} overdue checkout session(s).", overdueSessions.Count);

		foreach (var session in overdueSessions)
		{
			var expireResult = session.Expire();

			if (expireResult.IsError)
			{
				logger.LogWarning(
					"Failed to expire checkout session {SessionId}: {Error}",
					session.Id,
					expireResult.Errors.First().Description);
				continue;
			}
		}

		try
		{
			await repository.SaveChangesAsync(cancellationToken);
		}
		catch (DbUpdateConcurrencyException ex)
		{
			logger.LogInformation(ex,
				"Concurrency conflict while expiring sessions; will retry on next poll.");
		}
	}
}

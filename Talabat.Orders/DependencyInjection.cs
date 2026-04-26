using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Orders.Application.BackgroundServices;
using Talabat.Orders.Authorization;
using Talabat.Orders.Data;
using Talabat.Orders.Data.Repositories;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Orders;

public static class DependencyInjection
{
	public static IServiceCollection AddOrdersInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		// Register plan configuration
		OrdersPlanConfiguration.Register();

		services.AddPersistence(configuration)
			.AddMediatR()
			.AddBackgroundServices()
			.AddAuthorization()
			.AddEndpoints();

		return services;
	}

	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<OrdersDbContext>(cfg =>
		{
			cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
		});

		services.AddScoped<IOrdersRepository, OrdersRepository>();
		services.AddScoped<ICheckoutSessionRepository, CheckoutSessionRepository>();

		return services;
	}

	public static IServiceCollection AddMediatR(this IServiceCollection services)
	{
		services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

		return services;
	}

	private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
	{
		services.AddHostedService<CheckoutSessionExpirationService>();

		return services;
	}

	private static IServiceCollection AddAuthorization(this IServiceCollection services)
	{
		services.AddPlanLimitService<OrdersPlanLimitService>();

		return services;
	}

	private static IServiceCollection AddEndpoints(this IServiceCollection services)
	{
		EndpointAssemblyRegistry.Register(typeof(DependencyInjection).Assembly);

		return services;
	}
}

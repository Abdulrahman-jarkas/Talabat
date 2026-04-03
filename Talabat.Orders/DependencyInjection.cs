using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Orders.Application.BackgroundServices;
using Talabat.Orders.Application.Services;
using Talabat.Orders.Data;
using Talabat.Orders.Data.Repositories;

namespace Talabat.Orders;

public static class DependencyInjection
{
	public static IServiceCollection AddOrdersInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddPersistence(configuration)
			.AddMediatR()
			.AddApplicationServices();

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

	private static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddScoped<ICheckoutSessionService, CheckoutSessionService>();
		services.AddHostedService<CheckoutSessionExpirationService>();

		return services;
	}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talabat.OrderProcessing.Application.Services;
using Talabat.OrderProcessing.Data;
using Talabat.OrderProcessing.Data.Repositories;

namespace Talabat.Vender;

public static class DependencyInjection
{
	public static IServiceCollection AddOrderProcessingInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddPersistence(configuration)
			.AddMediatR();

		return services;
	}

	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<OrderProcessingDbContext>(cfg =>
		{
			cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
		});

		services.AddScoped<IOrderRepository, OrderRepository>();
		services.AddScoped<IOrderService, OrderService>();

		return services;
	}

	public static IServiceCollection AddMediatR(this IServiceCollection services)
	{
		services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

		return services;
	}
}

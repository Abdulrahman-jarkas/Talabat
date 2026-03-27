using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Checkout.Application.Services;
using Talabat.Checkout.Data;
using Talabat.Checkout.Data.Repositories;

namespace Talabat.Checkout;

public static class DependencyInjection
{
	public static IServiceCollection AddCheckoutInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddPersistence(configuration)
			.AddMediatR();

		return services;
	}

	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<CheckoutDbContext>(cfg =>
		{
			cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
		});

		services.AddScoped<ICheckoutSessionRepository, CheckoutSessionRepository>();
		services.AddScoped<ICheckoutSessionService, CheckoutSessionService>();
		services.AddScoped<IProductService, ProductService>();

		return services;
	}

	public static IServiceCollection AddMediatR(this IServiceCollection services)
	{
		services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

		return services;
	}
}

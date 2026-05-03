using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Products.Authorization;
using Talabat.Products.Data;
using Talabat.Products.Data.Repositories;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products;

public static class DependencyInjection
{
	public static IServiceCollection AddProductsInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		// Register plan configuration
		ProductsPlanConfiguration.Register();

		services.AddPersistence(configuration)
			.AddMediatR()
			.AddAuthorization()
			.AddEndpoints();

		return services;
	}

	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ProductsDbContext>(cfg =>
		{
			cfg.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
		});

		services.AddScoped<IProductsRepository, ProductsRepository>();

		return services;
	}

	public static IServiceCollection AddMediatR(this IServiceCollection services)
	{
		services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

		return services;
	}

	private static IServiceCollection AddAuthorization(this IServiceCollection services)
	{
		services.AddPlanLimitService<ProductsPlanLimitService>();

		return services;
	}

	private static IServiceCollection AddEndpoints(this IServiceCollection services)
	{
		EndpointAssemblyRegistry.Register(typeof(DependencyInjection).Assembly);

		return services;
	}
}

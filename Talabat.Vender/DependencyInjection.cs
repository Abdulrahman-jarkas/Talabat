using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Vender.Infrastructure.Persistence;
using Talabat.Vender.Infrastructure.Persistence.Repositories;
using Talabat.Vender.Services;

namespace Talabat.Vender;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddPersistence(configuration);
		return services;
	}

	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ProductsManagementDbContext>(cfg =>
		{
			cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
		});

		services.AddScoped<IProductsRepository, ProductsRepository>();
		services.AddScoped<IProductService, ProductService>();

		return services;
	}

	public static IServiceCollection AddEndpoints(this IServiceCollection services)
	{
		services.AddFastEndpoints();
		// app.UseFastEndpoints();
		return services;
	}
}

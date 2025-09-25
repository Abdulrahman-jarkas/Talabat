using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductsManagement.Infrastructure.Persistence;

namespace ProductsManagement.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddPersistence(configuration);

			return services;
		}

		public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ProductsManagementDbContext>(cfg => {
				cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
			});

			return services;
		}
	}
}

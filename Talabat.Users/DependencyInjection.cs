using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Users.Data;
using Talabat.Users.Data.Repositories;

namespace Talabat.Users;

public static class DependencyInjection
{
	public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddPersistence(configuration)
			.AddMediatR();

		return services;
	}

	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<UsersDbContext>(cfg =>
		{
			cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
		});

		services.AddScoped<IUsersRepository, UsersRepository>();
		services.AddScoped<ICustomerService, CustomerService>();

		return services;
	}

	public static IServiceCollection AddMediatR(this IServiceCollection services)
	{
		services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

		return services;
	}
}

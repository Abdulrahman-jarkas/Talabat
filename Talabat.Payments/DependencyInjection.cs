using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talabat.SharedKernal;

namespace Talabat.Payments;

public static class DependencyInjection
{
	public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddPersistence(configuration)
			.AddMediatR()
			.AddEndpoints();

		return services;
	}

	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<PaymentsDbContext>(cfg =>
		{
			cfg.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
		});

		services.AddScoped<IPaymentService, PaymentService>();
		services.AddScoped<IPaymentsRepository, PaymentsRepository>();

		return services;
	}

	public static IServiceCollection AddMediatR(this IServiceCollection services)
	{
		services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

		return services;
	}

	private static IServiceCollection AddEndpoints(this IServiceCollection services)
	{
		EndpointAssemblyRegistry.Register(typeof(DependencyInjection).Assembly);

		return services;
	}
}

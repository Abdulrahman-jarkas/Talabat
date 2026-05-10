using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Accounts.Application.Account.Services;
using Talabat.Accounts.Authorization;
using Talabat.Accounts.Data;
using Talabat.Accounts.Data.Repositories;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration)
            .AddIdentityHttpClient(configuration)
            .AddMediatR()
            .AddAuthorization()
            .AddEndpoints();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccountsDbContext>(cfg =>
        {
            cfg.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IAccountsRepository, AccountsRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IIdentityUserService, IdentityUserService>();

        return services;
    }

    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

        return services;
    }

    private static IServiceCollection AddIdentityHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var identityOptions = configuration
            .GetSection(IdentityClientOptions.SectionName)
            .Get<IdentityClientOptions>()!;

        services.AddDistributedMemoryCache();

        services.AddClientCredentialsTokenManagement()
            .AddClient("identity", client =>
            {
                client.TokenEndpoint = identityOptions.TokenEndpoint;
                client.ClientId = identityOptions.ClientId;
                client.ClientSecret = identityOptions.ClientSecret;
                client.Scope = identityOptions.Scope;
            });

        services.AddClientCredentialsHttpClient("identity.api", "identity", client =>
        {
            client.BaseAddress = new Uri(identityOptions.BaseAddress);
        });

        return services;
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAccountAuthorizationDataProvider, AccountAuthorizationDataProvider>();

        return services;
    }

    private static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        EndpointAssemblyRegistry.Register(typeof(DependencyInjection).Assembly);

        return services;
    }
}

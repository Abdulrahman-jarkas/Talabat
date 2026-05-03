using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Talabat.Api.Middleware;
using Talabat.Orders;
using Talabat.Payments;
using Talabat.Products;
using Talabat.Products.Data;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
using Talabat.Accounts;
using Talabat.Accounts.Data;
using Talabat.Users;
using Talabat.Users.Data;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog from appsettings
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    // Configure JWT Authentication
    var authority = builder.Configuration["Authentication:Authority"]!;

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Audience = "talabat.api";

        if (builder.Environment.IsDevelopment())
        {
            // Fetch JWKS manually for development (handles self-signed certs)
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var httpClient = new HttpClient(handler);
            var jwksJson = httpClient.GetStringAsync($"{authority}/.well-known/openid-configuration/jwks").GetAwaiter().GetResult();
            var jsonWebKeySet = new JsonWebKeySet(jwksJson);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authority,
                ValidateAudience = true,
                ValidAudience = "talabat.api",
                ValidateLifetime = true,
                NameClaimType = "name",
                RoleClaimType = AuthorizationClaimTypes.Role,
                IssuerSigningKeys = jsonWebKeySet.GetSigningKeys()
            };
        }
        else
        {
            // Production: use standard Authority-based discovery
            options.Authority = authority;
            options.TokenValidationParameters.NameClaimType = "name";
            options.TokenValidationParameters.RoleClaimType = AuthorizationClaimTypes.Role;
        }
    });

    // Register shared authorization infrastructure
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddAuthorization();
    builder.Services.AddSharedAuthorization();

    // Register module services
    builder.Services.AddOrdersInfrastructure(builder.Configuration);
    builder.Services.AddProductsInfrastructure(builder.Configuration);
    builder.Services.AddPaymentsInfrastructure(builder.Configuration);
    builder.Services.AddUsersInfrastructure(builder.Configuration);
    builder.Services.AddAccountsInfrastructure(builder.Configuration);

    // Register FastEndpoints
    builder.Services
        .AddFastEndpoints(o => o.Assemblies = [.. EndpointAssemblyRegistry.Assemblies])
        .SwaggerDocument(o =>
        {
            o.DocumentSettings = s =>
            {
                s.Title = "Talabat API";
                s.Version = "v1";
                s.Description = "Talabat Modular Monolith API";
            };
        });

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseFastEndpoints(c =>
    {
        c.Serializer.Options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

    app.UseSwaggerGen();

    if (app.Environment.IsDevelopment())
    {
        await app.Services.SeedUsersDataAsync();
        await app.Services.SeedProductsDataAsync();
        await app.Services.SeedAccountsDataAsync();
    }

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

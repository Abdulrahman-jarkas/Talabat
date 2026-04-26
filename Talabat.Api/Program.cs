using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Talabat.Api.Middleware;
using Talabat.Orders;
using Talabat.Payments;
using Talabat.Products;
using Talabat.Products.Data;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
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

	// Configure JWT Bearer Authentication
	var identityServerAuthority = builder.Configuration["IdentityServer:Authority"]
		?? throw new InvalidOperationException("IdentityServer:Authority configuration is required.");

	builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		.AddJwtBearer(options =>
		{
			options.Authority = identityServerAuthority;
			options.Audience = "talabat.api";
			options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ClockSkew = TimeSpan.FromSeconds(30),
				NameClaimType = "name",
				RoleClaimType = AuthorizationClaimTypes.Role
			};

			options.Events = new JwtBearerEvents
			{
				OnAuthenticationFailed = context =>
				{
					Log.Warning(
						context.Exception,
						"Authentication failed for request {Path}",
						context.Request.Path);
					return Task.CompletedTask;
				},
				OnTokenValidated = context =>
				{
					var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
					var tenantType = context.Principal?.FindFirstValue(AuthorizationClaimTypes.TenantType);

					Log.Debug(
						"Token validated for user {UserId} with tenant type {TenantType}",
						userId,
						tenantType);
					return Task.CompletedTask;
				},
				OnChallenge = context =>
				{
					Log.Warning(
						"Authentication challenge for request {Path}: {Error} - {ErrorDescription}",
						context.Request.Path,
						context.Error,
						context.ErrorDescription);
					return Task.CompletedTask;
				}
			};
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

	// Register FastEndpoints — assemblies are registered by each module's DI
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

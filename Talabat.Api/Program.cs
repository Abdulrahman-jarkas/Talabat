using FastEndpoints;
using FastEndpoints.Swagger;
using Talabat.Api.Middleware;
using Talabat.Orders;
using Talabat.Payments;
using Talabat.Products;
using Talabat.Products.Data;
using Talabat.SharedKernal;
using Talabat.Users;
using Talabat.Users.Data;

var builder = WebApplication.CreateBuilder(args);

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

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

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

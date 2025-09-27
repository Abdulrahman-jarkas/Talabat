using FastEndpoints;
using FastEndpoints.Swagger;
using Talabat.Vender;

var builder = WebApplication.CreateBuilder(args);
{
	builder.Services
		.AddInfrastructure(builder.Configuration)
		.AddFastEndpoints()
		.SwaggerDocument();
}

var app = builder.Build();
{
	app.UseFastEndpoints();
	app.UseSwaggerGen();
	app.Run();
}
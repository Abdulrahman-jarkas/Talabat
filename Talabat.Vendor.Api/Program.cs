using FastEndpoints;
using Talabat.Vender;

var builder = WebApplication.CreateBuilder(args);
{
	builder.Services
		.AddInfrastructure(builder.Configuration);
}

var app = builder.Build();
{
	app.UseFastEndpoints();
	app.Run();
}
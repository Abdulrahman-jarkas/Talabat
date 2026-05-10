using System.Net;
using System.Text.Json;
using Talabat.SharedKernal;

namespace Talabat.Api.Middleware;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await next(context);
		}
		catch (EventualConsistencyException ex)
		{
			logger.LogError(ex, "Eventual consistency error: {Code} - {Description}",
				ex.EventualConsistencyError.Code,
				ex.EventualConsistencyError.Description);

			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			context.Response.ContentType = "application/json";

			var response = ApiResponse.Fail<object?>("An internal processing error occurred. Please try again later.");
			await context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Unhandled exception");

			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			context.Response.ContentType = "application/json";

			var response = ApiResponse.Fail<object?>("An unexpected error occurred.");
			await context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}
	}
}

using ErrorOr;

namespace Talabat.SharedKernal;

public static class ErrorOrExtensions
{
	public static (ApiResponse<T?> Response, int StatusCode) ToApiResult<T>(this ErrorOr<T> result)
	{
		if (!result.IsError)
			return (ApiResponse.Ok<T?>(result.Value), 200);

		return MapErrors<T?>(result.Errors);
	}

	public static (ApiResponse<T?> Response, int StatusCode) ToCreatedApiResult<T>(this ErrorOr<T> result)
	{
		if (!result.IsError)
			return (new ApiResponse<T?>(true, result.Value, null), 201);

		return MapErrors<T?>(result.Errors);
	}

	private static (ApiResponse<T> Response, int StatusCode) MapErrors<T>(List<Error> errors)
	{
		var messages = errors.Select(e => e.Description).ToArray();

		var statusCode = errors[0].Type switch
		{
			ErrorType.NotFound => 404,
			ErrorType.Validation => 400,
			ErrorType.Conflict => 409,
			ErrorType.Unauthorized => 401,
			ErrorType.Forbidden => 403,
			_ => 500
		};

		return (ApiResponse.Fail<T>(messages), statusCode);
	}
}

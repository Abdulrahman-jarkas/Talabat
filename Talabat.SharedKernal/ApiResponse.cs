namespace Talabat.SharedKernal;

public record ApiResponse<T>(bool Success, T? Data, string[]? Errors);

public static class ApiResponse
{
	public static ApiResponse<T> Ok<T>(T data) => new(true, data, null);

	public static ApiResponse<object?> Ok() => new(true, null, null);

	public static ApiResponse<T?> Fail<T>(params string[] errors) => new(false, default, errors);
}

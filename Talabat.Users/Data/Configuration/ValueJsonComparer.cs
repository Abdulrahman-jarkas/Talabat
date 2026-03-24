using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talabat.Users.Data.Configuration;

public class ValueJsonComparer<T> : ValueComparer<T>
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		NumberHandling = JsonNumberHandling.AllowReadingFromString,
		PropertyNameCaseInsensitive = false,
		IncludeFields = true,
		PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate
	};

	public ValueJsonComparer() : base(
		(l, r) => JsonSerializer.Serialize(l, JsonOptions) == JsonSerializer.Serialize(r, JsonOptions),
		v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOptions).GetHashCode(),
		v => v) // Return the same instance instead of deserializing - simpler snapshot
	{
	}
}

public class ValueJsonConverter<T> : ValueConverter<T, string>
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		NumberHandling = JsonNumberHandling.AllowReadingFromString,
		PropertyNameCaseInsensitive = false,
		IncludeFields = true,
		PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate
	};

	public ValueJsonConverter(ConverterMappingHints? mappingHints = null)
		: base(
			v => JsonSerializer.Serialize(v, JsonOptions),
			v => JsonSerializer.Deserialize<T>(v, JsonOptions)!,
			mappingHints)
	{
	}
}

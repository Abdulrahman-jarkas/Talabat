using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Talabat.Taxes.Configuration;

public static class FluentApiExtensions
{
	public static PropertyBuilder<T> HasValueJsonConverter<T>(this PropertyBuilder<T> propertyBuilder)
	{
		return propertyBuilder.HasConversion(
			new ValueJsonConverter<T>(),
			new ValueJsonComparer<T>());
	}
}
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.OrderProcessing.Data.Configuration;

namespace Talabat.Vender.OrderProcessing.Data.Configuration;

public static class FluentApiExtensions
{
	public static PropertyBuilder<T> HasValueJsonConverter<T>(this PropertyBuilder<T> propertyBuilder)
	{
		return propertyBuilder.HasConversion(
			new ValueJsonConverter<T>(),
			new ValueJsonComparer<T>());
	}
}
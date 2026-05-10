using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Talabat.Users.Data.Configuration;

public static class FluentApiExtensions
{
    public static PropertyBuilder<T> HasValueJsonConverter<T>(this PropertyBuilder<T> propertyBuilder)
    {
        return propertyBuilder.HasConversion(
            new ValueJsonConverter<T>(),
            new ValueJsonComparer<T>());
    }

    public static PropertyBuilder HasValueJsonConverter(this PropertyBuilder propertyBuilder)
    {
        var propertyType = propertyBuilder.Metadata.ClrType;
        var converterType = typeof(ValueJsonConverter<>).MakeGenericType(propertyType);
        var comparerType = typeof(ValueJsonComparer<>).MakeGenericType(propertyType);

        var converter = Activator.CreateInstance(converterType);
        var comparer = Activator.CreateInstance(comparerType);

        return propertyBuilder.HasConversion(
            (Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter)converter!,
            (Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer)comparer!);
    }
}

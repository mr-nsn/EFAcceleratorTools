namespace EFAcceleratorTools.Select.Helpers;

/// <summary>
/// Provides helper methods for determining the complexity of types,
/// such as whether a type is simple, complex, or a collection.
/// </summary>
public static class ComplexObjectsHelper
{
    /// <summary>
    /// Determines whether the specified type is considered simple.
    /// Simple types include primitives, enums, strings, decimals, DateTime, and nullable versions of these types.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is simple; otherwise, <c>false</c>.</returns>
    public static bool IsSimple(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // Nullable type, check if the nested type is simple.
            return IsSimple(type.GetGenericArguments()[0]);
        }

        return type.IsPrimitive
          || type.IsEnum
          || type.Equals(typeof(string))
          || type.Equals(typeof(decimal))
          || type.Equals(typeof(DateTime));
    }

    /// <summary>
    /// Determines whether the specified type is considered complex.
    /// A complex type is not simple and not a collection.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is complex; otherwise, <c>false</c>.</returns>
    public static bool IsComplex(Type type)
    {
        return !IsSimple(type) && !IsCollection(type);
    }

    /// <summary>
    /// Determines whether the specified type is a collection (excluding strings).
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is a collection; otherwise, <c>false</c>.</returns>
    public static bool IsCollection(Type type)
    {
        var typeFullName = type.FullName ?? string.Empty;
        var collectionsNameSpace = string.Format("{0}.{1}.{2}", nameof(System), nameof(System.Collections), nameof(System.Collections.Generic));
        return typeFullName.StartsWith(collectionsNameSpace) && type != typeof(string);
    }
}

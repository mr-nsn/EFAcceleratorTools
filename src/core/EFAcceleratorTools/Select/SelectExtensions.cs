using Apparatus.AOT.Reflection;
using EFAcceleratorTools.Select.Helpers;

namespace EFAcceleratorTools.Select;

/// <summary>
/// Provides extension methods for dynamically selecting specific fields from an <see cref="IQueryable{T}"/>.
/// </summary>
public static class SelectExtensions
{
    /// <summary>
    /// Projects each element of a sequence into a new form, including only the specified fields.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence. Must be a class.</typeparam>
    /// <param name="query">The source queryable sequence.</param>
    /// <param name="fields">The names of the fields to include in the projection. Supports nested properties using dot notation.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that, when enumerated, will return objects of type <typeparamref name="T"/> with only the specified fields populated.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fields"/> is null or empty.</exception>
    public static IQueryable<T> DynamicSelect<T>(this IQueryable<T> query, params KeyOf<T>[] fields) where T : class
    {
        return query.Select(SelectHelper.DynamicSelectGenerator<T>(fields.Select(f => f.Value).ToArray()));
    }
}

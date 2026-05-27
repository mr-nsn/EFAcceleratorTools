using EFAcceleratorTools.Select.Helpers;

namespace EFAcceleratorTools.Select.Defaults;

/// <summary>
/// Default selection metadata for an entity type, exposing simple property names for lightweight projections.
/// </summary>
/// <typeparam name="T">
/// Entity type for which default fields are resolved. Must be instantiable via parameterless constructor to support <see cref="System.Activator.CreateInstance{T}"/>.
/// </typeparam>
/// <remarks>
/// Includes only properties considered simple by <see cref="CleanArchTypeExtensions.IsSimple(System.Type)"/> (e.g., primitives, strings, enums).
/// </remarks>
public static class SelectsDefaults<T>
{
    /// <summary>
    /// Returns default field names for <typeparamref name="T"/>, including only simple properties.
    /// </summary>
    /// <exception cref="System.MissingMethodException">Thrown if <typeparamref name="T"/> lacks a parameterless constructor.</exception>
    /// <exception cref="System.MemberAccessException">Thrown if the constructor is inaccessible or <typeparamref name="T"/> is abstract.</exception>
    /// <exception cref="System.Reflection.TargetInvocationException">Thrown if the constructor of <typeparamref name="T"/> throws an exception.</exception>
    public static string[] BasicFields =>
        typeof(T)
            .GetProperties()
            .Where(p => ComplexObjectsHelper.IsSimple(p.PropertyType))
            .Select(p =>
            {
                return p.Name;
            })
            .ToArray();
}

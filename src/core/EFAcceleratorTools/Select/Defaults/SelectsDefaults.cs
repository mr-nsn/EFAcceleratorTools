using Apparatus.AOT.Reflection;
using EFAcceleratorTools.Select.Helpers;

namespace EFAcceleratorTools.Select.Defaults;

public static class SelectsDefaults<T>
{
    public static KeyOf<T>[] BasicFields =>
    [
        .. Activator.CreateInstance<T>()
            .GetProperties()
            .Values
            .Where(v =>  ComplexObjectsHelper.IsSimple(v.PropertyType))
            .Select(v =>
            {
                KeyOf<T>.TryParse(v.Name, out var key);
                return key;
            })
    ];
}

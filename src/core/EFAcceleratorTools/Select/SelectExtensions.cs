using EFAcceleratorTools.Select.Helpers;

namespace EFAcceleratorTools.Select;

public static class SelectExtensions
{
    public static IQueryable<T> DynamicSelect<T>(this IQueryable<T> query, params string[] fields) where T : class
    {
        return query.Select(SelectHelper.DynamicSelectGenerator<T>(fields));
    }
}

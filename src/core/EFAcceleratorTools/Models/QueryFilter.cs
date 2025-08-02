using Apparatus.AOT.Reflection;
using EFAcceleratorTools.Select.Defaults;

namespace EFAcceleratorTools.Models;

/// <summary>
/// Represents a filter for queries, including pagination and field selection options.
/// </summary>
/// <typeparam name="T">The type of the entity being filtered.</typeparam>
public class QueryFilter<T>
{
    /// <summary>
    /// Gets the current page number for the query.
    /// </summary>
    public int Page { get; private set; }

    /// <summary>
    /// Gets the number of items per page for the query.
    /// </summary>
    public int PageSize { get; private set; }

    /// <summary>
    /// Gets the fields to be selected in the query, specified as <see cref="KeyOf{T}"/> values.
    /// </summary>
    public KeyOf<T>[] Fields { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryFilter{T}"/> class with default values.
    /// </summary>
    public QueryFilter()
    {
        Page = 1;
        PageSize = 10;
        Fields = SelectsDefaults<T>.BasicFields;
    }

    /// <summary>
    /// Sets the current page number for the query.
    /// </summary>
    /// <param name="page">The page number. Must be greater than or equal to 1.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="page"/> is less than 1.</exception>
    public void SetPage(int page)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than or equal to 1.");
        Page = page;
    }

    /// <summary>
    /// Sets the number of items per page for the query.
    /// </summary>
    /// <param name="pageSize">The page size. Must be greater than or equal to 1.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="pageSize"/> is less than 1.</exception>
    public void SetPageSize(int pageSize)
    {
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than or equal to 1.");
        PageSize = pageSize;
    }

    /// <summary>
    /// Sets the fields to be selected in the query.
    /// </summary>
    /// <param name="fields">The fields to include, specified as <see cref="KeyOf{T}"/> values. Cannot be null or empty.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="fields"/> is null or empty.</exception>
    public void SetFields(params KeyOf<T>[] fields)
    {
        if (fields == null || fields.Length == 0) throw new ArgumentException("Fields cannot be null or empty.", nameof(fields));
        Fields = fields;
    }
}
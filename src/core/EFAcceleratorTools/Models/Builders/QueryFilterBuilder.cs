using Apparatus.AOT.Reflection;

namespace EFAcceleratorTools.Models.Builders;

/// <summary>
/// Provides a fluent builder for constructing <see cref="QueryFilter{T}"/> instances.
/// Allows step-by-step configuration of query filter parameters such as pagination and selected fields.
/// </summary>
/// <typeparam name="T">The type of the entity being filtered.</typeparam>
public class QueryFilterBuilder<T>
{
    private readonly QueryFilter<T> _queryFilter;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryFilterBuilder{T}"/> class.
    /// </summary>
    public QueryFilterBuilder()
    {
        _queryFilter = new QueryFilter<T>();
    }

    /// <summary>
    /// Sets the current page number for the query filter.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <returns>The current builder instance.</returns>
    public QueryFilterBuilder<T> WithPage(int page)
    {
        _queryFilter.SetPage(page);
        return this;
    }

    /// <summary>
    /// Sets the page size for the query filter.
    /// </summary>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>The current builder instance.</returns>
    public QueryFilterBuilder<T> WithPageSize(int pageSize)
    {
        _queryFilter.SetPageSize(pageSize);
        return this;
    }

    /// <summary>
    /// Sets the fields to be selected in the query filter.
    /// </summary>
    /// <param name="fields">The fields to include, specified as <see cref="KeyOf{T}"/> values.</param>
    /// <returns>The current builder instance.</returns>
    public QueryFilterBuilder<T> WithFields(params KeyOf<T>[] fields)
    {
        _queryFilter.SetFields(fields);
        return this;
    }

    /// <summary>
    /// Builds and returns the configured <see cref="QueryFilter{T}"/> instance.
    /// </summary>
    /// <returns>The constructed <see cref="QueryFilter{T}"/>.</returns>
    public QueryFilter<T> Build()
    {
        return _queryFilter;
    }
}
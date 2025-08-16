using Apparatus.AOT.Reflection;
using System.Linq.Expressions;

namespace EFAcceleratorTools.Models.Builders;

/// <summary>
/// Fluent builder for creating <see cref="QueryFilter{T}"/> instances.
/// Supports pagination, field selection, filters, and ordering (ascending/descending).
/// A default order is required to guarantee deterministic pagination when no explicit order is provided.
/// </summary>
/// <typeparam name="T">The entity type being filtered. Must inherit from <see cref="Entity"/>.</typeparam>
public class QueryFilterBuilder<T> where T : Entity
{
    private readonly QueryFilter<T> _queryFilter;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryFilterBuilder{T}"/> class.
    /// </summary>
    /// <param name="defaultOrder">
    /// The default ordering expression used when no explicit ordering is provided.
    /// This ensures deterministic pagination.
    /// </param>
    public QueryFilterBuilder(Expression<Func<T, object?>> defaultOrder)
    {
        _queryFilter = new QueryFilter<T>(defaultOrder);
    }

    /// <summary>
    /// Sets the current page number.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <returns>The current builder instance.</returns>
    public QueryFilterBuilder<T> WithPage(int page)
    {
        _queryFilter.SetPage(page);
        return this;
    }

    /// <summary>
    /// Sets the page size.
    /// </summary>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>The current builder instance.</returns>
    public QueryFilterBuilder<T> WithPageSize(int pageSize)
    {
        _queryFilter.SetPageSize(pageSize);
        return this;
    }

    /// <summary>
    /// Replaces all filter predicates with the provided collection.
    /// </summary>
    /// <param name="filters">A collection of filter expressions.</param>
    /// <returns>The current builder instance.</returns>
    public QueryFilterBuilder<T> WithFilters(ICollection<Expression<Func<T, bool>>> filters)
    {
        _queryFilter.SetFilters(filters);
        return this;
    }

    /// <summary>
    /// Adds a single filter predicate to the current set of filters.
    /// </summary>
    /// <param name="filter">The filter expression to add.</param>
    /// <returns>The current builder instance.</returns>
    public QueryFilterBuilder<T> WithFilter(Expression<Func<T, bool>> filter)
    {
        _queryFilter.AddFilter(filter);
        return this;
    }

    /// <summary>
    /// Replaces all ascending order expressions with the provided collection.
    /// </summary>
    /// <param name="orders">A collection of ascending order expressions.</param>
    /// <returns>The current builder instance.</returns>
    public QueryFilterBuilder<T> WithOrdersAscending(ICollection<Expression<Func<T, object?>>> orders)
    {
        _queryFilter.SetOrdersAscending(orders);
        return this;
    }

    /// <summary>
    /// Adds a single ascending order expression.
    /// </summary>
    /// <param name="order">The ascending order expression to add.</param>
    /// <returns>The current builder instance.</returns>
    public QueryFilterBuilder<T> WithOrderAscending(Expression<Func<T, object?>> order)
    {
        _queryFilter.AddOrderAscending(order);
        return this;
    }

    /// <summary>
    /// Replaces all descending order expressions with the provided collection.
    /// </summary>
    /// <param name="orders">A collection of descending order expressions.</param>
    /// <returns>The current builder instance.</returns>
    public QueryFilterBuilder<T> WithOrdersDescending(ICollection<Expression<Func<T, object?>>> orders)
    {
        _queryFilter.SetOrdersDescending(orders);
        return this;
    }

    /// <summary>
    /// Adds a single descending order expression.
    /// </summary>
    /// <param name="order">The descending order expression to add.</param>
    /// <returns>The current builder instance.</returns>
    public QueryFilterBuilder<T> WithOrderDescending(Expression<Func<T, object?>> order)
    {
        _queryFilter.AddOrderDescending(order);
        return this;
    }

    /// <summary>
    /// Sets the fields to be selected in the query.
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
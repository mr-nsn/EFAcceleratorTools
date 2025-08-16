using Apparatus.AOT.Reflection;
using EFAcceleratorTools.Select.Defaults;
using System.Linq.Expressions;

namespace EFAcceleratorTools.Models;

/// <summary>
/// Represents a filter for queries, including pagination, field selection, filtering and ordering options.
/// A default order is required to ensure deterministic pagination when no explicit order is provided.
/// </summary>
/// <typeparam name="T">The type of the entity being filtered. Must inherit from <see cref="Entity"/>.</typeparam>
public class QueryFilter<T> where T : Entity
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
    /// Gets the collection of filter predicates to apply to the query.
    /// </summary>
    public ICollection<Expression<Func<T, bool>>> Filters { get; private set; }

    /// <summary>
    /// Gets the collection of ordering expressions to be applied in ascending order.
    /// </summary>
    public ICollection<Expression<Func<T, object?>>> OrdersAscending { get; private set; }

    /// <summary>
    /// Gets the collection of ordering expressions to be applied in descending order.
    /// </summary>
    public ICollection<Expression<Func<T, object?>>> OrdersDescending { get; private set; }

    /// <summary>
    /// Gets the fields to be selected in the query, specified as <see cref="KeyOf{T}"/> values.
    /// </summary>
    public KeyOf<T>[] Fields { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryFilter{T}"/> class with default values.
    /// </summary>
    /// <param name="defaultOrder">
    /// The default ordering expression used when no explicit ordering is provided.
    /// This ensures deterministic pagination.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="defaultOrder"/> is null.</exception>
    public QueryFilter(Expression<Func<T, object?>> defaultOrder)
    {
        Page = 1;
        PageSize = 10;
        Fields = SelectsDefaults<T>.BasicFields;
        Filters = new List<Expression<Func<T, bool>>>
        {
            _ => true
        };
        OrdersAscending = new List<Expression<Func<T, object?>>>
        {
            defaultOrder ?? throw new ArgumentNullException(nameof(defaultOrder), "Default order cannot be null.")
        };
        OrdersDescending = new List<Expression<Func<T, object?>>>();
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
    /// Replaces all filter predicates with the provided collection.
    /// </summary>
    /// <param name="filter">The collection of filter expressions to set.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="filter"/> is null.</exception>
    public void SetFilters(ICollection<Expression<Func<T, bool>>> filter)
    {
        Filters = filter ?? throw new ArgumentNullException(nameof(filter), "Filter cannot be null.");
    }

    /// <summary>
    /// Adds a single filter predicate to the current set of filters.
    /// </summary>
    /// <param name="filter">The filter expression to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="filter"/> is null.</exception>
    public void AddFilter(Expression<Func<T, bool>> filter)
    {
        if (filter is null) throw new ArgumentNullException(nameof(filter), "Filter cannot be null.");
        Filters.Add(filter);
    }

    /// <summary>
    /// Replaces all ascending order expressions with the provided collection.
    /// </summary>
    /// <param name="ordersAscending">The collection of ascending order expressions.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="ordersAscending"/> is null.</exception>
    public void SetOrdersAscending(ICollection<Expression<Func<T, object?>>> ordersAscending)
    {
        OrdersAscending = ordersAscending ?? throw new ArgumentNullException(nameof(ordersAscending), "Order cannot be null.");
    }

    /// <summary>
    /// Adds a single ascending order expression.
    /// </summary>
    /// <param name="orderAscending">The ascending order expression to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="orderAscending"/> is null.</exception>
    public void AddOrderAscending(Expression<Func<T, object?>> orderAscending)
    {
        if (orderAscending is null) throw new ArgumentNullException(nameof(orderAscending), "Order cannot be null.");
        OrdersAscending.Add(orderAscending);
    }

    /// <summary>
    /// Replaces all descending order expressions with the provided collection.
    /// </summary>
    /// <param name="ordersDescending">The collection of descending order expressions.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="ordersDescending"/> is null.</exception>
    public void SetOrdersDescending(ICollection<Expression<Func<T, object?>>> ordersDescending)
    {
        OrdersDescending = ordersDescending ?? throw new ArgumentNullException(nameof(ordersDescending), "Order cannot be null.");
    }

    /// <summary>
    /// Adds a single descending order expression.
    /// </summary>
    /// <param name="orderDescending">The descending order expression to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="orderDescending"/> is null.</exception>
    public void AddOrderDescending(Expression<Func<T, object?>> orderDescending)
    {
        if (orderDescending is null) throw new ArgumentNullException(nameof(orderDescending), "Order cannot be null.");
        OrdersDescending.Add(orderDescending);
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
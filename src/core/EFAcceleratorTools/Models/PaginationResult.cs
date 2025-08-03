namespace EFAcceleratorTools.Models;

/// <summary>
/// Represents the result of a paginated query, including pagination metadata and the result set.
/// </summary>
/// <typeparam name="T">The type of the items in the result set.</typeparam>
public class PaginationResult<T>
{
    /// <summary>
    /// Gets the current page number.
    /// </summary>
    public int Page { get; private set; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; private set; }

    /// <summary>
    /// Gets the total number of records available.
    /// </summary>
    public int TotalRecords { get; private set; }

    /// <summary>
    /// Gets the query used to retrieve the paginated data.
    /// </summary>
    public IQueryable<T> Query { get; private set; }

    /// <summary>
    /// Gets the collection of items for the current page.
    /// </summary>
    public ICollection<T> Result { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationResult{T}"/> class with default values.
    /// </summary>
    public PaginationResult()
    {
        Page = 1;
        PageSize = 10;
        TotalRecords = 0;
        Query = Enumerable.Empty<T>().AsQueryable();
        Result = Enumerable.Empty<T>().ToList();
    }

    /// <summary>
    /// Sets the current page number.
    /// </summary>
    /// <param name="page">The page number. Must be greater than or equal to 1.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="page"/> is less than 1.</exception>
    public void SetPage(int page)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than or equal to 1.");
        Page = page;
    }

    /// <summary>
    /// Sets the number of items per page.
    /// </summary>
    /// <param name="pageSize">The page size. Must be greater than or equal to 1.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="pageSize"/> is less than 1.</exception>
    public void SetPageSize(int pageSize)
    {
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than or equal to 1.");
        PageSize = pageSize;
    }

    /// <summary>
    /// Sets the total number of records available.
    /// </summary>
    /// <param name="totalRecords">The total number of records. Cannot be negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="totalRecords"/> is negative.</exception>
    public void SetTotalRecords(int totalRecords)
    {
        if (totalRecords < 0) throw new ArgumentOutOfRangeException(nameof(totalRecords), "Total records cannot be negative.");
        TotalRecords = totalRecords;
    }

    /// <summary>
    /// Sets the query used to retrieve the paginated data.
    /// </summary>
    /// <param name="query">The query representing the data source.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is null.</exception>
    public void SetQuery(IQueryable<T> query)
    {
        Query = query ?? throw new ArgumentNullException(nameof(query), "Query cannot be null.");
    }

    /// <summary>
    /// Sets the result collection for the current page.
    /// If <paramref name="result"/> is null, the result will be populated from the <see cref="Query"/>.
    /// </summary>
    /// <param name="result">The collection of items for the current page, or <c>null</c> to use the query result.</param>
    public void SetResult(ICollection<T>? result = null)
    {
        Result = result ?? Query.ToList();
    }
}

namespace EFAcceleratorTools.Models.Builders;

/// <summary>
/// Provides a fluent builder for constructing <see cref="PaginationResult{T}"/> instances.
/// Allows step-by-step configuration of pagination parameters and results.
/// </summary>
/// <typeparam name="T">The type of the items being paginated.</typeparam>
public class PaginationResultBuilder<T>
{
    private readonly PaginationResult<T> _paginationResult;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationResultBuilder{T}"/> class.
    /// </summary>
    public PaginationResultBuilder()
    {
        _paginationResult = new PaginationResult<T>();
    }

    /// <summary>
    /// Sets the current page number for the pagination result.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <returns>The current builder instance.</returns>
    public PaginationResultBuilder<T> WithPage(int page)
    {
        _paginationResult.SetPage(page);
        return this;
    }

    /// <summary>
    /// Sets the page size for the pagination result.
    /// </summary>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>The current builder instance.</returns>
    public PaginationResultBuilder<T> WithPageSize(int pageSize)
    {
        _paginationResult.SetPageSize(pageSize);
        return this;
    }

    /// <summary>
    /// Sets the total number of records for the pagination result.
    /// </summary>
    /// <param name="totalRecords">The total number of records.</param>
    /// <returns>The current builder instance.</returns>
    public PaginationResultBuilder<T> WithTotalRecords(int totalRecords)
    {
        _paginationResult.SetTotalRecords(totalRecords);
        return this;
    }

    /// <summary>
    /// Sets the query used to retrieve the paginated data.
    /// </summary>
    /// <param name="query">The query representing the data source.</param>
    /// <returns>The current builder instance.</returns>
    public PaginationResultBuilder<T> WithQuery(IQueryable<T> query)
    {
        _paginationResult.SetQuery(query);
        return this;
    }

    /// <summary>
    /// Sets the result collection for the pagination result.
    /// </summary>
    /// <param name="result">The collection of items for the current page.</param>
    /// <returns>The current builder instance.</returns>
    public PaginationResultBuilder<T> WithResult(ICollection<T> result)
    {
        _paginationResult.SetResult(result);
        return this;
    }

    /// <summary>
    /// Builds and returns the configured <see cref="PaginationResult{T}"/> instance.
    /// </summary>
    /// <returns>The constructed <see cref="PaginationResult{T}"/>.</returns>
    public PaginationResult<T> Build()
    {
        return _paginationResult;
    }
}